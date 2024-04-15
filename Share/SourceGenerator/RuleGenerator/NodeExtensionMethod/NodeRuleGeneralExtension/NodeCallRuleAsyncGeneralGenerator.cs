/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 16:26

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeCallRuleAsyncGeneralGenerator
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
			Code.AppendLine("	public static class NodeCallRuleAsyncGeneral");
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
		public static TreeTask<OutT> TryCallAsync<{genericsAfter}OutT>(this INode self{genericTypeParameter}, OutT defaultOutT)
			=> self.TryCallRuleAsync(TypeInfo<ICallRuleAsync<{genericsAfter}OutT>>.Default{genericParameter}, defaultOutT);

		/// <summary>
		/// 执行异步调用法则
		/// </summary>
		public static TreeTask<OutT> CallAsync<N{generics}, OutT>(this N self{genericTypeParameter}, OutT defaultOutT)
			where N : class, INode, AsRule<ICallRuleAsync<{genericsAfter}OutT>>
		=> self.CallRuleAsync(TypeInfo<ICallRuleAsync<{genericsAfter}OutT>>.Default{genericParameter}, defaultOutT);

		/// <summary>
		/// 尝试执行异步调用法则
		/// </summary>
		public static TreeTask<UnitList<OutT>> TryCallsAsync<{genericsAfter}OutT>(this INode self{genericTypeParameter}, OutT defaultOutT)
			=> self.TryCallsRuleAsync(TypeInfo<ICallRuleAsync<{genericsAfter}OutT>>.Default{genericParameter}, defaultOutT);

		/// <summary>
		/// 执行异步调用法则
		/// </summary>
		public static TreeTask<UnitList<OutT>> CallsAsync<N{generics}, OutT>(this N self{genericTypeParameter}, OutT defaultOutT)
			where N : class, INode, AsRule<ICallRuleAsync<{genericsAfter}OutT>>
		=> self.CallsRuleAsync(TypeInfo<ICallRuleAsync<{genericsAfter}OutT>>.Default{genericParameter}, defaultOutT);
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeCallRuleAsyncGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}