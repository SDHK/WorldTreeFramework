/****************************************

* 作者：闪电黑客
* 日期：2024/5/20 15:39

* 描述：分支类型补充生成器

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace WorldTree.SourceGenerator
{

	/// <summary>
	/// 分支类型补充生成器
	/// </summary>
	public abstract class BranchSupplementGenerator<C> : SourceGeneratorBase<C>
		where C : ProjectGeneratorsConfig, new()
	{
		public override void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubClassSyntaxReceiver());
		}

		public override void ExecuteCore(GeneratorExecutionContext context)
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


		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			fileNamespace.Clear();
			classSyntax.Clear();
		}

		public static void Add(ClassDeclarationSyntax classDeclarationSyntax, Compilation compilation)
		{
			INamedTypeSymbol namedType = compilation.ToINamedTypeSymbol(classDeclarationSyntax);
			if (namedType == null) return;
			//抽象类型忽略
			if (namedType.IsAbstract) return;

			//检测是否继承分支基类

			INamedTypeSymbol IBranchTypeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranch);
			if (!NamedSymbolHelper.IsDerivedFrom(namedType, IBranchTypeSymbol, out _, TypeCompareOptions.CompareToGenericTypeDefinition)) return;

			//获取文件名
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
			Compilation compilation = context.Compilation;
			var ISourceGeneratorIgnore = compilation.ToINamedTypeSymbol(GeneratorHelper.ISourceGeneratorIgnore);
			var IMethodRule = compilation.ToINamedTypeSymbol(GeneratorHelper.IMethodRule);

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

					INamedTypeSymbol IBranchTypeSymbol = NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranch_1);
					if (!NamedSymbolHelper.IsDerivedFrom(fileClass, IBranchTypeSymbol, out INamedTypeSymbol baseTypeSymbol, TypeCompareOptions.CompareToGenericTypeDefinition)) continue;
					//找到分支基类INamedTypeSymbol

					BranchClass(ClassCode, compilation, fileClass, baseTypeSymbol);
					if (NamedSymbolHelper.IsDerivedFrom(fileClass, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchIdKey), out _, TypeCompareOptions.None))
					{
						GetMethodIdKey(MethodCode, compilation, fileClass, baseTypeSymbol);
					}
					else if (NamedSymbolHelper.IsDerivedFrom(fileClass, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchTypeKey), out _, TypeCompareOptions.None))
					{
						GetMethodTypeKey(MethodCode, compilation, fileClass, baseTypeSymbol);
					}
					else if (fileClass.IsGenericType)
					{
						GetMethodGenericTypeKey(MethodCode, compilation, fileClass, baseTypeSymbol);
					}
					else
					{
						GetMethod(MethodCode, compilation, fileClass, baseTypeSymbol);
					}
				}

				if (MethodCode.ToString() != "")
				{
					ClassCode.AppendLine($"	/// <summary>");
					ClassCode.AppendLine($"	/// {fileClassList.Key}补充类");
					ClassCode.AppendLine($"	/// </summary>");
					ClassCode.AppendLine($"	public static class {fileClassList.Key}Supplement");
					ClassCode.AppendLine("	{");

					ClassCode.Append(MethodCode);

					ClassCode.AppendLine("	}");
				}

				if (ClassCode.ToString() != "")
				{
					fileCode.AppendLine("// <auto-generated />");
					fileCode.AppendLine("// 分支类型生成");
					fileCode.AppendLine("// </auto-generated>");
					fileCode.AppendLine(fileUsings[fileClassList.Key]);
					fileCode.AppendLine($"namespace {fileNamespace[fileClassList.Key]}");
					fileCode.AppendLine("{");
					fileCode.Append(ClassCode);
					fileCode.Append("}");
					context.AddSource($"{fileClassList.Key}Supplement.cs", SourceText.From(fileCode.ToString(), Encoding.UTF8));
				}
			}
		}

		private static void BranchClass(StringBuilder Code, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseClass)
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
			if (!NamedSymbolHelper.IsDerivedFrom(typeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchUnConstraint), out _, TypeCompareOptions.None))
			{
				AddComment(Code, "分支约束", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				Code.AppendLine(@$"	public interface As{ClassFullName} : AsBranch<{ClassFullName}> {{}}");
			}

			AddComment(Code, "父节点约束", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			if (typeSymbol.IsGenericType)
			{
				ClassNameUnBranch = ClassNameUnBranch.Split('<')[0];
				string genericType = baseClass.IsGenericType ? baseClass.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";
				Code.AppendLine(@$"	public interface {ClassNameUnBranch}Of<{genericType}, in P> : NodeOf<P,{ClassFullName}> where P : class, INode {{}}");
			}
			else
			{
				Code.AppendLine(@$"	public interface {ClassNameUnBranch}Of<in P> : NodeOf<P,{ClassFullName}> where P : class, INode {{}}");
			}


		}

		public static void GetMethodGenericTypeKey(StringBuilder Code, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			bool isUnConstraint = NamedSymbolHelper.IsDerivedFrom(typeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchUnConstraint), out _, TypeCompareOptions.None);

			BranchTypeNodeGetBranch(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchTypeTryGetNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchTypeCutNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchTypeGraftNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchRemoveNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchRemoveAllNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeAddNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
		}

		public static void GetMethod(StringBuilder Code, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			bool isUnConstraint = NamedSymbolHelper.IsDerivedFrom(typeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchUnConstraint), out _, TypeCompareOptions.None);

			NodeGetBranch(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTryGetNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchCutNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchGraftNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchRemoveNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchRemoveAllNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchAddNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
		}


		public static void GetMethodIdKey(StringBuilder Code, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			//拿到键值泛型类型
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			bool isUnConstraint = NamedSymbolHelper.IsDerivedFrom(typeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchUnConstraint), out _, TypeCompareOptions.None);

			NodeGetBranch(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchIdKeyTryGetNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchIdKeyCutNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchIdKeyGraftNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchIdKeyRemoveNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchIdKeyRemoveAllNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchIdKeyAddNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
		}


		public static void GetMethodTypeKey(StringBuilder Code, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			//拿到键值泛型类型
			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";

			bool isUnConstraint = NamedSymbolHelper.IsDerivedFrom(typeSymbol, NamedSymbolHelper.ToINamedTypeSymbol(compilation, GeneratorHelper.IBranchUnConstraint), out _, TypeCompareOptions.None);

			NodeGetBranch(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeKeyTryGetNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeKeyCutNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeKeyGraftNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchTypeKeyRemoveNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeKeyRemoveAllNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, isUnConstraint);
			BranchTypeKeyAddNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
			BranchBaseTypeKeyAddNode(Code, ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara, genericType, isUnConstraint);
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

		#region 泛型限制

		private static void NodeGetBranch(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			AddComment(stringBuilder, "尝试获取分支", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static {ClassFullName} {ClassFullName}<N>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
		=> NodeBranchHelper.GetBranch<{ClassFullName}>(self);");
		}

		#region 泛型分支

		private static void BranchTypeNodeGetBranch(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnGeneric = ClassFullName.Split('<')[0];

			AddComment(stringBuilder, "尝试获取分支", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static {ClassFullName} {ClassNameUnGeneric}<{genericType}, N>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
		=> NodeBranchHelper.GetBranch<{ClassFullName}>(self);");
		}

		private static void BranchTypeTryGetNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Split('<')[0].Replace("Branch", "");
			AddComment(stringBuilder, "尝试获取节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<{genericType}, N, T>(this N self, {genericType} key, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(key) as T) != null;");
		}

		private static void BranchTypeCutNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Split('<')[0].Replace("Branch", "");
			AddComment(stringBuilder, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<{genericType}, N, T>(this N self, {genericType} key, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(key)?.CutSelf() as T) != null;");
		}
		private static void BranchTypeGraftNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Split('<')[0].Replace("Branch", "");
			AddComment(stringBuilder, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<{genericType}, N, T>(this N self, {genericType} key, T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(key, self);");
		}

		private static void BranchTypeAddNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Split('<')[0].Replace("Branch", "");

			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				AddComment(stringBuilder, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				stringBuilder.AppendLine(@$"		public static T Add{ClassNameUnBranch}<{genericType}, N, T{genericsType}>(this N self, {genericType} key, out T node{genericTypeParameter})
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>, AsRule<Awake{genericsTypeAngle}>
		=> NodeBranchHelper.AddNode(self, default({ClassFullName}), key, out node{genericParameter});");
			}
		}

		#endregion

		#region 普通分支

		private static void BranchTryGetNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试获取节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, {genericType} key, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}> 
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(key) as T) != null;");
		}

		private static void BranchCutNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, {genericType} key, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(key)?.CutSelf() as T) != null;");
		}

		private static void BranchGraftNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, {genericType} key, T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(key, self);");
		}

		private static void BranchRemoveNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void Remove{ClassNameUnBranch}N>(this N self, {genericType} key)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
		=> NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(key)?.Dispose();");
		}

		private static void BranchRemoveAllNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}N>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
		=> self.RemoveAllNode(self.TypeToCode<{ClassFullName}>());");
		}

		private static void BranchAddNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				AddComment(stringBuilder, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				stringBuilder.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{genericsType}>(this N self, {genericType} key, out T node{genericTypeParameter})
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>, AsRule<Awake{genericsTypeAngle}>
		=> NodeBranchHelper.AddNode<N, {ClassFullName}, {genericType}, T{genericsType}>(self, default({ClassFullName}), key, out node{genericParameter});");
			}
		}

		#endregion

		#region Id键值分支

		private static void BranchIdKeyTryGetNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试获取节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, {genericType} id, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(id) as T) != null;");
		}
		private static void BranchIdKeyCutNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, {genericType} id, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(id)?.CutSelf() as T) != null;");
		}

		private static void BranchIdKeyGraftNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(node.Id, self);");
		}

		private static void BranchIdKeyRemoveNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void Remove{ClassNameUnBranch}N>(this N self, {genericType} id)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			=> NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(id)?.Dispose();");
		}

		private static void BranchIdKeyRemoveAllNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}N>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			=> self.RemoveAllNode(self.TypeToCode<{ClassFullName}>());");
		}
		private static void BranchIdKeyAddNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");

			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				AddComment(stringBuilder, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				stringBuilder.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{genericsType}>(this N self, out T node{genericTypeParameter})
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>, AsRule<Awake{genericsTypeAngle}>
		{{
			node = self.Core.PoolGetNode<T>();
			return (T)NodeBranchHelper.AddNodeToTree(self, default({ClassFullName}), node.Id, node{genericParameter});
		}}");

			}
		}
		#endregion

		#region 类型键值分支

		private static void BranchTypeKeyTryGetNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试获取节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGet{ClassNameUnBranch}<N, T>(this N self, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(self.TypeToCode<T>()) as T) != null;");
		}

		private static void BranchTypeKeyCutNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试裁剪节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryCut{ClassNameUnBranch}<N, T>(this N self, out T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> (node = NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(self.TypeToCode<T>())?.CutSelf() as T) != null;");
		}

		private static void BranchTypeKeyGraftNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			AddComment(stringBuilder, "尝试嫁接节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static bool TryGraft{ClassNameUnBranch}<N, T>(this N self, T node)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(self.TypeToCode<T>(), self);");
		}

		private static void BranchTypeKeyRemoveNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void Remove{ClassNameUnBranch}N, T>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}>
		=> NodeBranchHelper.GetBranch<{ClassFullName}>(self)?.GetNode(self.TypeToCode<T>())?.Dispose();");
		}

		private static void BranchTypeKeyRemoveAllNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			bool isGeneric = ClassFullName.Contains("<");
			ClassNameUnBranch = (isGeneric) ? ClassNameUnBranch.Replace('>', ',') : ClassNameUnBranch + "<";
			AddComment(stringBuilder, "移除分支全部节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			stringBuilder.AppendLine(@$"		public static void RemoveAll{ClassNameUnBranch}N>(this N self)
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			=> self.RemoveAllNode(self.TypeToCode<{ClassFullName}>());");
		}

		private static void BranchTypeKeyAddNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				AddComment(stringBuilder, "添加节点", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				stringBuilder.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T{genericsType}>(this N self, out T node{genericTypeParameter})
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where T : class, INode, NodeOf<N,{ClassFullName}> , AsRule<Awake{genericsTypeAngle}>
		=> NodeBranchHelper.AddNode<N, {ClassFullName}, {genericType}, T{genericsType}>(self, default({ClassFullName}), self.TypeToCode<T>(), out node{genericParameter});");
			}
		}

		#region 基类类型键值

		private static void BranchBaseTypeKeyAddNode(StringBuilder stringBuilder, string ClassFullNameAndNameSpace, string ClassFullName, string BaseFullName, string BaseTypePara, string genericType, bool isUnConstraint)
		{
			string ClassNameUnBranch = ClassFullName.Replace("Branch", "");
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];

				AddComment(stringBuilder, "添加节点，以基类为键", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
				stringBuilder.AppendLine(@$"		public static T Add{ClassNameUnBranch}<N, T, SubT{genericsType}>(this N self,T defaultBaseT, out SubT node{genericTypeParameter})
			where N : class, {(isUnConstraint ? "INode" : $"INode, AsBranch<{ClassFullName}>")}
			where SubT : class, INode, T, NodeOf<N,{ClassFullName}> , AsRule<Awake{genericsTypeAngle}>
		=> NodeBranchHelper.AddNode<N, {ClassFullName}, {genericType}, SubT{genericsType}>(self, default({ClassFullName}), self.TypeToCode<T>(), out node{genericParameter});");
			}
		}
		#endregion

		#endregion

		#endregion


	}

}
