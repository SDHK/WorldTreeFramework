/****************************************

* 作者：闪电黑客
* 日期：2024/5/15 19:54

* 描述：

*/
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class CallRuleSupplementHelper
	{
		public static void GetDelegate(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			string TypeArguments = RuleSupplementHelper.GetTypeArguments(typeSymbol);

			string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface, false);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t");

			RuleSupplementHelper.AddComment(Code, "调用法则委托", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public delegate {outType} On{ClassName}<N{TypeArguments}>(N self{genericTypeParameter}) where N : class, INode, AsRule<{ClassFullName}> {WhereTypeArguments};");
		}

		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;

			string ClassName = typeSymbol.Name;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			string TypeArgumentsAngle = RuleSupplementHelper.GetTypeArgumentsAngle(typeSymbol);
			string genericParameter = GetCallRuleGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I');
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			RuleSupplementHelper.AddComment(Code, "执行调用法则", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			//生成调用方法
			Code.AppendLine(@$"		public static {outType} {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => NodeRuleHelper.{BaseName}(self, default({ClassFullName}){genericParameter});");
		}


		/// <summary>
		/// 获取泛型参数 , T1 arg1, T2 arg2 ,out T3 defaultOutT = default
		/// </summary>
		public static string GetCallRuleGenericTypesParameters(INamedTypeSymbol typeSymbol, bool isOutT = true)
		{
			if (!typeSymbol.IsGenericType) return "";

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < typeSymbol.TypeArguments.Length - 1; i++)
			{
				sb.Append($", {typeSymbol.TypeArguments[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} arg{i + 1}");
			}
			if (isOutT) sb.Append($", {typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} defaultOutT = default");
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
			string outType = typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			//sb.Append($", out {outType} _");
			sb.Append($", out defaultOutT");
			return sb.ToString();
		}

	}
}