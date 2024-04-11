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
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"

		/// <summary>
		/// 法则列表异步调用执行
		/// </summary>
		public static async TreeTask<OutT> CallAsync<R{generics}, OutT>(this IRuleList<R> ruleList, INode node{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsyncBase<{genericsAfter}OutT>
		{{
			OutT outT = TypeInfo<OutT>.Default;
			foreach (ICallRuleAsyncBase<{genericsAfter}OutT> rule in (RuleList)ruleList)
			{{
				rule.IsMulticast = true;
				outT = await rule.Invoke(node{genericParameter});
				if (!rule.IsMulticast) return outT;
			}}
			return outT;
		}}

		/// <summary>
		/// 法则列表异步调用执行
		/// </summary>
		public static async TreeTask<UnitList<OutT>> CallsAsync<R{generics}, OutT>(this IRuleList<R> ruleList, INode node{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsyncBase<{genericsAfter}OutT>
		{{
			UnitList<OutT> outT = node.PoolGetUnit<UnitList<OutT>>();
			foreach (ICallRuleAsyncBase<{genericsAfter}OutT> rule in (RuleList)ruleList)
			{{
				rule.IsMulticast = true;
				outT.Add(await rule.Invoke(node{genericParameter}));
				if (!rule.IsMulticast) return outT;
			}}
			return outT;
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListCallAsyncRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}