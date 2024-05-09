/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 14:22

* 描述：补充法则类型生成器

*/

using Microsoft.CodeAnalysis;
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
		/// 接口名-where约束
		/// </summary>
		public static Dictionary<string, string> classWheres = new();

		/// <summary>
		/// 接口名-注释
		/// </summary>
		public static Dictionary<string, string> classSummary = new();

		public static string ISendRuleBase = "ISendRuleBase";
		public static string ICallRuleBase = "ICallRuleBase";
		public static string ISendRuleAsyncBase = "ISendRuleAsyncBase";
		public static string ICallRuleAsyncBase = "ICallRuleAsyncBase";

		public static void Init(Compilation compilation)
		{
			fileClassDict.Clear();
			fileUsings.Clear();
			classWheres.Clear();
			classSummary.Clear();
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
			}

			if (set.Any(a => a.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))) return;
			set.Add(namedType);
			classSummary.Add(namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), GetInterfaceSummary(interfaceDeclaration));
			classWheres.Add(namedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), TreeSyntaxHelper.GetWhereTypeArguments(interfaceDeclaration));
		}

		public static void Execute(GeneratorExecutionContext context)
		{
			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();

				ClassCode.AppendLine(fileUsings[fileClassList.Key]);
				ClassCode.AppendLine("namespace WorldTree");
				ClassCode.AppendLine("{");

				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{
					if (NamedSymbolHelper.CheckInterface(fileClass, ISendRuleBase, out INamedTypeSymbol? baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							SendRuleMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ISendRuleAsyncBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);
							SendRuleAsyncMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRuleBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);

							CallRuleMethod(MethodCode, fileClass, baseInterface);
						}
					}
					else if (NamedSymbolHelper.CheckInterface(fileClass, ICallRuleAsyncBase, out baseInterface))
					{
						if (baseInterface != null)
						{
							RuleClass(ClassCode, fileClass, baseInterface);

							CallRuleAsyncMethod(MethodCode, fileClass, baseInterface);
						}
					}
				}

				ClassCode.AppendLine($"	public static class {fileClassList.Key}Supplement");
				ClassCode.AppendLine("	{");

				ClassCode.AppendLine($"{MethodCode}");

				ClassCode.AppendLine("	}");
				ClassCode.Append("}");

				context.AddSource($"{fileClassList.Key}Supplement.cs", SourceText.From(ClassCode.ToString(), Encoding.UTF8));
			}
		}

		/// <summary>
		/// 生成法则类
		/// </summary>
		private static void RuleClass(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			string BaseName = baseInterface.Name.TrimStart('I');

			string TypeArguments = GetTypeArguments(typeSymbol);
			string BaseTypeArguments = GetTypeArguments(baseInterface);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string WhereTypeArguments = classWheres[IClassFullName];
			string IBaseFullName = SecurityElement.Escape(GetNameWithGenericArguments(baseInterface));
			string BaseTypeArgumentsAngle = SecurityElement.Escape(GetTypeArgumentsAngle(baseInterface));

			//As约束接口
			Code.AppendLine(@$"	/// <summary>");
			Code.AppendLine(@$"	/// 约束接口: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"	/// </summary>");
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsRule<{IClassFullName}>, INode {WhereTypeArguments}{{}}");

			//抽象基类注释
			Code.AppendLine(@$"	/// <summary>");
			Code.AppendLine(@$"	/// 法则基类: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"	/// </summary>");

			//抽象基类
			Code.AppendLine(@$"	public abstract class {ClassName}<N{TypeArguments}> : {BaseName}<N, {IClassFullName}{BaseTypeArguments}> where N : class, INode, AsRule<{IClassFullName}> {WhereTypeArguments}{{}}");
		}

		private static void SendRuleMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);
			string genericTypeParameter = GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = classWheres[IClassFullName];
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			string IBaseFullName = SecurityElement.Escape(GetNameWithGenericArguments(baseInterface));
			string BaseTypeArgumentsAngle = SecurityElement.Escape(GetTypeArgumentsAngle(baseInterface));

			//生成调用方法
			Code.AppendLine(@$"		/// <summary>");
			Code.AppendLine(@$"		/// 执行通知法则: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"		/// </summary>");
			Code.AppendLine(@$"		public static void {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void SendRuleAsyncMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);
			string genericTypeParameter = GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = classWheres[IClassFullName];
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			string IBaseFullName = SecurityElement.Escape(GetNameWithGenericArguments(baseInterface));
			string BaseTypeArgumentsAngle = SecurityElement.Escape(GetTypeArgumentsAngle(baseInterface));

			//生成调用方法
			Code.AppendLine(@$"		/// <summary>");
			Code.AppendLine(@$"		/// 执行异步通知法则: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"		/// </summary>");
			Code.AppendLine(@$"		public static TreeTask {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void CallRuleMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = GetCallRuleGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
			string WhereTypeArguments = classWheres[IClassFullName];
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			string IBaseFullName = SecurityElement.Escape(GetNameWithGenericArguments(baseInterface));
			string BaseTypeArgumentsAngle = SecurityElement.Escape(GetTypeArgumentsAngle(baseInterface));

			//生成调用方法
			Code.AppendLine(@$"		/// <summary>");
			Code.AppendLine(@$"		/// 执行调用法则: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"		/// </summary>");
			Code.AppendLine(@$"		public static {outType} {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{IClassFullName}>.Default{genericParameter});");
		}

		private static void CallRuleAsyncMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol baseInterface)
		{
			string IClassName = typeSymbol.Name;
			string ClassName = IClassName.TrimStart('I');
			string TypeArgumentsAngle = GetTypeArgumentsAngle(typeSymbol);
			string IClassFullName = GetNameWithGenericArguments(typeSymbol);
			string ClassFullName = IClassFullName.TrimStart('I');
			string genericParameter = GetCallRuleAsyncGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleAsyncGenericTypesParameters(baseInterface);
			string WhereTypeArguments = classWheres[IClassFullName];
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I').Replace("Base", "");
			string IBaseFullName = SecurityElement.Escape(GetNameWithGenericArguments(baseInterface));
			string BaseTypeArgumentsAngle = SecurityElement.Escape(GetTypeArgumentsAngle(baseInterface));

			//生成调用方法
			Code.AppendLine(@$"		/// <summary>");
			Code.AppendLine(@$"		/// 执行异步调用法则: <see cref=""{SecurityElement.Escape(IClassFullName)}""/> : <see cref=""{IBaseFullName}""/> {BaseTypeArgumentsAngle} {classSummary[IClassFullName]}");
			Code.AppendLine(@$"		/// </summary>");
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
		/// 获取泛型参数 , T1 arg1, T2 arg2 ,out T3 outT
		/// </summary>
		public static string GetCallRuleGenericTypesParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			sb.Append($", out {typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} outT");
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , arg1, arg2 ,out outT
		/// </summary>
		public static string GetCallRuleGetGenericParameter(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", arg{i + 1}");
			}
			sb.Append($", out outT");
			return sb.ToString();
		}

		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2, T3 defaultOutT
		/// </summary>
		public static string GetCallRuleAsyncGenericTypesParameters(INamedTypeSymbol typeSymbol)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			sb.Append($", {typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} defaultOutT");
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
		/// 获取接口的声明注释
		/// </summary>
		public static string GetInterfaceSummary(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
		{
			//获取注释
			var summaryTrivia = interfaceDeclarationSyntax.GetLeadingTrivia()
			 .Select(i => i.GetStructure())
			 .OfType<DocumentationCommentTriviaSyntax>()
			 .FirstOrDefault();

			if (summaryTrivia == null) return string.Empty;

			//获取summary节点
			var summaryNode = summaryTrivia.ChildNodes().OfType<XmlElementSyntax>()
				.FirstOrDefault(i => i.StartTag.Name.ToString() == "summary");

			//获取summary节点的内容
			if (summaryNode == null) return string.Empty;
			string summaryNodeContent = summaryNode.Content.ToString();

			//去除后换行
			summaryNodeContent = summaryNodeContent.Trim('\r', '\n', ' ', '\t', '/');
			return summaryNodeContent;
		}
	}
}