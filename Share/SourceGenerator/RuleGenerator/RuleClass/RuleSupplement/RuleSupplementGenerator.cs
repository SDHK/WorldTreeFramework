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
		public static Dictionary<string, string> fileUsing = new();

		/// <summary>
		/// 接口名-语法树
		/// </summary>
		public static Dictionary<string, InterfaceDeclarationSyntax> classInterfaceSyntax = new();

		public static string ISendRuleBase = "ISendRuleBase";
		public static string ICallRuleBase = "ICallRuleBase";
		public static string ISendRuleAsyncBase = "ISendRuleAsyncBase";
		public static string ICallRuleAsyncBase = "ICallRuleAsyncBase";

		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			fileUsing.Clear();
			classInterfaceSyntax.Clear();
		}

		public static void Add(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
		{
			INamedTypeSymbol? namedType = compilation.ToINamedTypeSymbol(interfaceDeclaration);
			if (namedType == null) return;

			//检测是否继承4大法则接口
			if (!(NamedSymbolHelper.CheckInterface(namedType, ISendRuleBase, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ICallRuleBase, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ISendRuleAsyncBase, out _) ||
				NamedSymbolHelper.CheckInterface(namedType, ICallRuleAsyncBase, out _))) return;

			string fileName = Path.GetFileNameWithoutExtension(interfaceDeclaration.SyntaxTree.FilePath);
			if (!fileClassDict.TryGetValue(fileName, out List<INamedTypeSymbol> set))
			{
				set = new List<INamedTypeSymbol>();
				fileClassDict.Add(fileName, set);
				fileUsings.Add(fileName, TreeSyntaxHelper.GetUsings(interfaceDeclaration));
				fileUsing.Add(fileName, TreeSyntaxHelper.GetUsing(interfaceDeclaration));
			}
			string interfaceName = namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			if (set.Any(a => a.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == interfaceName)) return;

			set.Add(namedType);

			classInterfaceSyntax.Add(interfaceName, interfaceDeclaration);
		}

		public static void Execute(GeneratorExecutionContext context)
		{
			var ILifeCycleRule = context.Compilation.ToINamedTypeSymbol("WorldTree.ILifeCycleRule");
			var IRuleSupplementIgnore = context.Compilation.ToINamedTypeSymbol("WorldTree.IRuleSupplementIgnore");


			if (ILifeCycleRule == null) return;
			if (IRuleSupplementIgnore == null) return;

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();

				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();

				

				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{
					if (NamedSymbolHelper.CheckAllInterface(fileClass, IRuleSupplementIgnore)) continue;

						if (NamedSymbolHelper.CheckInterface(fileClass, ISendRuleBase, out INamedTypeSymbol? baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							if (!NamedSymbolHelper.CheckAllInterface(fileClass, ILifeCycleRule))
								SendRuleMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ISendRuleAsyncBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							if (!NamedSymbolHelper.CheckAllInterface(fileClass, ILifeCycleRule))
								SendRuleAsyncMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRuleBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							if (!NamedSymbolHelper.CheckAllInterface(fileClass, ILifeCycleRule))
								CallRuleMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRuleAsyncBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							if (!NamedSymbolHelper.CheckAllInterface(fileClass, ILifeCycleRule))
								CallRuleAsyncMethod(MethodCode, fileClass, baseInterface);
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

				if (ClassCode.ToString() != "") {

					fileCode.AppendLine(fileUsings[fileClassList.Key]);
					fileCode.AppendLine($"namespace {fileUsing[fileClassList.Key]}");
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
		private static void RuleClass(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName;
			string BaseName = baseInterface.Name.TrimStart('I');
			string TypeArguments = GetTypeArguments(typeSymbol);
			string BaseTypeArguments = GetTypeArguments(baseInterface);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[IClassFullName]);

			StringBuilder CommentPara = new();

			//As约束接口
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "约束接口", "\t");
			string BaseTypePara = GetBaseTypePara(baseInterface, "\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t"));
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsRule<{IClassFullName}>, INode {WhereTypeArguments}{{}}");

			//抽象基类注释
			CommentPara.Clear();
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "法则基类", "\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t"));

			//抽象基类
			Code.AppendLine(@$"	public abstract class {ClassName}Rule<N{TypeArguments}> : {BaseName}<N, {IClassFullName}{BaseTypeArguments}> where N : class, INode, AsRule<{IClassFullName}> {WhereTypeArguments}{{}}");
		}

		private static void SendRuleMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName;
			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);
			string genericTypeParameter = GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[IClassFullName]);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");

			StringBuilder CommentPara = new();
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "执行通知法则", "\t\t");
			string BaseTypePara = GetBaseTypePara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法
			Code.AppendLine(@$"		public static void {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void SendRuleAsyncMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName;
			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);
			string genericTypeParameter = GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[IClassFullName]);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			StringBuilder CommentPara = new();
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "执行异步通知法则", "\t\t");
			string BaseTypePara = GetBaseTypePara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法

			Code.AppendLine(@$"		public static TreeTask {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void CallRuleMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName;
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = GetCallRuleGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[IClassFullName]);
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			StringBuilder CommentPara = new();
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "执行调用法则", "\t\t");
			string BaseTypePara = GetBaseTypePara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法
			Code.AppendLine(@$"		public static {outType} {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void CallRuleAsyncMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName;
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = GetCallRuleAsyncGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleAsyncGenericTypesParameters(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[IClassFullName]);
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			StringBuilder CommentPara = new();
			AddCommentPara(CommentPara, typeSymbol, baseInterface, "执行异步调用法则", "\t\t");
			string BaseTypePara = GetBaseTypePara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(GetCommentAddOrInsertRemarks(classInterfaceSyntax[IClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法
			Code.AppendLine(@$"		public static TreeTask<{outType}> {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
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
		/// 获取泛型参数 , T1 arg1, T2 arg2 ,out T3 defaultOutT = default
		/// </summary>
		public static string GetCallRuleGenericTypesParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			sb.Append($", {typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} defaultOutT = default");
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , arg1, arg2 ,out defaultOutT
		/// </summary>
		public static string GetCallRuleGetGenericParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", arg{i + 1}");
			}
			sb.Append($", out defaultOutT");
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2, T3 defaultOutT = default
		/// </summary>
		public static string GetCallRuleAsyncGenericTypesParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			sb.Append($", {typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} defaultOutT = default");
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , arg1, arg2 ,out defaultOutT
		/// </summary>
		public static string GetCallRuleAsyncGetGenericParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", arg{i + 1}");
			}
			sb.Append($", defaultOutT");
			return sb.ToString();
		}

		/// <summary>
		/// 添加注释Para
		/// </summary>
		private static void AddCommentPara(StringBuilder sb, INamedTypeSymbol typeSymbol, INamedTypeSymbol BaseSymbol, string Title, string tab)
		{
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string IBaseFullName = GetNameWithGenericArguments(BaseSymbol);
			sb.AppendLine(@$"{tab}/// <Para>");
			sb.AppendLine(@$"{tab}/// {Title}: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{SecurityElement.Escape(IBaseFullName)}""/>");
			sb.AppendLine(@$"{tab}/// </Para>");
		}

		/// <summary>
		/// 获取泛型参数
		/// <para>T1 : <see cref="float"/>, OutT : <see cref="int"/></para>
		/// </summary>
		public static string GetBaseTypePara(INamedTypeSymbol BaseTypeSymbol, string tab)
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