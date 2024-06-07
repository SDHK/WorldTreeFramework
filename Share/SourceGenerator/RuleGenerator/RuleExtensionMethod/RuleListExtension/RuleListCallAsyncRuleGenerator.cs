/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 14:56

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleListCallAsyncRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 法则列表异步调用执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListCallAsyncRule");
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
							/// 法则列表异步调用执行
							/// </summary>
							public static async TreeTask<OutT> CallAsync<R{{genericsType}}, OutT>(this IRuleList<R> ruleList, INode node{{genericTypeParameter}}, OutT defaultOutT)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								OutT outT = TypeInfo<OutT>.Default;
								foreach (ICallRuleAsync<{{genericsTypeAfter}}OutT> rule in (RuleList)ruleList)
								{
									outT = await rule.Invoke(node{{genericParameter}});
								}
								return outT;
							}

							/// <summary>
							/// 法则列表异步调用执行
							/// </summary>
							public static async TreeTask<UnitList<OutT>> CallsAsync<R{{genericsType}}, OutT>(this IRuleList<R> ruleList, INode node{{genericTypeParameter}}, OutT defaultOutT)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								UnitList<OutT> outT = node.Core.PoolGetUnit<UnitList<OutT>>();
								foreach (ICallRuleAsync<{{genericsTypeAfter}}OutT> rule in (RuleList)ruleList)
								{
									outT.Add(await rule.Invoke(node{{genericParameter}}));
								}
								return outT;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListCallAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}