/****************************************

* 作者：闪电黑客
* 日期：2024/4/15 19:45

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleActuatorSendRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 执行器执行通知法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleActuatorSendRule");
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
		/// 执行器执行通知法则
		/// </summary>
		public static void Send<R{generics}>(this IRuleActuator<R> Self{genericTypeParameter})
			where R : ISendRule{genericsAngle}
		{{
			if (!Self.IsActive) return;
			IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
			foreach ((INode, RuleList) nodeRuleTuple in self)
			{{
				((IRuleList<R>)nodeRuleTuple.Item2).Send(nodeRuleTuple.Item1{genericParameter});
			}}
		}}

		/// <summary>
		/// 执行器执行异步通知法则
		/// </summary>
		public static async TreeTask SendAsync<R{generics}>(this IRuleActuator<R> Self{genericTypeParameter})
			where R : ISendRuleAsync{genericsAngle}
		{{
			IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
			if (!Self.IsActive || self.GetEnumerator().Current == default) await Self.TreeTaskCompleted();
			foreach ((INode, RuleList) nodeRuleTuple in self)
			{{
				await ((IRuleList<R>)nodeRuleTuple.Item2).SendAsync(nodeRuleTuple.Item1{genericParameter});
			}}
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleActuatorSendRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}