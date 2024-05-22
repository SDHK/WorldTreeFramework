/****************************************

* 作者：闪电黑客
* 日期：2024/5/15 19:54

* 描述：

*/
using Microsoft.CodeAnalysis;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class CallRuleSupplementHelper
	{
		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;

			string ClassName = typeSymbol.Name;
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			string TypeArgumentsAngle = RuleSupplementHelper.GetTypeArgumentsAngle(typeSymbol);
			string genericParameter = GetCallRuleGetGenericParameter(baseInterface);
			string genericTypeParameter = GetCallRuleGenericTypesParameters(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string outType = baseInterface.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseName = baseInterface.Name.TrimStart('I');
			StringBuilder CommentPara = new();
			RuleSupplementHelper.AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "执行调用法则", "\t\t");
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(TreeSyntaxHelper.GetCommentAddOrInsertRemarks(RuleSupplementHelper.classInterfaceSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法
			Code.AppendLine(@$"		public static {outType} {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => NodeRuleHelper.{BaseName}(self, TypeInfo<{ClassFullName}>.Default{genericParameter});");
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
			string outType = typeSymbol.TypeArguments.Last().ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			//sb.Append($", out {outType} _");
			sb.Append($", out defaultOutT");
			return sb.ToString();
		}

	}
}