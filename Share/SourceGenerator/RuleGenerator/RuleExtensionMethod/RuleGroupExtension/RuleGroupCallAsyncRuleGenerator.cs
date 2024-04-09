/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 18:29

* 描述：

*/

using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
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
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);

				Code.Append
($@"

		/// <summary>
		/// 调用法则集合异步执行
		/// </summary>
		public static async TreeTask<OutT> CallAsync<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsyncBase<{genericsAfter}OutT>
		{{
			if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
			{{
				return await ((IRuleList<R>)ruleList).CallAsync(node{genericParameter}, defaultOutT);
			}}
			await node.TreeTaskCompleted();
			return TypeInfo<OutT>.Default;
		}}
");
				Code.Append
($@"

		/// <summary>
		/// 调用法则集合异步执行
		/// </summary>
		public static async TreeTask<UnitList<OutT>> CallsAsync<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsyncBase<{genericsAfter}OutT>
		{{
			if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
			{{
				return await ((IRuleList<R>)ruleList).CallsAsync(node{genericParameter}, defaultOutT);
			}}
			await node.TreeTaskCompleted();
			return null;
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupCallAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}