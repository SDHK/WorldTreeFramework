/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:51

* 描述： 异步调用法则基类
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self);
    }
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<T1, OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self, T1 arg1);
    }
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<T1, T2, OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<T1, T2, T3, OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<T1, T2, T3, T4, OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步调用法则接口
    /// </summary>
    public interface ICallRuleAsync<T1, T2, T3, T4, T5, OutT> : IRule
    {
        TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, OutT> : RuleBase<N, R>, ICallRuleAsync<OutT>
    where N : class, INode
    where R : ICallRuleAsync<OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self) => OnEvent(self as N);
        public abstract TreeTask<OutT> OnEvent(N self);
    }
    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, T1, OutT> : RuleBase<N, R>, ICallRuleAsync<T1, OutT>
    where N : class, INode
    where R : ICallRuleAsync<T1, OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self, T1 arg1) => OnEvent(self as N, arg1);
        public abstract TreeTask<OutT> OnEvent(N self, T1 arg1);
    }
    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, T1, T2, OutT> : RuleBase<N, R>, ICallRuleAsync<T1, T2, OutT>
    where N : class, INode
    where R : ICallRuleAsync<T1, T2, OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2) => OnEvent(self as N, arg1, arg2);
        public abstract TreeTask<OutT> OnEvent(N self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, OutT> : RuleBase<N, R>, ICallRuleAsync<T1, T2, T3, OutT>
    where N : class, INode
    where R : ICallRuleAsync<T1, T2, T3, OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as N, arg1, arg2, arg3);
        public abstract TreeTask<OutT> OnEvent(N self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, T4, OutT> : RuleBase<N, R>, ICallRuleAsync<T1, T2, T3, T4, OutT>
    where N : class, INode
    where R : ICallRuleAsync<T1, T2, T3, T4, OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as N, arg1, arg2, arg3, arg4);
        public abstract TreeTask<OutT> OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步调用法则抽象基类
    /// </summary>
    public abstract class CallRuleAsyncBase<N, R, T1, T2, T3, T4, T5, OutT> : RuleBase<N, R>, ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
    where N : class, INode
    where R : ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
    {
        public virtual TreeTask<OutT> Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as N, arg1, arg2, arg3, arg4, arg5);
        public abstract TreeTask<OutT> OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
