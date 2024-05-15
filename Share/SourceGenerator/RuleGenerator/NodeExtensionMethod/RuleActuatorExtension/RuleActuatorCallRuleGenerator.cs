/****************************************

* 作者：闪电黑客
* 日期：2024/4/16 10:43

* 描述：

*/
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class RuleActuatorCallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 执行器执行调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class RuleActuatorCallRule");
			Code.Append("	{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i,true);
				string genericsAngle = RuleGeneratorHelper.GetGenericsAngle(i);
				string genericParameter = RuleGeneratorHelper.GetGenericParameter(i);
				string genericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				Code.Append
($@"
		/// <summary>
		/// 执行器执行调用法则
		/// </summary>
		public static OutT Call<R{generics}, OutT>(this IRuleActuator<R> Self{genericTypeParameter}, out OutT outT)
			where R : ICallRule<{genericsAfter}OutT>
		{{
			outT = default;
			if (!Self.IsActive) return outT;
			IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
			foreach ((INode, RuleList) nodeRuleTuple in self)
			{{
				((IRuleList<R>)nodeRuleTuple.Item2).Call(nodeRuleTuple.Item1{genericParameter}, out outT);
			}}
			return outT;
		}}

		/// <summary>
		/// 执行器执行异步调用法则
		/// </summary>
		public static async TreeTask<OutT> CallAsync<R{generics}, OutT>(this IRuleActuator<R> Self{genericTypeParameter}, OutT defaultOutT)
			where R : ICallRuleAsync<{genericsAfter}OutT>
		{{
			IRuleActuatorEnumerable self = (IRuleActuatorEnumerable)Self;
			if (!Self.IsActive || self.GetEnumerator().Current == default)
			{{
				await Self.TreeTaskCompleted();
				return defaultOutT;
			}}
			foreach ((INode, RuleList) nodeRuleTuple in self)
			{{
				defaultOutT = await ((IRuleList<R>)nodeRuleTuple.Item2).CallAsync(nodeRuleTuple.Item1{genericParameter}, defaultOutT);
			}}
			return defaultOutT;
		}}
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("RuleActuatorCallRule.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}

	}
}
