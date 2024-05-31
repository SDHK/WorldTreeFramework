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
						GetMethod(MethodCode, fileClass, baseInterface);
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
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classSyntax[ClassFullName]);
			StringBuilder CommentPara = new();

			//As约束接口
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseClass, "分支约束", "\t");
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseClass, "\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t"));
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsBranch<{ClassFullName}>, INode {WhereTypeArguments}{{}}");
		}


		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			//string ClassNameShort = typeSymbol.Name.Replace("Branch","");

			// 获取泛型参数 《T1, T2》
			string TypeArgumentsAngle = typeSymbol.IsGenericType ? typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(typeSymbol.Name, "") : "";
			// 获取泛型参数 , T1, T2
			string TypeArguments = typeSymbol.IsGenericType ? (", " + TypeArgumentsAngle.Trim('<', '>')) : "";

			string genericType = baseInterface.IsGenericType ? baseInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) : "";
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classSyntax[ClassFullName]);

			StringBuilder CommentPara = new();

			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "尝试获取分支", "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));
			Code.AppendLine(@$"		public static bool TryGet{ClassName}<BN{TypeArguments}>(this As{ClassFullName} self, {genericType} key, out BN node)
				where BN : class, INode, NodeOf<As{ClassFullName}, {ClassFullName}> {WhereTypeArguments} 
			=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(key) as BN) != null;");

			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "尝试裁剪节点", "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));
			Code.AppendLine(@$"		public static bool TryCut{ClassName}<BN{TypeArguments}>(this As{ClassFullName} self, {genericType} key, out BN node)
				where BN : class, INode, NodeOf<As{ClassFullName}, ComponentBranch> {WhereTypeArguments} 
			=> (node = self.GetBranch<{ClassFullName}>()?.GetNode(key)?.CutSelf() as BN) != null;");

			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "尝试嫁接节点", "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));
			Code.AppendLine(@$"		public static bool TryGraft{ClassName}<BN{TypeArguments}>(this As{ClassFullName} self, {genericType} key, BN node)
				where BN : class, INode, NodeOf<As{ClassFullName}, {ClassFullName}> {WhereTypeArguments}
			=> node.TryGraftSelfToTree<{ClassFullName},{genericType}>(key, self);");

			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "移除分支节点", "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));
			Code.AppendLine(@$"		public static void Remove{ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self, {genericType} key){WhereTypeArguments} 
			=> self.GetBranch<{ClassFullName}>()?.GetNode(key)?.Dispose();");

			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "移除分支全部节点", "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));
			Code.AppendLine(@$"		public static void RemoveAll{ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self){WhereTypeArguments} 
			=> self.RemoveAllNode(TypeInfo<{ClassFullName}>.TypeCode);");

		}


		/// <summary>
		/// 添加法则继承注释
		/// </summary>
		public static void AddRuleExtendCommentPara(StringBuilder sb, INamedTypeSymbol typeSymbol, INamedTypeSymbol BaseSymbol, string Title, string tab)
		{
			string IClassFullName = typeSymbol.ToDisplayString();
			string IBaseFullName = BaseSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			sb.AppendLine(@$"{tab}/// <Para>");
			sb.AppendLine(@$"{tab}/// {Title}: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{SecurityElement.Escape(IBaseFullName)}""/>");
			sb.AppendLine(@$"{tab}/// </Para>");
		}



	}

}
