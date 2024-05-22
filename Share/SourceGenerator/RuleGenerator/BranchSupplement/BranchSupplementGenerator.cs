/****************************************

* 作者：闪电黑客
* 日期：2024/5/20 15:39

* 描述：分支类型补充生成器

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

		public static string Branch = "Branch";

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

			//检测是否继承分支基类
			if (namedType.BaseType?.Name != Branch) return;
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
			if (ISourceGeneratorIgnore == null) return;

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();
				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{


				}
			}

		}

		private static void BranchClass(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseClass)
		{
			if (baseClass == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classSyntax[ClassFullName]);
			StringBuilder CommentPara = new();

			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseClass, "法则约束", "\t");


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
