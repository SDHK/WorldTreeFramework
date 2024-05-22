/****************************************

* 作者：闪电黑客
* 日期：2024/4/7 17:54

* 描述：异步调用法则基类 生成器帮助类

*/

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace WorldTree.SourceGenerator
{
	internal class CallRuleAsyncBaseGenerator
	{
		public static void Execute(GeneratorExecutionContext context)
		{
			int argumentCount = RuleGeneratorSetting.argumentCount;
			StringBuilder Code = new StringBuilder();
			Code.AppendLine(
@$"/****************************************
* 可以理解为Node的有返回值异步方法
*
* ICallRuleAsync 继承 IRule
* 主要作用：统一 调用方法 OutT Invoke(INode self,T1 arg1, ...);
*
* CallRuleAsync 则继承 Rule
* 同时还继承了 ICallRuleAsync 可以转换为 ICallRuleAsync 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 Execute 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*/
"
);

			Code.AppendLine("namespace WorldTree");
			Code.Append("{");

			for (int i = 0; i <= argumentCount; i++)
			{
				string generics = RuleGeneratorHelper.GetGenerics(i);
				string genericsAfter = RuleGeneratorHelper.GetGenerics(i, true);
				string GenericTypeParameter = RuleGeneratorHelper.GetGenericTypeParameter(i);
				string GenericParameter = RuleGeneratorHelper.GetGenericParameter(i);

				Code.Append
($@"

    /// <summary>
    /// 异步调用法则基类接口
    /// </summary>
    public interface ICallRuleAsync<{genericsAfter}OutT> : IRule
	{{
        TreeTask<OutT> Invoke(INode self{GenericTypeParameter});
	}}

    /// <summary>
	/// 异步调用法则基类
	///</summary>
	public abstract class CallRuleAsync<N, R{generics}, OutT> : Rule<N, R>, ICallRuleAsync<{genericsAfter}OutT>
		where N : class, INode, AsRule<R>
		where R : ICallRuleAsync<{genericsAfter}OutT>
	{{
		public virtual TreeTask<OutT> Invoke(INode self{GenericTypeParameter}) => Execute(self as N{GenericParameter});
		protected abstract TreeTask<OutT> Execute(N self{GenericTypeParameter});
	}}
	
	/// <summary>
	/// 异步调用法则基类实现
	/// </summary>
    public abstract class CallRuleAsyncDefault<R{generics}, OutT> : Rule<INode, R>, ICallRuleAsync<{genericsAfter}OutT>
		where R : ICallRuleAsync<{genericsAfter}OutT>
	{{
		public virtual TreeTask<OutT> Invoke(INode self{GenericTypeParameter}) => Execute(self{GenericParameter});
		protected abstract TreeTask<OutT> Execute(INode self{GenericTypeParameter});
	}}
");
			}
			Code.Append("}");

			context.AddSource("CallRuleAsync.cs", SourceText.From(Code.ToString(), Encoding.UTF8));//生成代码
		}
	}
}