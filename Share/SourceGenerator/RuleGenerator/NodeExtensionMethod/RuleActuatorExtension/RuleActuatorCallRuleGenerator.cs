/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 10:43

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleActuatorCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 执行器执行调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleActuatorCallRule");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAfter = GeneratorTemplate.GenericsTypesAfter[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
			
				Code.AppendLine(
					$$"""
							/// <summary>
							/// 执行器执行调用法则
							/// </summary>
							public static OutT Call<R{{genericsType}}, OutT>(this IRuleActuator<R> Self{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								outT = default;
								if (!Self.IsActive) return outT;
								IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
								self.RefreshTraversalCount();
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out var nodeRuleTuple))
									{
										((IRuleList<R>)nodeRuleTuple.Item2).Call(nodeRuleTuple.Item1{{genericParameter}}, out outT);
									}
								}
								return outT;
							}

							/// <summary>
							/// 执行器执行异步调用法则
							/// </summary>
							public static async TreeTask<OutT> CallAsync<R{{genericsType}}, OutT>(this IRuleActuator<R> Self{{genericTypeParameter}}, OutT defaultOutT)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								if (!Self.IsActive) 
								{
									await Self.TreeTaskCompleted(); 
									return defaultOutT;
								}
								IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
								self.RefreshTraversalCount();
								if (self.TraversalCount == 0) 
								{
									await Self.TreeTaskCompleted(); 
									return defaultOutT;
								}
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out var nodeRuleTuple))
									{
										defaultOutT = await ((IRuleList<R>)nodeRuleTuple.Item2).CallAsync(nodeRuleTuple.Item1{{genericParameter}}, defaultOutT);
									}
									else
									{
										await Self.TreeTaskCompleted();
										return defaultOutT;
									}
								}
								return defaultOutT;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleActuatorCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}

	}
}
