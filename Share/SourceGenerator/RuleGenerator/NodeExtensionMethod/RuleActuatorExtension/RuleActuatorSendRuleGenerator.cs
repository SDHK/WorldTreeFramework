/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 19:45

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleActuatorSendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 执行器执行通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleActuatorSendRule");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAngle = GeneratorTemplate.GenericsTypesAngle[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
				Code.Append(
					$$"""

							/// <summary>
							/// 执行器执行通知法则
							/// </summary>
							public static void Send<R{{genericsType}}>(this IRuleActuator<R> Self{{genericTypeParameter}})
								where R : ISendRule{{genericsTypeAngle}}
							{
								if (!Self.IsActive) return;
								IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
								self.RefreshTraversalCount();
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out var nodeRuleTuple))
									{
										((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1{{genericParameter}});
									}
								}
							}

							/// <summary>
							/// 执行器执行异步通知法则
							/// </summary>
							public static async TreeTask SendAsync<R{{genericsType}}>(this IRuleActuator<R> Self{{genericTypeParameter}})
								where R : ISendRuleAsync{{genericsTypeAngle}}
							{
								if (!Self.IsActive)
								{
									await Self.TreeTaskCompleted();
									return;
								}
								IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
								self.RefreshTraversalCount();
								if (self.TraversalCount == 0)
								{
									await Self.TreeTaskCompleted();
									return;			
								}
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out var nodeRuleTuple))
									{
										await ((IRuleList<R>)nodeRuleTuple.Item2).SendAsync(nodeRuleTuple.Item1{{genericParameter}});
									}
									else
									{
										await Self.TreeTaskCompleted();
									}
								}
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleActuatorSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}