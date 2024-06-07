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
				string genericsType = GeneratorTemplate.GenericsTypes[i];
				string genericsTypeAfter = GeneratorTemplate.GenericsTypesAfter[i];
				string genericParameter = GeneratorTemplate.GenericsParameter[i];
				string genericTypeParameter = GeneratorTemplate.GenericsTypeParameter[i];
				
				Code.AppendLine(
					$$"""
							/// <summary>
							/// 尝试执行异步调用法则
							/// </summary>
							public static TreeTask<OutT> TryCallAsync<{{{genericsTypeAfter}}}OutT>(this INode self{{genericTypeParameter}}, OutT defaultOutT)
								=> self.TryCallRuleAsync(TypeInfo<ICallRuleAsync<{{genericsTypeAfter}}OutT>>.Default{{genericParameter}}, defaultOutT);

							/// <summary>
							/// 执行异步调用法则
							/// </summary>
							public static TreeTask<OutT> CallAsync<N{{genericsType}}, OutT>(this N self{{genericTypeParameter}}, OutT defaultOutT)
								where N : class, INode, AsRule<ICallRuleAsync<{{genericsTypeAfter}}OutT>>
							=> self.CallRuleAsync(TypeInfo<ICallRuleAsync<{{genericsTypeAfter}}OutT>>.Default{{genericParameter}}, defaultOutT);

							/// <summary>
							/// 尝试执行异步调用法则
							/// </summary>
							public static TreeTask<UnitList<OutT>> TryCallsAsync<{{genericsTypeAfter}}OutT>(this INode self{{genericTypeParameter}}, OutT defaultOutT)
								=> self.TryCallsRuleAsync(TypeInfo<ICallRuleAsync<{{genericsTypeAfter}}OutT>>.Default{{genericParameter}}, defaultOutT);

							/// <summary>
							/// 执行异步调用法则
							/// </summary>
							public static TreeTask<UnitList<OutT>> CallsAsync<N{{genericsType}}, OutT>(this N self{{genericTypeParameter}}, OutT defaultOutT)
								where N : class, INode, AsRule<ICallRuleAsync<{{genericsTypeAfter}}OutT>>
							=> self.CallsRuleAsync(TypeInfo<ICallRuleAsync<{{genericsTypeAfter}}OutT>>.Default{{genericParameter}}, defaultOutT);
					""");
			}
			Code.AppendLine("	}");
			Code.Append("}");

			context.AddSource("NodeCallRuleAsyncGeneral.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}