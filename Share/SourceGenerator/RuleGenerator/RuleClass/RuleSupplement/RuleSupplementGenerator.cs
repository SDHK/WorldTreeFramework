/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 14:22

* 描述：补充法则类型生成器

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Security;
using System.Text;

namespace WorldTree.SourceGenerator
{
	/// <summary>
	/// 补充法则类型生成器
	/// </summary>
	[Generator]
	internal class RuleSupplementGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new FindSubInterfaceSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is FindSubInterfaceSyntaxReceiver receiver)) return;
			if (receiver.InterfaceDeclarations.Count == 0) return;

			RuleSupplementHelper.Init(context.Compilation);
			foreach (InterfaceDeclarationSyntax interfaceDeclaration in receiver.InterfaceDeclarations)
			{
				RuleSupplementHelper.Add(interfaceDeclaration, context.Compilation);
			}
			RuleSupplementHelper.Execute(context);
		}
	}

	public static class RuleSupplementHelper
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
		public static Dictionary<string, InterfaceDeclarationSyntax> classInterfaceSyntax = new();

		public static string ISendRule = "ISendRule";
		public static string ICallRule = "ICallRule";
		public static string ISendRuleAsync = "ISendRuleAsync";
		public static string ICallRuleAsync = "ICallRuleAsync";

		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			fileNamespace.Clear();
			classInterfaceSyntax.Clear();
		}

		public static void Add(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
		{
			INamedTypeSymbol? namedType = compilation.ToINamedTypeSymbol(interfaceDeclaration);
			if (namedType == null) return;

			//检测是否继承4大法则接口
			if (!(NamedSymbolHelper.CheckInterface(namedType, ISendRule, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ICallRule, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ISendRuleAsync, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ICallRuleAsync, out _))) return;

			string fileName = Path.GetFileNameWithoutExtension(interfaceDeclaration.SyntaxTree.FilePath);
			if (!fileClassDict.TryGetValue(fileName, out List<INamedTypeSymbol> set))
			{
				set = new List<INamedTypeSymbol>();
				fileClassDict.Add(fileName, set);
				fileUsings.Add(fileName, TreeSyntaxHelper.GetUsings(interfaceDeclaration));
				fileNamespace.Add(fileName, TreeSyntaxHelper.GetNamespace(interfaceDeclaration));
			}
			string interfaceName = namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			if (set.Any(a => a.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == interfaceName)) return;

			set.Add(namedType);

			classInterfaceSyntax.Add(interfaceName, interfaceDeclaration);
		}

		public static void Execute(GeneratorExecutionContext context)
		{
			var IRuleSupplementIgnore = context.Compilation.ToINamedTypeSymbol("WorldTree.IRuleSupplementIgnore");
			var IMethodRule = context.Compilation.ToINamedTypeSymbol("WorldTree.IMethodRule");

			if (IRuleSupplementIgnore == null) return;
			if (IMethodRule == null) return;

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();

				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{
					if (NamedSymbolHelper.CheckAllInterface(fileClass, IRuleSupplementIgnore)) continue;
					INamedTypeSymbol? baseInterface = null;
					bool isMethodRule = NamedSymbolHelper.CheckAllInterface(fileClass, IMethodRule);

					if (NamedSymbolHelper.CheckInterface(fileClass, ISendRule, out baseInterface))
					{
						if (isMethodRule) SendRuleSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ISendRuleAsync, out baseInterface))
					{
						if (isMethodRule) SendRuleAsyncSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRule, out baseInterface))
					{
						if (isMethodRule) CallRuleSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRuleAsync, out baseInterface))
					{
						if (isMethodRule) CallRuleAsyncSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}

					RuleClass(ClassCode, fileClass, baseInterface);
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

		/// <summary>
		/// 生成法则类
		/// </summary>
		private static void RuleClass(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullName = GetNameWithGenericArguments(typeSymbol);

			string BaseName = baseInterface.Name.TrimStart('I');
			string TypeArguments = GetTypeArguments(typeSymbol);
			string BaseTypeArguments = GetTypeArguments(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[ClassFullName]);

			StringBuilder CommentPara = new();

			//As约束接口
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "法则约束", "\t");
			string BaseTypePara = GetRuleParametersTypeCommentPara(baseInterface, "\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[ClassFullName], CommentPara.ToString(), "\t"));
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsRule<{ClassFullName}>, INode {WhereTypeArguments}{{}}");

			//抽象基类注释
			CommentPara.Clear();
			AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "法则基类", "\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[ClassFullName], CommentPara.ToString(), "\t"));

			//抽象基类
			Code.AppendLine(@$"	public abstract class {ClassName}Rule<N{TypeArguments}> : {BaseName}<N, {ClassFullName}{BaseTypeArguments}> where N : class, INode, AsRule<{ClassFullName}> {WhereTypeArguments}{{}}");
		}

		/// <summary>
		/// 获取类型名称包括泛型
		/// </summary>
		public static string GetNameWithGenericArguments(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return typeSymbol.Name;
			return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
		}

		/// <summary>
		/// 获取泛型参数 《T1, T2》
		/// </summary>
		public static string GetTypeArgumentsAngle(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";
			return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(typeSymbol.Name, "");
		}

		/// <summary>
		/// 获取泛型参数 , T1, T2
		/// </summary>
		public static string GetTypeArguments(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";
			string typeArguments = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(typeSymbol.Name, "");
			return ", " + typeArguments.Trim('<', '>');
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2
		/// </summary>
		public static string GetGenericTypeParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 添加法则继承注释
		/// </summary>
		public static void AddRuleExtendCommentPara(StringBuilder sb, INamedTypeSymbol typeSymbol, INamedTypeSymbol BaseSymbol, string Title, string tab)
		{
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string IBaseFullName = GetNameWithGenericArguments(BaseSymbol);
			sb.AppendLine(@$"{tab}/// <Para>");
			sb.AppendLine(@$"{tab}/// {Title}: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{SecurityElement.Escape(IBaseFullName)}""/>");
			sb.AppendLine(@$"{tab}/// </Para>");
		}

		/// <summary>
		/// 获取法则参数泛型类型注释
		/// <para>T1 : <see cref="float"/>, OutT : <see cref="int"/></para>
		/// </summary>
		public static string GetRuleParametersTypeCommentPara(INamedTypeSymbol BaseTypeSymbol, string tab)
		{
			if (!BaseTypeSymbol.IsGenericType) return "";
			StringBuilder sb = new StringBuilder();
			StringBuilder sbType = new StringBuilder();

			sb.AppendLine($"{tab}/// <para>");
			sb.Append($"{tab}/// {BaseTypeSymbol.Name}");
			int index = 1;
			for (int i = 0; i < BaseTypeSymbol.TypeArguments.Length; i++)
			{
				string name = BaseTypeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

				//不是泛型
				if (BaseTypeSymbol.TypeArguments[i].TypeKind != TypeKind.TypeParameter)
				{
					sbType.Append($", <see cref=\"{name}\"/>");
				}
				else //是泛型参数
				{
					sbType.Append($", <typeparamref name= \"{name}\"/>");
				}
				index++;
			}
			sb.AppendLine($"&lt;{sbType.ToString().TrimStart(',', ' ')}&gt;");
			sb.AppendLine($"{tab}/// </para>");
			return sb.ToString();
		}

		/// <summary>
		/// 获取注释，添加或插入备注
		/// </summary>
		public static string GetCommentAddOrInsertRemarks(InterfaceDeclarationSyntax interfaceDeclarationSyntax, string remarksToAdd, string tab)
		{
			var triviaList = interfaceDeclarationSyntax.GetLeadingTrivia().Where(i => i.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
			bool remarksExists = false;
			StringBuilder allComments = new StringBuilder();
			if (triviaList.Any())
			{
				StringBuilder CommentNode = new StringBuilder();
				int CommentNodeIndex = 0;
				foreach (var trivia in triviaList)
				{
					CommentNode.Clear();
					string[] triviaStrings = trivia.ToFullString().Split('\n');

					foreach (string triviaStringLine in triviaStrings)//遍历出一行注释
					{
						if (triviaStringLine == string.Empty) continue;

						string newTriviaStringLine = triviaStringLine.TrimStart('\t', ' ');
						if (triviaStringLine.Contains("</remarks>"))
						{
							remarksExists = true;
							var index = triviaStringLine.IndexOf("</remarks>");
							CommentNode.Append(tab + newTriviaStringLine.Insert(index - 1, $"\n{remarksToAdd}{tab}/// "));
						}
						else
						{
							CommentNode.Append(tab + newTriviaStringLine);
						}
					}
					CommentNodeIndex++;

					if (CommentNodeIndex != triviaList.Count())
					{
						allComments.AppendLine(CommentNode.ToString());
					}
					else
					{
						allComments.Append(CommentNode.ToString());
					}
				}
			}

			// If there is no remarks node, add one
			if (!remarksExists)
			{
				allComments.AppendLine($"{tab}/// <remarks>");
				allComments.Append($"{remarksToAdd}");
				allComments.AppendLine($"{tab}/// </remarks>");
			}

			return allComments.ToString();
		}
	}
}