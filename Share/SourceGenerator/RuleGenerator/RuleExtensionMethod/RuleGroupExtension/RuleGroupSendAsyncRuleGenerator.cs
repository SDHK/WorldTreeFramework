/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 10:21

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupSendAsyncRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则集合异步执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupSendAsyncRule");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 尝试通知法则集合异步执行
		/// </summary>
		public static async TreeTask<bool> TrySendAsync<R{generics}>(this IRuleGroup<R> group, INode node{genericTypeParameter})
			where R : ISendRuleAsync{genericsAngle}
		{{
			if (((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList))
			{{
				await ((IRuleList<R>)ruleList).SendAsync(node{genericParameter});
				return true;
			}}
			await node.TreeTaskCompleted();
			return false;
		}}

		/// <summary>
		/// 通知法则集合异步执行
		/// </summary>
		public static async TreeTask SendAsync<R{generics}>(this IRuleGroup<R> group, INode node{genericTypeParameter})
			where R : ISendRuleAsync{genericsAngle}
		{{
			if (!((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList)) await node.TreeTaskCompleted();
			await ((IRuleList<R>)ruleList).SendAsync(node{genericParameter});
		}}
");
			}

			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupSendAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}