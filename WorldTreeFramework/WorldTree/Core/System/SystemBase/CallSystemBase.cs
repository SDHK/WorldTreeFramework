namespace WorldTree
{
    public interface ICallSystem<OutT> : ISystem
    {
        OutT Invoke(Entity self);
    }

    public interface ICallSystem<T1, OutT> : ISystem
    {
        OutT Invoke(Entity self, T1 arg1);
    }

    public interface ICallSystem<T1, T2, OutT> : ISystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2);
    }

    public interface ICallSystem<T1, T2, T3, OutT> : ISystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3);
    }
    public interface ICallSystem<T1, T2, T3, T4, OutT> : ISystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface ICallSystem<T1, T2, T3, T4, T5, OutT> : ISystem
    {
        OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }



    public abstract class CallSystemBase<S, E, OutT> : SystemBase<E, S>, ICallSystem<OutT>
    where E : Entity
    where S : ICallSystem<OutT>
    {
        public OutT Invoke(Entity self) => OnEvent(self as E);
        public abstract OutT OnEvent(E self);
    }

    public abstract class CallSystemBase<S, E, T1, OutT> : SystemBase<E, S>, ICallSystem<T1, OutT>
    where E : Entity
    where S : ICallSystem<T1, OutT>
    {
        public OutT Invoke(Entity self, T1 arg1) => OnEvent(self as E, arg1);
        public abstract OutT OnEvent(E self, T1 arg1);
    }

    public abstract class CallSystemBase<S, E, T1, T2, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, OutT>
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2) => OnEvent(self as E, arg1, arg2);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, OutT>
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3) => OnEvent(self as E, arg1, arg2, arg3);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, T4, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, T4, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, T4, OutT>
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent(self as E, arg1, arg2, arg3, arg4);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class CallSystemBase<S, E, T1, T2, T3, T4, T5, OutT> : SystemBase<E, S>, ICallSystem<T1, T2, T3, T4, T5, OutT>
    where E : Entity
    where S : ICallSystem<T1, T2, T3, T4, T5, OutT>
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent(self as E, arg1, arg2, arg3, arg4, arg5);
        public abstract OutT OnEvent(E self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
