/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 11:16

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class RuleListSendRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则列表执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListSendRule");
			Code.Append("	{");
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
				Code.AppendLine(
					$$"""

							/// <summary>
							/// 法则列表通知执行
							/// </summary>
							public static void Send<R{{genericsType}}>(this IRuleList<R> iRuleList, INode node{{genericTypeParameter}})
								where R : ISendRule{{genericsTypeAngle}}
							{
								RuleList ruleList = (RuleList)iRuleList;
								for(int i = 0; i < ruleList.Count; i++)
								{
									 ((ISendRule{{genericsTypeAngle}})ruleList[i]).Invoke(node{{genericParameter}});
								}
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}