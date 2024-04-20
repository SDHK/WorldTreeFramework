/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 11:12

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeSendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class NodeSendRule");
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
		/// 尝试执行通知法则
		/// </summary>
		public static bool TrySendRule<R{generics}>(this INode self, R nullRule{(i==0?" = null":genericTypeParameter)})
			where R : class, ISendRuleBase{genericsAngle}
		{{
			if (!self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList)) return false;
			ruleList.Send(self{genericParameter});
			return true;
		}}

		/// <summary>
		/// 执行通知法则
		/// </summary>
		public static void SendRule<N, R{generics}>(this N self, R nullRule{genericTypeParameter})
			where N : class, INode, AsRule<R>
			where R : class, ISendRuleBase{genericsAngle}
		=> self.TrySendRule(nullRule{genericParameter});
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}