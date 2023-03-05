/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 17:29

* 描述： 异步发送系统事件基类

*/

namespace WorldTree
{
    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync : IRule
    {
        TreeTask Invoke(Node self);
    }

    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync<T1> : IRule
    {
        TreeTask Invoke(Node self, T1 arg1);
    }

    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync<T1, T2> : IRule
    {
        TreeTask Invoke(Node self, T1 arg1, T2 arg2);
    }

    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync<T1, T2, T3> : IRule
    {
        TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3);
    }

    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync<T1, T2, T3, T4> : IRule
    {
        TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <summary>
    /// 异步发送系统接口
    /// </summary>
    public interface ISendSystemAsync<T1, T2, T3, T4, T5> : IRule
    {
        TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }



    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E> : RuleBase<E, S>, ISendSystemAsync
    where E : Node
    where S : ISendSystemAsync
    {
        public virtual TreeTask Invoke(Node self) => OnEvent(self as E);
        public abstract TreeTask OnEvent(E self);
    }
    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E, T1> : RuleBase<E, S>, ISendSystemAsync<T1>
    where E : Node
    where S : ISendSystemAsync<T1>
    {
        public virtual TreeTask Invoke(Node self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract TreeTask OnEvent(E self, T1 arg1);
    }
    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E, T1, T2> : RuleBase<E, S>, ISendSystemAsync<T1, T2>
    where E : Node
    where S : ISendSystemAsync<T1, T2>
    {
        public virtual TreeTask Invoke(Node self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract TreeTask OnEvent(E self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E, T1, T2, T3> : RuleBase<E, S>, ISendSystemAsync<T1, T2, T3>
    where E : Node
    where S : ISendSystemAsync<T1, T2, T3>
    {
        public virtual TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract TreeTask OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E, T1, T2, T3, T4> : RuleBase<E, S>, ISendSystemAsync<T1, T2, T3, T4>
    where E : Node
    where S : ISendSystemAsync<T1, T2, T3, T4>
    {
        public virtual TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract TreeTask OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 异步发送系统抽象基类
    /// </summary>
    public abstract class SendSystemAsyncBase<S, E, T1, T2, T3, T4, T5> : RuleBase<E, S>, ISendSystemAsync<T1, T2, T3, T4, T5>
    where E : Node
    where S : ISendSystemAsync<T1, T2, T3, T4, T5>
    {
        public virtual TreeTask Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract TreeTask OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
