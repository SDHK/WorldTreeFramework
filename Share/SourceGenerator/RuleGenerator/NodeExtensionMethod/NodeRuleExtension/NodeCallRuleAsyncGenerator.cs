/****************************************

* 作者：闪电黑客
* 日期：2024/4/10 20:08

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeCallRuleAsyncGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行异步调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class NodeCallRuleAsync");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 尝试执行异步调用法则
		/// </summary>
		public static async TreeTask<OutT> TryCallRuleAsync<R{generics}, OutT>(this INode self, R nullRule{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsync<{genericsAfter}OutT>
		{{
			if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
			{{
				return await ruleList.CallAsync(self{genericParameter}, defaultOutT);
			}}
			await self.TreeTaskCompleted();
			return TypeInfo<OutT>.Default;
		}}

		/// <summary>
		/// 执行异步调用法则
		/// </summary>
		public static async TreeTask<OutT> CallRuleAsync<N, R{generics}, OutT>(this N self, R nullRule{genericTypeParameter}, OutT defaultOutT)
			where N : class, INode, AsRule<R>
			where R : ICallRuleAsync<{genericsAfter}OutT>
		=> await self.TryCallRuleAsync(nullRule{genericParameter}, defaultOutT);

		/// <summary>
		/// 尝试执行异步调用法则
		/// </summary>
		public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R{generics}, OutT>(this INode self, R nullRule{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsync<{genericsAfter}OutT>
		{{
			if (self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList))
			{{
				return await ruleList.CallsAsync(self{genericParameter}, defaultOutT);
			}}
			await self.TreeTaskCompleted();
			return null;
		}}

		/// <summary>
		/// 执行异步调用法则
		/// </summary>
		public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R{generics}, OutT>(this N self, R nullRule{genericTypeParameter}, OutT defaultOutT)
			where N : class, INode, AsRule<R>
			where R : ICallRuleAsync<{genericsAfter}OutT>
		=> await self.TryCallsRuleAsync(nullRule{genericParameter}, defaultOutT);
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeCallRuleAsync.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}