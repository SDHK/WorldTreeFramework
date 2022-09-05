namespace WorldTree
{
    public interface ICallSystem<OutT> : ISystem
    {
        OutT Invoke(object self);
    }

    public interface ICallSystem<T1, OutT> : ISystem
    {
        OutT Invoke(object self, T1 arg1);
    }

    public abstract class CallSystem<T, OutT> : SystemBase<T, ICallSystem<OutT>>, ICallSystem<OutT>
     where T : class
    {
        public OutT Invoke(object self) => Event(self as T);
        public abstract OutT Event(T self);
    }

    public abstract class CallSystem<T, T1, OutT> : SystemBase<T, ICallSystem<T1, OutT>>, ICallSystem<T1, OutT>
    where T : class
    {
        public OutT Invoke(object self, T1 arg1) => Event(self as T, arg1);
        public abstract OutT Event(T self, T1 arg1);
    }
}
