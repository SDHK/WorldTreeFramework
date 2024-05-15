/****************************************

* 作者：闪电黑客
* 日期：2024/5/15 19:53

* 描述：

*/
using Microsoft.CodeAnalysis;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class SendRuleAsyncSupplementHelper
	{
		public static void GetMethod(StringBuilder Code, INamedTypeSymbol typeSymbol, INamedTypeSymbol? baseInterface)
		{
			if (baseInterface == null) return;

			string ClassName = typeSymbol.Name;
			string ClassFullName = RuleSupplementHelper.GetNameWithGenericArguments(typeSymbol);

			int baseTypeCount = baseInterface.TypeArguments.Count();
			string TypeArgumentsAngle = RuleSupplementHelper.GetTypeArgumentsAngle(typeSymbol);
			string genericParameter = RuleGeneratorHelper.GetGenericParameter(baseTypeCount);
			string genericTypeParameter = RuleSupplementHelper.GetGenericTypeParameter(baseInterface);
			string WhereTypeArguments = TreeSyntaxHelper.GetWhereTypeArguments(RuleSupplementHelper.classInterfaceSyntax[ClassFullName]);
			string BaseName = baseInterface.Name.TrimStart('I');
			StringBuilder CommentPara = new();
			RuleSupplementHelper.AddRuleExtendCommentPara(CommentPara, typeSymbol, baseInterface, "执行异步通知法则", "\t\t");
			string BaseTypePara = RuleSupplementHelper.GetRuleParametersTypeCommentPara(baseInterface, "\t\t");
			CommentPara.Append(BaseTypePara);
			Code.Append(RuleSupplementHelper.GetCommentAddOrInsertRemarks(RuleSupplementHelper.classInterfaceSyntax[ClassFullName], CommentPara.ToString(), "\t\t"));

			//生成调用方法
			Code.AppendLine(@$"		public static TreeTask {ClassName}{TypeArgumentsAngle}(this As{ClassFullName} self{genericTypeParameter}){WhereTypeArguments} => Node{BaseName}.{BaseName}(self, TypeInfo<{ClassFullName}>.Default{genericParameter});");
		}

	}
}