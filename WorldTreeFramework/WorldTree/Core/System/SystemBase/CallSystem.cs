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



    public abstract class CallSystem<T, OutT> : SystemBase<T, ICallSystem<OutT>>, ICallSystem<OutT>
     where T : Entity
    {
        public OutT Invoke(Entity self) => Event(self as T);
        public abstract OutT Event(T self);
    }

    public abstract class CallSystem<T, T1, OutT> : SystemBase<T, ICallSystem<T1, OutT>>, ICallSystem<T1, OutT>
    where T : Entity
    {
        public OutT Invoke(Entity self, T1 arg1) => Event(self as T, arg1);
        public abstract OutT Event(T self, T1 arg1);
    }

    public abstract class CallSystem<T, T1, T2, OutT> : SystemBase<T, ICallSystem<T1, T2, OutT>>, ICallSystem<T1, T2, OutT>
    where T : Entity
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2) => Event(self as T, arg1, arg2);
        public abstract OutT Event(T self, T1 arg1, T2 arg2);
    }

    public abstract class CallSystem<T, T1, T2, T3, OutT> : SystemBase<T, ICallSystem<T1, T2, T3, OutT>>, ICallSystem<T1, T2, T3, OutT>
    where T : Entity
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3) => Event(self as T, arg1, arg2, arg3);
        public abstract OutT Event(T self, T1 arg1, T2 arg2, T3 arg3);
    }

    public abstract class CallSystem<T, T1, T2, T3, T4, OutT> : SystemBase<T, ICallSystem<T1, T2, T3, T4, OutT>>, ICallSystem<T1, T2, T3, T4, OutT>
    where T : Entity
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Event(self as T, arg1, arg2, arg3, arg4);
        public abstract OutT Event(T self, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public abstract class CallSystem<T, T1, T2, T3, T4, T5, OutT> : SystemBase<T, ICallSystem<T1, T2, T3, T4, T5, OutT>>, ICallSystem<T1, T2, T3, T4, T5, OutT>
    where T : Entity
    {
        public OutT Invoke(Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Event(self as T, arg1, arg2, arg3, arg4, arg5);
        public abstract OutT Event(T self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
