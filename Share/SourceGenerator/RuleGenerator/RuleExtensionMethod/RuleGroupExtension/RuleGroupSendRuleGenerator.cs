/****************************************

* 作者：闪电黑客
* 日期：2024/4/8 18:07

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleGroupSendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通知法则集合执行
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleGroupSendRule");
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
		/// 尝试通知法则集合执行
		/// </summary>
		public static bool TrySend<R{generics}>(this IRuleGroup<R> group, INode node{genericTypeParameter})
			where R : ISendRule{genericsAngle}
		{{
			if (!((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList)) return false;
			((IRuleList<R>)ruleList).Send(node{genericParameter});
			return true;
		}}

		/// <summary>
		/// 通知法则集合执行
		/// </summary>
		public static void Send<R{generics}>(this IRuleGroup<R> group, INode node{genericTypeParameter})
			where R : ISendRule{genericsAngle}
		{{
			group.TrySend(node{genericParameter});
		}}
");
			}

			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleGroupSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}