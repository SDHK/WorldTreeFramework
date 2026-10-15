/****************************************

* 作者：闪电黑客
* 日期：2024/5/15 19:53

* 描述：

*/
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class SendRuleAsyncSupplementHelper
	{
		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;

			string ClassName = typeSymbol.Name;
			string ClassFullNameAndNameSpace = typeSymbol.ToDisplayString();
			string ClassFullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string BaseFullName = baseInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			int baseTypeCount = baseInterface.TypeArguments.Count();
			//string TypeArgumentsAngle = RuleSupplementHelper.GetTypeArgumentsAngle(typeSymbol);
			string TypeArguments = RuleSupplementHelper.GetTypeArguments(typeSymbol);
			string genericParameter = GeneratorTemplate.GenericsParameter[baseTypeCount];
			string genericTypeParameter = RuleSupplementHelper.GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string BaseName = baseInterface.Name.TrimStart('I');
			string BaseTypePara = NamedSymbolHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");

			RuleSupplementHelper.AddComment(Code, "执行异步通知法则", "\t\t", ClassFullNameAndNameSpace, ClassFullName, BaseFullName, BaseTypePara);
			//生成调用方法
			Code.AppendLine(@$"		public static TreeTask {ClassName}<N{TypeArguments}>(this N self{genericTypeParameter})where N : class, INode, AsRule<{ClassFullName}> {WhereTypeArguments} => NodeRuleHelper.{BaseName}(self, default({ClassFullName}){genericParameter});");
		}
	}
}