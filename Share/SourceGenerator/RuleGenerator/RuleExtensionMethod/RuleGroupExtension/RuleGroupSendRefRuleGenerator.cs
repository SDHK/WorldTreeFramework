/****************************************

* 作者：闪电黑客
* 日期：2024/7/25 20:42

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupSendRefRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则集合执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupSendRefRule");
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
							/// 尝试通知法则集合执行
							/// </summary>
							public static bool TrySendRef<R{{genericsType}}>(this IRuleGroup<R> group, INode node{{genericRefTypeParameter}})
								where R : ISendRefRule{{genericsTypeAngle}}
							{
								if (!((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList)) return false;
								((IRuleList<R>)ruleList).SendRef(node{{genericRefParameter}});
								return true;
							}

							/// <summary>
							/// 通知法则集合执行
							/// </summary>
							public static void SendRef<R{{genericsType}}>(this IRuleGroup<R> group, INode node{{genericRefTypeParameter}})
								where R : ISendRefRule{{genericsTypeAngle}}
							{
								group.TrySendRef(node{{genericRefParameter}});
							}
					""");
			}

			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupSendRefRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}