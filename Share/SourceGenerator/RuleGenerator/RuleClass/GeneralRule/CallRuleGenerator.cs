/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 20:18

* 描述：通用调用法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 通用调用法则
*/
"
);
			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				Code.Append
($@"

	/// <summary>
	/// 节点：通用调用法则限制
	/// </summary>
	/// <remarks>节点拥有的法则，和Where约束搭配形成法则调用限制</remarks>
    public interface AsCallRule<{genericsAfter}OutT> : AsRule<ICallRule<{genericsAfter}OutT>> {{}}

	/// <summary>
	/// 通用调用法则接口
	/// </summary>
	public interface ICallRule<{genericsAfter}OutT> : ICallRuleBase<{genericsAfter}OutT> {{}}

	/// <summary>
	/// 通用调用法则
	/// </summary>
    public abstract class CallRule<N{generics}, OutT> : CallRuleBase<N, ICallRule<{genericsAfter}OutT>, {genericsAfter}OutT> where N : class, INode, AsRule<ICallRule<{genericsAfter}OutT>> {{}}
");
			}
			Code.Append("}");

			context.AddSource("CallRule.cs", SourceText.From(Code.ToString(), System.Text.Encoding.UTF8));
		}
	}
}