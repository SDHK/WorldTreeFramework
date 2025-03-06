/****************************************

* 作者：闪电黑客
* 日期：2024/4/28 14:22

* 描述：法则类型补充生成器

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
	/// 法则类型补充生成器
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

			//检测是否继承基础法则接口
			if (!(NamedSymbolHelper.CheckInterfaceName(namedType, GeneratorHelper.ISendRule, out _) ||
				NamedSymbolHelper.CheckInterfaceName(namedType, GeneratorHelper.ISendRefRule, out _) ||
				NamedSymbolHelper.CheckInterfaceName(namedType, GeneratorHelper.ICallRule, out _) ||
				NamedSymbolHelper.CheckInterfaceName(namedType, GeneratorHelper.ISendRuleAsync, out _) ||
				NamedSymbolHelper.CheckInterfaceName(namedType, GeneratorHelper.ICallRuleAsync, out _))) return;

			if (NamedSymbolHelper.CheckInterface(namedType, GeneratorHelper.ISourceGeneratorIgnore, out _)) return;


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
			var IMethodRule = context.Compilation.ToINamedTypeSymbol(GeneratorHelper.IMethodRule);

			if (IMethodRule == null) return;

			foreach (var fileClassList in fileClassDict)
			{
				StringBuilder fileCode = new();
				StringBuilder ClassCode = new();
				StringBuilder MethodCode = new();

				foreach (INamedTypeSymbol fileClass in fileClassList.Value)
				{
					bool isMethodRule = NamedSymbolHelper.CheckAllInterface(fileClass, IMethodRule);

					INamedTypeSymbol? baseInterface = null;
					if (NamedSymbolHelper.CheckInterfaceName(fileClass, GeneratorHelper.ISendRule, out baseInterface))
					{
						if (isMethodRule) SendRuleSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterfaceName(fileClass, GeneratorHelper.ISendRuleAsync, out baseInterface))
					{
						if (isMethodRule) SendRuleAsyncSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterfaceName(fileClass, GeneratorHelper.ICallRule, out baseInterface))
					{
						if (isMethodRule) CallRuleSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterfaceName(fileClass, GeneratorHelper.ICallRuleAsync, out baseInterface))
					{
						if (isMethodRule) CallRuleAsyncSupplementHelper.GetMethod(MethodCode, fileClass, baseInterface);
					}
					else if (NamedSymbolHelper.CheckInterfaceName(fileClass, GeneratorHelper.ISendRefRule, out baseInterface)) { }

					RuleClass(ClassCode, fileClass, baseInterface);
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
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			string BaseName = baseInterface.Name.TrimStart('I');
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string TypeArguments = GetTypeArguments(typeSymbol);
			string BaseTypeArguments = GetTypeArguments(baseInterface);

			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(classInterfaceSyntax[ClassFullName]);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t");

			//As约束接口
			AddComment(Code, "法则约束", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public interface As{ClassFullName} : AsRule<{ClassFullName}>, INode {WhereTypeArguments}{{}}");

			//抽象基类
			AddComment(Code, "法则基类", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public abstract class {ClassName}Rule<N{TypeArguments}> : {BaseName}<N, {ClassFullName}{BaseTypeArguments}> where N : class, INode, AsRule<{ClassFullName}> {WhereTypeArguments}{{}}");

			//法则委托
			if (NamedSymbolHelper.CheckInterfaceName(typeSymbol, GeneratorHelper.ISendRule, out _))
			{
				SendRuleSupplementHelper.GetDelegate(Code, typeSymbol, baseInterface);
			}
			else if (NamedSymbolHelper.CheckInterfaceName(typeSymbol, GeneratorHelper.ISendRuleAsync, out _))
			{
				SendRuleAsyncSupplementHelper.GetDelegate(Code, typeSymbol, baseInterface);
			}
			else if (NamedSymbolHelper.CheckInterfaceName(typeSymbol, GeneratorHelper.ICallRule, out _))
			{
				CallRuleSupplementHelper.GetDelegate(Code, typeSymbol, baseInterface);
			}
			else if (NamedSymbolHelper.CheckInterfaceName(typeSymbol, GeneratorHelper.ICallRuleAsync, out _))
			{
				CallRuleAsyncSupplementHelper.GetDelegate(Code, typeSymbol, baseInterface);
			}
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
			stringBuilder.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(classInterfaceSyntax[ClassFullName], Para, tab));
		}
	}
}