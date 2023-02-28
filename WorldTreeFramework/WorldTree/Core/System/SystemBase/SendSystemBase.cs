using System.Collections.Generic;

namespace WorldTree
{

    public interface ISendSystem : ISystem
    {
        void Invoke(Entity self);
    }

    public interface ISendSystem<T1> : ISystem
    {
        void Invoke(Entity self, T1 arg1);
    }

    public interface ISendSystem<T1, T2> : ISystem
    {
        void Invoke(Entity self, T1 arg1, T2 arg2);
    }

    public interface ISendSystem<T1, T2, T3> : ISystem
    {
        void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3);
    }

    public interface ISendSystem<T1, T2, T3, T4> : ISystem
    {
        void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface ISendSystem<T1, T2, T3, T4, T5> : ISystem
    {
        void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public abstract class SendSystemBase<S, E> : SystemBase<E, S>, ISendSystem
    where E : Entity
    where S : ISendSystem
    {
        public virtual void Invoke(Entity self) => OnEvent(self as E);
        public abstract void OnEvent(E self);
    }
    public abstract class SendSystemBase<S, E, T1> : SystemBase<E, S>, ISendSystem<T1>
    where E : Entity
    where S : ISendSystem<T1>
    {
        public virtual void Invoke(Entity self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract void OnEvent(E self, T1 arg1);
    }

    public abstract class SendSystemBase<S, E, T1, T2> : SystemBase<E, S>, ISendSystem<T1, T2>
    where E : Entity
    where S : ISendSystem<T1, T2>
    {
        public virtual void Invoke(Entity self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2);
    }

    public abstract class SendSystemBase<S, E, T1, T2, T3> : SystemBase<E, S>, ISendSystem<T1, T2, T3>
    where E : Entity
    where S : ISendSystem<T1, T2, T3>
    {
        public virtual void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class SendSystemBase<S, E, T1, T2, T3, T4> : SystemBase<E, S>, ISendSystem<T1, T2, T3, T4>
    where E : Entity
    where S : ISendSystem<T1, T2, T3, T4>
    {
        public virtual void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class SendSystemBase<S, E, T1, T2, T3, T4, T5> : SystemBase<E, S>, ISendSystem<T1, T2, T3, T4, T5>
    where E : Entity
    where S : ISendSystem<T1, T2, T3, T4, T5>
    {
        public virtual void Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract void OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
