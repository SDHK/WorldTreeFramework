/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:51

* 描述： 异步调用系统事件基类
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self);
    }
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<T1, OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self, T1 arg1);
    }
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<T1, T2, OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<T1, T2, T3, OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<T1, T2, T3, T4, OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步调用系统接口
    /// </summary>
    public interface ICallSystemAsync<T1, T2, T3, T4, T5, OutT> : IRule
    {
        TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, OutT> : RuleBase<E, S>, ICallSystemAsync<OutT>
    where E : Node
    where S : ICallSystemAsync<OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self) => OnEvent(self as E);
        public abstract TreeTask<OutT> OnEvent(E self);
    }
    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, T1, OutT> : RuleBase<E, S>, ICallSystemAsync<T1, OutT>
    where E : Node
    where S : ICallSystemAsync<T1, OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract TreeTask<OutT> OnEvent(E self, T1 arg1);
    }
    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, T1, T2, OutT> : RuleBase<E, S>, ICallSystemAsync<T1, T2, OutT>
    where E : Node
    where S : ICallSystemAsync<T1, T2, OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract TreeTask<OutT> OnEvent(E self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, T1, T2, T3, OutT> : RuleBase<E, S>, ICallSystemAsync<T1, T2, T3, OutT>
    where E : Node
    where S : ICallSystemAsync<T1, T2, T3, OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract TreeTask<OutT> OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, T1, T2, T3, T4, OutT> : RuleBase<E, S>, ICallSystemAsync<T1, T2, T3, T4, OutT>
    where E : Node
    where S : ICallSystemAsync<T1, T2, T3, T4, OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract TreeTask<OutT> OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步调用系统抽象基类
    /// </summary>
    public abstract class CallSystemAsyncBase<S, E, T1, T2, T3, T4, T5, OutT> : RuleBase<E, S>, ICallSystemAsync<T1, T2, T3, T4, T5, OutT>
    where E : Node
    where S : ICallSystemAsync<T1, T2, T3, T4, T5, OutT>
    {
        public virtual TreeTask<OutT> Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract TreeTask<OutT> OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
