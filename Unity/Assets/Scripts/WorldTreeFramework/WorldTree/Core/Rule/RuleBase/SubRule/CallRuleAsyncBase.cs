/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:51

* 描述： 异步调用法则基类
*

*/

namespace WorldTree
{
	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self);
	//}

	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<T1, OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self, T1 arg1);
	//}

	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<T1, T2, OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2);
	//}

	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<T1, T2, T3, OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
	//}

	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<T1, T2, T3, T4, OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	//}

	///// <summary>
	///// 异步调用法则基类接口
	///// </summary>
	//public interface ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT> : IRule
	//{
	//	TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self) => Execute(self as N);

	//	protected abstract TreeTask<OutT> Execute(N self);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, T1, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<T1, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<T1, OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self, T1 arg1) => Execute(self as N, arg1);

	//	protected abstract TreeTask<OutT> Execute(N self, T1 arg1);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, T1, T2, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<T1, T2, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<T1, T2, OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2) => Execute(self as N, arg1, arg2);

	//	protected abstract TreeTask<OutT> Execute(N self, T1 arg1, T2 arg2);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<T1, T2, T3, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => Execute(self as N, arg1, arg2, arg3);

	//	protected abstract TreeTask<OutT> Execute(N self, T1 arg1, T2 arg2, T3 arg3);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, T4, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Execute(self as N, arg1, arg2, arg3, arg4);

	//	protected abstract TreeTask<OutT> Execute(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	//}

	///// <summary>
	///// 异步调用法则基类
	///// </summary>
	//public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, T4, T5, OutT> : RuleBase<N, R>, ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
	//{
	//	public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Execute(self as N, arg1, arg2, arg3, arg4, arg5);

	//	protected abstract TreeTask<OutT> Execute(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	//}
}