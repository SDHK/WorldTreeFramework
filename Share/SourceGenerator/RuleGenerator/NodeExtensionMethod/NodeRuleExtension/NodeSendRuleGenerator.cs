/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 11:12

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeSendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static partial class NodeRuleHelper");
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
						/// 尝试执行通知法则
						/// </summary>
						public static bool TrySendRule<R{{genericsType}}>(INode self, R nullRule{{(i == 0 ? " = null" : genericTypeParameter)}})
							where R : class, ISendRule{{genericsTypeAngle}}
						{
							if (!self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList)) return false;
							ruleList.Send(self{{genericParameter}});
							return true;
						}

						/// <summary>
						/// 执行通知法则
						/// </summary>
						public static void SendRule<N, R{{genericsType}}>(N self, R nullRule{{genericTypeParameter}})
							where N : class, INode, AsRule<R>
							where R : class, ISendRule{{genericsTypeAngle}}
						{
							if (!self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList)) return;
							ruleList.Send(self{{genericParameter}});
						}
				""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeRuleHelperSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}