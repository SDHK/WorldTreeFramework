/****************************************

* 作者：闪电黑客
* 日期：2024/7/25 20:38

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Reflection.Metadata;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleListSendRefRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = GeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则列表执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListSendRefRule");
			Code.Append("	{");
			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericRefParameter = GeneratorTemplate.GenericsRefParameter[i];
				string genericRefTypeParameter = GeneratorTemplate.GenericsRefTypeParameter[i];
				Code.AppendLine(
					$$"""

							/// <summary>
							/// 法则列表通知执行
							/// </summary>
							public static void SendRef<R{{genericsType}}>(this IRuleList<R> iRuleList, INode node{{genericRefTypeParameter}})
								where R : ISendRefRule{{genericsTypeAngle}}
							{
								RuleList ruleList = (RuleList)iRuleList;
								for(int i = 0; i < ruleList.Count; i++)
								{
									 ((ISendRefRule{{genericsTypeAngle}})ruleList[i]).Invoke(node{{genericRefParameter}});
								}
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListSendRefRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}

	}
}