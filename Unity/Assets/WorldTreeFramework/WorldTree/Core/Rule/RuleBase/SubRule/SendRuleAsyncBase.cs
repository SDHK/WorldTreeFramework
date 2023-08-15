/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:29

* 描述： 异步通知法则基类

*/

namespace WorldTree
{
    /// <summary>
    /// 异步通知法则基类接口
    /// </summary>
    public interface ISendRuleAsyncBase : IRule
    {
        TreeTask Invoke(INode self);
    }

    /// <summary>
    /// 异步通知法则基类接口
    /// </summary>
    public interface ISendRuleAsyncBase<T1> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1);
    }

    /// <summary>
    /// 异步通知系统基类接口
    /// </summary>
    public interface ISendRuleAsyncBase<T1, T2> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2);
    }

    /// <summary>
    /// 异步通知系统基类接口
    /// </summary>
    public interface ISendRuleAsyncBase<T1, T2, T3> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
    }

    /// <summary>
    /// 异步通知系统基类接口
    /// </summary>
    public interface ISendRuleAsyncBase<T1, T2, T3, T4> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <summary>
    /// 异步通知系统基类接口
    /// </summary>
    public interface ISendRuleAsyncBase<T1, T2, T3, T4, T5> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }



    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R> : RuleBase<N, R>, ISendRuleAsyncBase
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase
    {
        public virtual TreeTask Invoke(INode self) => OnEvent(self as N);
        protected abstract TreeTask OnEvent(N self);
    }
    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R, T1> : RuleBase<N, R>, ISendRuleAsyncBase<T1>
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase<T1>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1) => OnEvent(self as N, arg1);
        protected abstract TreeTask OnEvent(N self, T1 arg1);
    }
    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R, T1, T2> : RuleBase<N, R>, ISendRuleAsyncBase<T1, T2>
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase<T1, T2>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2) => OnEvent(self as N, arg1, arg2);
        protected abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R, T1, T2, T3> : RuleBase<N, R>, ISendRuleAsyncBase<T1, T2, T3>
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase<T1, T2, T3>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as N, arg1, arg2, arg3);
        protected abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R, T1, T2, T3, T4> : RuleBase<N, R>, ISendRuleAsyncBase<T1, T2, T3, T4>
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase<T1, T2, T3, T4>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as N, arg1, arg2, arg3, arg4);
        protected abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步通知法则基类
    /// </summary>
    public abstract class SendRuleAsyncBase<N, R, T1, T2, T3, T4, T5> : RuleBase<N, R>, ISendRuleAsyncBase<T1, T2, T3, T4, T5>
    where N : class, INode, AsRule<R>
    where R : ISendRuleAsyncBase<T1, T2, T3, T4, T5>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as N, arg1, arg2, arg3, arg4, arg5);
        protected abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
