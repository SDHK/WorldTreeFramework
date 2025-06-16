/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 12:00

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	public static class RuleListSendAsyncRuleGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 法则列表异步通知执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListSendAsyncRule");
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
							/// 法则列表异步通知执行
							/// </summary>
							public static async TreeTask SendAsync<R{{genericsType}}>(this IRuleList<R> iRuleList, INode node{{genericTypeParameter}})
								where R : ISendRuleAsync{{genericsTypeAngle}}
							{
								RuleList ruleList = (RuleList)iRuleList;
								for(int i = 0; i < ruleList.Count; i++)
								{
									await ((ISendRuleAsync{{genericsTypeAngle}})ruleList[i]).Invoke(node{{genericParameter}});
								}
							}
					""");
			}

			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListSendAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}