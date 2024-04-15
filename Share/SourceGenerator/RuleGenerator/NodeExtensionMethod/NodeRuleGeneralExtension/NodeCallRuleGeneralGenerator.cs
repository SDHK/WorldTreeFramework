/****************************************

* 作者：闪电黑客
* 日期：2024/4/11 19:49

* 描述：

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class NodeCallRuleGeneralGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 节点执行调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.AppendLine("{");
			Code.AppendLine("	public static class NodeCallRuleGeneral");
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
		/// 尝试执行调用法则
		/// </summary>
		public static bool TryCall<{genericsAfter}OutT>(this INode self{genericTypeParameter}, out OutT outT)
			=> self.TryCallRule(TypeInfo<ICallRule<{genericsAfter}OutT>>.Default{genericParameter}, out outT);

		/// <summary>
		/// 执行调用法则
		/// </summary>
		public static OutT Call<N{generics}, OutT>(this N self{genericTypeParameter}, out OutT outT)
			where N : class, INode, AsRule<ICallRule<{genericsAfter}OutT>>
		=> self.CallRule(TypeInfo<ICallRule<{genericsAfter}OutT>>.Default{genericParameter}, out outT);

		/// <summary>
		/// 尝试执行调用法则
		/// </summary>
		public static bool TryCalls<{genericsAfter}OutT>(this INode self{genericTypeParameter}, out UnitList<OutT> outT)
			=> self.TryCallsRule(TypeInfo<ICallRule<{genericsAfter}OutT>>.Default{genericParameter}, out outT);

		/// <summary>
		/// 执行调用法则
		/// </summary>
		public static UnitList<OutT> Calls<N{generics}, OutT>(this N self{genericTypeParameter}, out UnitList<OutT> outT)
			where N : class, INode, AsRule<ICallRule<{genericsAfter}OutT>>
		=> self.CallsRule(TypeInfo<ICallRule<{genericsAfter}OutT>>.Default{genericParameter}, out outT);
");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeCallRuleGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}