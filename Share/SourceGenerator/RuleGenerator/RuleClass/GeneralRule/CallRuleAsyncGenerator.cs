/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 20:21

* 描述：异步通用调用法则 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleAsyncGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 异步通用调用法则
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
	/// 异步通用调用法则接口
	/// </summary>
	public interface ICallRuleAsync<{genericsAfter}OutT> : ICallRuleAsyncBase<{genericsAfter}OutT> {{}}
");
				Code.Append
($@"
    /// <summary>
	/// 异步通用调用法则
	/// </summary>
	public abstract class CallRuleAsync<N{generics}, OutT> : CallRuleAsyncBase<N, ICallRuleAsync<{genericsAfter}OutT>, {genericsAfter}OutT> where N : class, INode, AsRule<ICallRuleAsync<{genericsAfter}OutT>> {{}}
");
			}
			Code.Append("}");

			context.AddSource("CallRuleAsync.cs", SourceText.From(Code.ToString(), Encoding.UTF8));
		}
	}
}