/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 通知法则基类
* 
* 可以理解为Entity的无返回值扩展方法
* 
* 
* ISendRule 继承 IRule
* 主要作用：统一 调用方法 Invoke(INode self,T1 ar1, ...);
* 
* 
* SendRuleBase 则继承 RuleBase 
* 同时还继承了 ISendRule 可以转换为 ISendRule 进行统一调用。
* 
* 主要作用：确定Entity的类型并转换，并统一 Invoke 中转调用 OnEvent 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
* 

*/

namespace WorldTree
{

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase : IRule
    {
        void Invoke(INode self);
    }

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase<T1> : IRule
    {
        void Invoke(INode self, T1 arg1);
    }

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase<T1, T2> : IRule
    {
        void Invoke(INode self, T1 arg1, T2 arg2);
    }

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase<T1, T2, T3> : IRule
    {
        void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3);
    }

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase<T1, T2, T3, T4> : IRule
    {
        void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <summary>
    /// 通知法则基类接口
    /// </summary>
    public interface ISendRuleBase<T1, T2, T3, T4, T5> : IRule
    {
        void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R> : RuleBase<N, R>, ISendRuleBase
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase
    {
        public virtual void Invoke(INode self) => OnEvent(self as N);
        public abstract void OnEvent(N self);
    }
    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R, T1> : RuleBase<N, R>, ISendRuleBase<T1>
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase<T1>
    {
        public virtual void Invoke(INode self, T1 arg1) => OnEvent(self as N, arg1);
        public abstract void OnEvent(N self, T1 arg1);
    }
    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R, T1, T2> : RuleBase<N, R>, ISendRuleBase<T1, T2>
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase<T1, T2>
    {
        public virtual void Invoke(INode self, T1 arg1, T2 arg2) => OnEvent(self as N, arg1, arg2);
        public abstract void OnEvent(N self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R, T1, T2, T3> : RuleBase<N, R>, ISendRuleBase<T1, T2, T3>
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase<T1, T2, T3>
    {
        public virtual void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as N, arg1, arg2, arg3);
        public abstract void OnEvent(N self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R, T1, T2, T3, T4> : RuleBase<N, R>, ISendRuleBase<T1, T2, T3, T4>
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase<T1, T2, T3, T4>
    {
        public virtual void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as N, arg1, arg2, arg3, arg4);
        public abstract void OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 通知法则基类
    /// </summary>
    public abstract class SendRuleBase<N, R, T1, T2, T3, T4, T5> : RuleBase<N, R>, ISendRuleBase<T1, T2, T3, T4, T5>
    where N : class, INode, AsRule<R>
    where R : ISendRuleBase<T1, T2, T3, T4, T5>
    {
        public virtual void Invoke(INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as N, arg1, arg2, arg3, arg4, arg5);
        public abstract void OnEvent(N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
