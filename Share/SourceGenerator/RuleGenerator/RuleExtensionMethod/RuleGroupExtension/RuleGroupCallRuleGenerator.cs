/****************************************

* 作者：闪电黑客
* 日期：2024/4/9 18:01

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 尝试调用法则集合执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupCallRule");
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
		/// 尝试调用法则集合执行
		/// </summary>
		public static bool TryCall<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, out OutT outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			if (((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList))
			{{
				((IRuleList<R>)ruleList).Call(node{genericParameter}, out outT);
				return true;
			}}
			outT = TypeInfo<OutT>.Default;
			return false;
		}}

		/// <summary>
		/// 调用法则集合执行
		/// </summary>
		public static OutT Call<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, out OutT outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			group.TryCall(node{genericParameter}, out outT);
			return outT;
		}}

		/// <summary>
		/// 尝试调用法则集合执行
		/// </summary>
		public static bool TryCalls<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, out UnitList<OutT> outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
			{{
				((IRuleList<R>)ruleList).Calls(node{genericParameter}, out outT);
				return true;
			}}
			outT = null;
			return true;
		}}

		/// <summary>
		/// 调用法则集合执行
		/// </summary>
		public static UnitList<OutT> Calls<R{generics}, OutT>(this IRuleGroup<R> group, INode node{genericTypeParameter}, out UnitList<OutT> outT)
			where R : ICallRuleBase<{genericsAfter}OutT>
		{{
			group.TryCalls(node{genericParameter}, out outT);
			return outT;
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}