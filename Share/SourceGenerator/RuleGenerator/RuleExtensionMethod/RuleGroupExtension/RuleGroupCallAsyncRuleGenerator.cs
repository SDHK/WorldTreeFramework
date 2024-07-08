/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 18:29

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupCallAsyncRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 调用法则集合异步执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupCallAsyncRule");
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
							/// 调用法则集合异步执行
							/// </summary>
							public static async TreeTask<OutT> CallAsync<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, OutT defaultOutT)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
								{
									return await ((IRuleList<R>)ruleList).CallAsync(node{{genericParameter}}, defaultOutT);
								}
								await node.TreeTaskCompleted();
								return defaultOutT;
							}

							/// <summary>
							/// 调用法则集合异步执行
							/// </summary>
							public static async TreeTask<TreeList<OutT>> CallsAsync<R{{genericsType}}, OutT>(this IRuleGroup<R> group, INode node{{genericTypeParameter}}, TreeList<OutT> outTList)
								where R : ICallRuleAsync<{{genericsTypeAfter}}OutT>
							{
								if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
								{
									return await ((IRuleList<R>)ruleList).CallsAsync(node{{genericParameter}}, outTList);
								}
								await node.TreeTaskCompleted();
								return outTList;
							}
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupCallAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}