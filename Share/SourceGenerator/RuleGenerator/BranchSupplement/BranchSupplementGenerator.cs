/****************************************

* 作者：闪电黑客
* 日期：2024/5/20 15:39

* 描述：分支类型补充生成器

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Reflection.Metadata;
using System.Security;
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 分支类型补充生成器
	/// </summary>
	[Generator]
	internal class BranchSupplementGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubClassSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindSubClassSyntaxReceiver receiver)) return;
			if (receiver.ClassDeclarations.Count == 0) return;

			BranchSupplementHelper.Init(context.Compilation);
			foreach (var ClassDeclaration in receiver.ClassDeclarations)
			{
				BranchSupplementHelper.Add(ClassDeclaration, context.Compilation);
			}
			BranchSupplementHelper.Execute(context);
		}
	}

	public static class BranchSupplementHelper
	{
		/// <summary>
		/// 文件名-接口集合
		/// </summary>
		public static Dictionary<string, List<INamedTypeSymbol>> fileClassDict = new();
		/// <summary>
		/// 文件名-引用
		/// </summary>
		public static Dictionary<string, string> fileUsings = new();
		/// <summary>
		/// 文件名-命名空间
		/// </summary>
		public static Dictionary<string, string> fileNamespace = new();
		/// <summary>
		/// 接口名-语法树
		/// </summary>
		public static Dictionary<string, ClassDeclarationSyntax> classSyntax = new();

		public static string IBranch = "IBranch";
		public static string IBranchIdKey = "IBranchIdKey";
		public static string IBranchTypeKey = "IBranchTypeKey";


		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			fileNamespace.Clear();
			classSyntax.Clear();
		}

		public static void Add(ClassDeclarationSyntax classDeclarationSyntax, Compilation compilation)
		{
			INamedTypeSymbol? namedType = compilation.ToINamedTypeSymbol(classDeclarationSyntax);
			if (namedType == null) return;
			if (namedType.Name == "Branch") return;

			//检测是否继承分支基类
			if (!NamedSymbolHelper.CheckInterface(namedType, IBranch, out _)) return;
			if (namedType.IsGenericType) return;

			string fileName = Path.GetFileNameWithoutExtension(classDeclarationSyntax.SyntaxTree.FilePath);
			if (!fileClassDict.TryGetValue(fileName, out List<INamedTypeSymbol> set))
			{
				set = new List<INamedTypeSymbol>();
				fileClassDict.Add(fileName, set);
				fileUsings.Add(fileName, TreeSyntaxHelper.GetUsings(classDeclarationSyntax));
				fileNamespace.Add(fileName, TreeSyntaxHelper.GetNamespace(classDeclarationSyntax));
			}
			string className = namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			if (set.Any(a => a.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == className)) return;
			set.Add(namedType);
			classSyntax.Add(className, classDeclarationSyntax);


		}

		public static void Execute(GeneratorExecutionContext context)
		{
			var ISourceGeneratorIgnore = context.Compilation.ToINamedTypeSymbol("WorldTree.ISourceGeneratorIgnore");
			var IMethodRule = context.Compilation.ToINamedTypeSymbol("WorldTree.IMethodRule");

			if (ISourceGeneratorIgnore == null) return;
			if (IMethodRule == null) return;

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();
				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{

					if (NamedSymbolHelper.CheckAllInterface(fileClass, ISourceGeneratorIgnore)) continue;

					//bool isMethodRule = NamedSymbolHelper.CheckAllInterface(fileClass, IMethodRule);

					if (NamedSymbolHelper.CheckInterface(fileClass, IBranch, out var baseInterface))
					{
						BranchClass(ClassCode, fileClass, baseInterface);

						if (NamedSymbolHelper.CheckInterface(fileClass, IBranchIdKey, out _))
						{
							GetMethodIdKey(MethodCode, fileClass, baseInterface);
						}
						else if (NamedSymbolHelper.CheckInterface(fileClass, IBranchTypeKey, out _))
						{
							GetMethodTypeKey(MethodCode, fileClass, baseInterface);
						}
						else
						{
							GetMethod(MethodCode, fileClass, baseInterface);
						}
					}
				}

				if (MethodCode.ToString() != "")
				{
					ClassCode.AppendLine($"	public static class {fileClassList.Key}Supplement");
					ClassCode.AppendLine("	{");

					ClassCode.Append(MethodCode);

					ClassCode.AppendLine("	}");
				}

				if (ClassCode.ToString() != "")
				{

					fileCode.AppendLine(fileUsings[fileClassList.Key]);
					fileCode.AppendLine($"namespace {fileNamespace[fileClassList.Key]}");
					fileCode.AppendLine("{");
					fileCode.Append(ClassCode);
					fileCode.Append("}");
					context.AddSource($"{fileClassList.Key}Supplement.cs", SourceText.From(fileCode.ToString(), Encoding.UTF8));
				}
			}
		}

		private static void BranchClass(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseClass)
		{
			if (baseClass == null) return;
			//拿到类型包含命名空间全名
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();

			//拿到类型名称
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			//拿到泛型显示
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseClass, "\t");

			//拿到去除Branch的类名
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			//As约束接口
			AddComment(Code, "分支约束", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsBranch<{ClassFullName}>, INode {{}}");

			AddComment(Code, "父节点约束", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public interface {ClassNameUnBranch}Of<in P> : NodeOf<P,{ClassFullName}>, INode where P : class, INode {{}}");

		}

		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			//拿到键值泛型类型
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			AddComment(Code, "尝试获取分支", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, {genericType} key, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(key) as T) != null;");

			AddComment(Code, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, {genericType} key, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(key)?.CutSelf() as T) != null;");

			AddComment(Code, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, {genericType} key, T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(key, self);");

			AddComment(Code, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void Remove{ClassNameUnBranch}(this As{ClassFullName} self, {genericType} key)
		=> self.GetBranch<{ClassFullName}>()?.GetNode(key)?.Dispose();");

			AddComment(Code, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}(this As{ClassFullName} self)
		=> self.RemoveAllNode(TypeInfo<{ClassFullName}>.TypeCode);");


			//where T : class, INode, NodeOf<As{ClassFullName}, {ClassFullName}> , AsRule<Awake{genericsAngle}>
			//添加方法的生成
			int argumentCount = RuleGeneratorSetting.argumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string generics = RuleGeneratorHelper.GetGenerics(i);

				AddComment(Code, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				Code.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{generics}>(this N self, {genericType} key, out T node{genericTypeParameter}, bool isPool = true)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>, AsRule<Awake{genericsAngle}>
		=> self.AddNode<N, {ClassFullName}, {genericType}, T{generics}>(key, out node{genericParameter}, isPool);");
			}
		}


		public static void GetMethodIdKey(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			//拿到键值泛型类型
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			AddComment(Code, "尝试获取分支", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, {genericType} id, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(id) as T) != null;");

			AddComment(Code, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, {genericType} id, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(id)?.CutSelf() as T) != null;");

			AddComment(Code, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(node.Id, self);");

			AddComment(Code, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void Remove{ClassNameUnBranch}(this As{ClassFullName} self, {genericType} id)
			=> self.GetBranch<{ClassFullName}>()?.GetNode(id)?.Dispose();");

			AddComment(Code, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}(this As{ClassFullName} self)
			=> self.RemoveAllNode(TypeInfo<{ClassFullName}>.TypeCode);");

			//添加方法的生成
			int argumentCount = RuleGeneratorSetting.argumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string generics = RuleGeneratorHelper.GetGenerics(i);

				AddComment(Code, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				Code.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{generics}>(this N self, out T node{genericTypeParameter}, bool isPool = true)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>, AsRule<Awake{genericsAngle}>
		{{
			node = self.GetOrNewNode<T>(isPool);
			return (T)node.AddSelfToTree<{ClassFullName}, {genericType}{generics}>(node.Id, self{genericParameter});
		}}");

			}
		}


		public static void GetMethodTypeKey(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			//拿到键值泛型类型
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			AddComment(Code, "尝试获取分支", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(TypeInfo<T>.TypeCode) as T) != null;");

			AddComment(Code, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, out T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(TypeInfo<T>.TypeCode)?.CutSelf() as T) != null;");

			AddComment(Code, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, T node)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(TypeInfo<T>.TypeCode, self);");

			AddComment(Code, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void Remove{ClassNameUnBranch}<N, T>(this N self)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> self.GetBranch<{ClassFullName}>()?.GetNode(TypeInfo<T>.TypeCode)?.Dispose();");

			AddComment(Code, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}(this As{ClassFullName} self)
			=> self.RemoveAllNode(TypeInfo<{ClassFullName}>.TypeCode);");

			//添加方法的生成
			int argumentCount = RuleGeneratorSetting.argumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string generics = RuleGeneratorHelper.GetGenerics(i);

				AddComment(Code, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				Code.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{generics}>(this N self, out T node{genericTypeParameter}, bool isPool = true)
			where N : class, As{ClassFullName}
			where T : class, INode, NodeOf<N,{ClassFullName}> , AsRule<Awake{genericsAngle}>
		=> self.AddNode<N, {ClassFullName}, {genericType}, T{generics}>(TypeInfo<T>.TypeCode, out node{genericParameter}, isPool);");
			}
		}

		/// <summary>
		/// 添加法则继承注释
		/// </summary>
		public static string AddRuleExtendCommentPara(string IClassFullName, string IBaseFullName, string BaseTypePara, string Title, string tab)
		{
			StringBuilder sb = new();
			sb.AppendLine(@$"{tab}/// <Para>");
			sb.AppendLine(@$"{tab}/// {Title}: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{SecurityElement.Escape(IBaseFullName)}""/>");
			sb.AppendLine(@$"{tab}/// </Para>");
			sb.Append(@$"{BaseTypePara}");
			return sb.ToString();
		}

		/// <summary>
		/// 添加注释
		/// </summary>
		public static void AddComment(StringBuilder stringBuilder, string Title, string tab, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara)
		{
			string Para = AddRuleExtendCommentPara(ClassFullNameAndNameSpace, BaseFullName, BaseTypePara, Title, tab);
			stringBuilder.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], Para, tab));
		}



	}

}
