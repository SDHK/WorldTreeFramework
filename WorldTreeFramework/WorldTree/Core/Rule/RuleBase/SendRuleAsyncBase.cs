/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:29

* 描述： 异步通知法则基类

*/

namespace WorldTree
{
    /// <summary>
    /// 异步通知法则接口
    /// </summary>
    public interface ISendRuleAsync : IRule
    {
        TreeTask Invoke(INode self);
    }

    /// <summary>
    /// 异步通知法则接口
    /// </summary>
    public interface ISendRuleAsync<T1> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1);
    }

    /// <summary>
    /// 异步通知系统接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2);
    }

    /// <summary>
    /// 异步通知系统接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
    }

    /// <summary>
    /// 异步通知系统接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3, T4> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <summary>
    /// 异步通知系统接口
    /// </summary>
    public interface ISendRuleAsync<T1, T2, T3, T4, T5> : IRule
    {
        TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }



    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N> : RuleBase<N, R>, ISendRuleAsync
    where N : class,INode
    where R : ISendRuleAsync
    {
        public virtual TreeTask Invoke(INode self) => OnEvent(self as N);
        public abstract TreeTask OnEvent(N self);
    }
    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N, T1> : RuleBase<N, R>, ISendRuleAsync<T1>
    where N : class,INode
    where R : ISendRuleAsync<T1>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1) => OnEvent(self as N, arg1);
        public abstract TreeTask OnEvent(N self, T1 arg1);
    }
    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N, T1, T2> : RuleBase<N, R>, ISendRuleAsync<T1, T2>
    where N : class,INode
    where R : ISendRuleAsync<T1, T2>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2) => OnEvent(self as N, arg1, arg2);
        public abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N, T1, T2, T3> : RuleBase<N, R>, ISendRuleAsync<T1, T2, T3>
    where N : class,INode
    where R : ISendRuleAsync<T1, T2, T3>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as N, arg1, arg2, arg3);
        public abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N, T1, T2, T3, T4> : RuleBase<N, R>, ISendRuleAsync<T1, T2, T3, T4>
    where N : class,INode
    where R : ISendRuleAsync<T1, T2, T3, T4>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as N, arg1, arg2, arg3, arg4);
        public abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步通知法则抽象基类
    /// </summary>
    public abstract class SendRuleAsyncBase<R, N, T1, T2, T3, T4, T5> : RuleBase<N, R>, ISendRuleAsync<T1, T2, T3, T4, T5>
    where N : class,INode
    where R : ISendRuleAsync<T1, T2, T3, T4, T5>
    {
        public virtual TreeTask Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as N, arg1, arg2, arg3, arg4, arg5);
        public abstract TreeTask OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
