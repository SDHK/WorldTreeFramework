using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemExtension
    {
        public static void Run<I>(this Entity self)
            where I : ISystem
        {
            var Group = self.Root.SystemManager.GetSystemGroup<I>();
            if (Group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem system in systems)
                {
                    system.Invoke(self);
                }
            }
        }

    }

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

    public abstract class SendSystem<T> : SystemBase<T, ISendSystem>, ISendSystem
       where T : Entity
    {
        public void Invoke(Entity self) => Event(self as T);
        public abstract void Event(T self);
    }

    public abstract class SendSystem<T, T1> : SystemBase<T, ISendSystem<T1>>, ISendSystem<T1>
       where T : Entity
    {
        public void Invoke(Entity self, T1 arg1) => Event(self as T, arg1);
        public abstract void Event(T self, T1 arg1);
    }
}
