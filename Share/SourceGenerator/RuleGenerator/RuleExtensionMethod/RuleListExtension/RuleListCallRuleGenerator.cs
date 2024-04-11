/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 12:10

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleListCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则列表执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleListCallRule");
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
		/// 法则列表调用执行
		/// </summary>
		public static OutT Call<R{generics}, OutT>(this IRuleList<R> ruleList, INode node{genericTypeParameter}, out OutT outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			outT = TypeInfo<OutT>.Default;
			foreach (ICallRuleBase<{genericsAfter}OutT> rule in (RuleList)ruleList)
			{{
				rule.IsMulticast = true;
				outT = rule.Invoke(node{genericParameter});
				if (!rule.IsMulticast) return outT;
			}}
			return outT;
		}}

		/// <summary>
		/// 法则列表调用执行
		/// </summary>
		public static UnitList<OutT> Calls<R{generics}, OutT>(this IRuleList<R> ruleList, INode node{genericTypeParameter}, out UnitList<OutT> outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			outT = node.PoolGetUnit<UnitList<OutT>>();
			foreach (ICallRuleBase<{genericsAfter}OutT> rule in  (RuleList)ruleList)
			{{
				rule.IsMulticast = true;
				outT.Add(rule.Invoke(node{genericParameter}));
				if (!rule.IsMulticast) return outT;
			}}
			return outT;
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleListCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}