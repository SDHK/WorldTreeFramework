/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 发送系统事件基类
* 
* 可以理解为Entity的无返回值扩展方法
* 
* 
* ISendSystem 继承 IRule
* 主要作用：统一 调用方法 Invoke(Node self,T1 ar1, ...);
* 
* 
* SendSystemBase 则继承 RuleBase 
* 同时还继承了 ISendSystem 可以转换为 ISendSystem 进行统一调用。
* 
* 主要作用：确定Entity的类型并转换，并统一 Invoke 中转调用 OnEvent 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
* 

*/

namespace WorldTree
{

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem : IRule
    {
        void Invoke(Node self);
    }

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem<T1> : IRule
    {
        void Invoke(Node self, T1 arg1);
    }

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem<T1, T2> : IRule
    {
        void Invoke(Node self, T1 arg1, T2 arg2);
    }

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem<T1, T2, T3> : IRule
    {
        void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3);
    }

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem<T1, T2, T3, T4> : IRule
    {
        void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    /// <summary>
    /// 发送系统接口
    /// </summary>
    public interface ISendSystem<T1, T2, T3, T4, T5> : IRule
    {
        void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E> : RuleBase<E, S>, ISendSystem
    where E : Node
    where S : ISendSystem
    {
        public virtual void Invoke(Node self) => OnEvent(self as E);
        public abstract void OnEvent(E self);
    }
    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E, T1> : RuleBase<E, S>, ISendSystem<T1>
    where E : Node
    where S : ISendSystem<T1>
    {
        public virtual void Invoke(Node self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract void OnEvent(E self, T1 arg1);
    }
    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E, T1, T2> : RuleBase<E, S>, ISendSystem<T1, T2>
    where E : Node
    where S : ISendSystem<T1, T2>
    {
        public virtual void Invoke(Node self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2);
    }
    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E, T1, T2, T3> : RuleBase<E, S>, ISendSystem<T1, T2, T3>
    where E : Node
    where S : ISendSystem<T1, T2, T3>
    {
        public virtual void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }
    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E, T1, T2, T3, T4> : RuleBase<E, S>, ISendSystem<T1, T2, T3, T4>
    where E : Node
    where S : ISendSystem<T1, T2, T3, T4>
    {
        public virtual void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
    /// <summary>
    /// 发送系统抽象基类
    /// </summary>
    public abstract class SendSystemBase<S, E, T1, T2, T3, T4, T5> : RuleBase<E, S>, ISendSystem<T1, T2, T3, T4, T5>
    where E : Node
    where S : ISendSystem<T1, T2, T3, T4, T5>
    {
        public virtual void Invoke(Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
