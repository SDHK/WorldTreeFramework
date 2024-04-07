/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 调用法则基类
*
* 可以理解为Node的有返回值扩展方法
*
*
* ICallRuleBase 继承 IRule
* 主要作用：统一 调用方法  OutT Invoke(INode self,T1 ar1, ...);
*
*
* CallRuleBase 则继承 RuleBase
* 同时还继承了 ICallRuleBase 可以转换为 ICallRuleBase 进行统一调用。
*
* 主要作用：确定Node的类型并转换，并统一 Invoke 中转调用 OnEvent 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
*
*/

namespace WorldTree
{
	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<OutT> : IRule
	//{
	//	OutT Invoke(INode self);
	//}

	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<T1, OutT> : IRule
	//{
	//	OutT Invoke(INode self, T1 arg1);
	//}

	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<T1, T2, OutT> : IRule
	//{
	//	OutT Invoke(INode self, T1 arg1, T2 arg2);
	//}

	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<T1, T2, T3, OutT> : IRule
	//{
	//	OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
	//}

	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<T1, T2, T3, T4, OutT> : IRule
	//{
	//	OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	//}

	///// <summary>
	///// 调用法则基类接口
	///// </summary>
	//public interface ICallRuleBase<T1, T2, T3, T4, T5, OutT> : IRule
	//{
	//	OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, OutT> : RuleBase<N, R>, ICallRuleBase<OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<OutT>
	//{
	//	public virtual OutT Invoke(INode self) => Execute(self as N);

	//	protected abstract OutT Execute(N self);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, T1, OutT> : RuleBase<N, R>, ICallRuleBase<T1, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<T1, OutT>
	//{
	//	public virtual OutT Invoke(INode self, T1 arg1) => Execute(self as N, arg1);

	//	protected abstract OutT Execute(N self, T1 arg1);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, T1, T2, OutT> : RuleBase<N, R>, ICallRuleBase<T1, T2, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<T1, T2, OutT>
	//{
	//	public virtual OutT Invoke(INode self, T1 arg1, T2 arg2) => Execute(self as N, arg1, arg2);

	//	protected abstract OutT Execute(N self, T1 arg1, T2 arg2);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, T1, T2, T3, OutT> : RuleBase<N, R>, ICallRuleBase<T1, T2, T3, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<T1, T2, T3, OutT>
	//{
	//	public virtual OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => Execute(self as N, arg1, arg2, arg3);

	//	protected abstract OutT Execute(N self, T1 arg1, T2 arg2, T3 arg3);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, T1, T2, T3, T4, OutT> : RuleBase<N, R>, ICallRuleBase<T1, T2, T3, T4, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<T1, T2, T3, T4, OutT>
	//{
	//	public virtual OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Execute(self as N, arg1, arg2, arg3, arg4);

	//	protected abstract OutT Execute(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	//}

	///// <summary>
	///// 调用法则基类
	///// </summary>
	//public abstract class CallRuleBase<N, R, T1, T2, T3, T4, T5, OutT> : RuleBase<N, R>, ICallRuleBase<T1, T2, T3, T4, T5, OutT>
	//where N : class, INode, AsRule<R>
	//where R : ICallRuleBase<T1, T2, T3, T4, T5, OutT>
	//{
	//	public virtual OutT Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Execute(self as N, arg1, arg2, arg3, arg4, arg5);

	//	protected abstract OutT Execute(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	//}
}