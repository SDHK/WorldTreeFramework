/****************************************

* 作者：闪电黑客
* 日期：2024/5/15 19:52

* 描述：

*/
using Microsoft.CodeAnalysis;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class SendRuleSupplementHelper
	{
		public static void GetDelegate(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string TypeArguments = RuleSupplementHelper.GetTypeArguments(typeSymbol);
			string genericTypeParameter = RuleSupplementHelper.GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t");
			RuleSupplementHelper.AddComment(Code, "通知法则委托", "\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			Code.AppendLine(@$"	public delegate void On{ClassName}<N{TypeArguments}>(N self{genericTypeParameter}) where N : class, INode, AsRule<{ClassFullName}> {WhereTypeArguments};");
		}
		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;
			string ClassName = typeSymbol.Name;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = RuleSupplementHelper.GetTypeArgumentsAngle(typeSymbol);
			string genericParameter = GeneratorTemplate.GenericsParameter[baseTypeCount];
			string genericTypeParameter = RuleSupplementHelper.GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string BaseName = baseInterface.Name.TrimStart('I');
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			RuleSupplementHelper.AddComment(Code, "执行通知法则", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			//生成调用方法
			Code.AppendLine(@$"		public static void {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => NodeRuleHelper.{BaseName}(self, default({ClassFullName}){genericParameter});");
		}


	}
}