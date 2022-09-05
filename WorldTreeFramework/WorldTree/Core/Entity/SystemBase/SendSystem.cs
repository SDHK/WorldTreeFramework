namespace WorldTree
{
    public interface ISendSystem : ISystem
    {
        void Invoke(object self);
    }

    public interface ISendSystem<T1> : ISystem
    {
        void Invoke(object self, T1 arg1);
    }


    public abstract class SendSystem<T> : SystemBase<T, ISendSystem>, ISendSystem
       where T : class
    {
        public void Invoke(object self) => Event(self as T);
        public abstract void Event(T self);
    }

    public abstract class SendSystem<T, T1> : SystemBase<T, ISendSystem<T1>>, ISendSystem<T1>
       where T : class
    {
        public void Invoke(object self, T1 arg1) => Event(self as T, arg1);
        public abstract void Event(T self, T1 arg1);
    }
}
