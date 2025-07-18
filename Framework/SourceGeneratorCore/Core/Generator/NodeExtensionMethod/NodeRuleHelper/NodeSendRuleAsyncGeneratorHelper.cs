﻿/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 11:25

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal static class NodeSendRuleAsyncGeneratorHelper
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = ProjectGeneratorSetting.ArgumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行异步通知法则
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
						/// 尝试执行异步通知法则
						/// </summary>
						public static async TreeTask<bool> TrySendRuleAsync<R{{genericsType}}>(INode self, R nullRule{{(i == 0 ? " = null" : string.Empty)}}{{genericTypeParameter}})
							where R : class, ISendRuleAsync{{genericsTypeAngle}}
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								await ruleList.SendAsync(self{{genericParameter}});
								return true;
							}
							await self.TreeTaskCompleted();
							return false;
						}

						/// <summary>
						/// 执行异步通知法则
						/// </summary>
						public static async TreeTask SendRuleAsync<N, R{{genericsType}}>(N self, R nullRule{{genericTypeParameter}})
							where N : class, INode, AsRule<R>
							where R : class, ISendRuleAsync{{genericsTypeAngle}}
						{
							if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
							{
								await ruleList.SendAsync(self{{genericParameter}});
							}
							await self.TreeTaskCompleted();
						}
				""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeRuleHelperSendRuleAsync.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}