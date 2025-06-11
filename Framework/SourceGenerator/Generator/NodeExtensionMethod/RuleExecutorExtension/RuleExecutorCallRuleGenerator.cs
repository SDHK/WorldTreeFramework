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
	internal class RuleExecutorCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = GeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 执行器执行调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleExecutorCallRule");
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
							public static OutT Call<R{{genericsType}}, OutT>(this IRuleExecutor<R> selfExecutor{{genericTypeParameter}}, out OutT outT)
								where R : ICallRule<{{genericsTypeAfter}}OutT>
							{
								outT = default;
								IRuleExecutorEnumerable self = (IRuleExecutorEnumerable)selfExecutor;
								if (!self.IsActive) return outT;
								self.RefreshTraversalCount();
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out INode node, out RuleList ruleList))
									{
										((IRuleList<R>)ruleList).Call(node{{genericParameter}}, out outT);
									}
								}
								return outT;
							}

							/// <summary>
							/// 执行器执行异步调用法则
							/// </summary>
							public static async TreeTask<OutT> CallAsync<R{{genericsType}}, OutT>(this IRuleExecutor<R> selfExecutor{{genericTypeParameter}}, OutT defaultOutT)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								IRuleExecutorEnumerable self = (IRuleExecutorEnumerable)selfExecutor;
								if (!self.IsActive) 
								{
									await self.TreeTaskCompleted(); 
									return defaultOutT;
								}
								self.RefreshTraversalCount();
								if (self.TraversalCount == 0) 
								{
									await self.TreeTaskCompleted(); 
									return defaultOutT;
								}
								for (int i = 0; i < self.TraversalCount; i++)
								{
									if (self.TryDequeue(out INode node, out RuleList ruleList))
									{				
					
										defaultOutT = await ((IRuleList<R>)ruleList).CallAsync(node{{genericParameter}}, defaultOutT);
									}
									else
									{
										await self.TreeTaskCompleted();
										return defaultOutT;
									}
								}
								return defaultOutT;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleExecutorCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}

	}
}
