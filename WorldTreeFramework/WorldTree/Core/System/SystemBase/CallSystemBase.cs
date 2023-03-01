
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 调用系统事件基类
* 
* 可以理解为Entity的有返回值扩展方法
* 
* 
* ICallSystem 继承 IEntitySystem
* 主要作用：统一 调用方法  OutT Invoke(Entity self,T1 ar1, ...);
* 
* 
* CallSystemBase 则继承 SystemBase 
* 同时还继承了 ICallSystem 可以转换为 ICallSystem 进行统一调用。
* 
* 主要作用：确定Entity的类型并转换，并统一 Invoke 中转调用 OnEvent 的过程。
* 其中 Invoke 设定为虚方法方便子类写特殊的中转调用。
* 
*/

namespace WorldTree
{
    public interface ICallSystem<OutT> : IEntitySystem
    {
        OutT Invoke(Entity self);
    }

    public interface ICallSystem<T1, OutT> : IEntitySystem
    {
        OutT Invoke(Entity self, T1 arg1);
    }

    public interface ICallSystem<T1, T2, OutT> : IEntitySystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2);
    }

    public interface ICallSystem<T1, T2, T3, OutT> : IEntitySystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3);
    }
    public interface ICallSystem<T1, T2, T3, T4, OutT> : IEntitySystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface ICallSystem<T1, T2, T3, T4, T5, OutT> : IEntitySystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }



    public abstract class CallSystemBase<S, E, OutT> : SystemBase<E, S>, ICallSystem<OutT>
    where E : Entity
    where S : ICallSystem<OutT>
    {
        public virtual OutT Invoke(Entity self) => OnEvent(self as E);
        public abstract OutT OnEvent(E self);
    }

    public abstract class CallSystemBase<S, E, T1, OutT> : SystemBase<E, S>, ICallSystem<T1, OutT>
    where E : Entity
    where S : ICallSystem<T1, OutT>
    {
        public virtual OutT Invoke(Entity self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract OutT OnEvent(E self, T1 arg1);
    }

    public abstract class CallSystemBase<S, E, T1, T2, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, OutT>
    {
        public virtual OutT Invoke(Entity self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, OutT>
    {
        public virtual OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, T4, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, T4, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, T4, OutT>
    {
        public virtual OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, T4, T5, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, T4, T5, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, T4, T5, OutT>
    {
        public virtual OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
