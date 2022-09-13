using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupAsyncExtension
    {

        #region Send

        public static async AsyncTask<bool> TrySendAsyncSystem<S>(this SystemGroup group, Entity self)
        where S : ICallSystem<AsyncTask>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        await system.Invoke(self);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1>(this SystemGroup group, Entity self, T1 arg1)
         where S : ICallSystem<T1, AsyncTask>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        await system.Invoke(self, arg1);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
       where S : ICallSystem<T1, T2, AsyncTask>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ICallSystem<T1, T2, AsyncTask> system in systems)
                    {
                        await system.Invoke(self, arg1, arg2);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        await system.Invoke(self, arg1, arg2, arg3);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async void SendAsyncSystem<S>(this SystemGroup group, Entity self)
        where S : ICallSystem<AsyncTask>
        {
            await group.TrySendAsyncSystem<S>(self);
        }
        public static async void SendAsyncSystem<S, T1>(this SystemGroup group, Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask>
        {
            await group.TrySendAsyncSystem<S, T1>(self, arg1);
        }

        public static async void SendAsyncSystem<S, T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
           where S : ICallSystem<T1, T2, AsyncTask>
        {
            await group.TrySendAsyncSystem<S, T1, T2>(self, arg1, arg2);
        }
        public static async void SendAsyncSystem<S, T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
          where S : ICallSystem<T1, T2, T3, AsyncTask>
        {
            await group.TrySendAsyncSystem<S, T1, T2, T3>(self, arg1, arg2, arg3);
        }

        #endregion


        #region Call

        public static async AsyncTask<OutT> CallAsyncSystem<S, OutT>(this SystemGroup group, Entity self)
        where S : ICallSystem<AsyncTask<OutT>>
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        outT = await system.Invoke(self);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return outT;
        }


        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        outT = await system.Invoke(self, arg1);
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return outT;
        }


        #endregion

        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, OutT>(this SystemGroup group, Entity self)
       where S : ICallSystem<AsyncTask<OutT>>
        {
            bool bit = false;
            UnitList<OutT> values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        values.Add(await system.Invoke(self));
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            bool bit = false;
            UnitList<OutT> values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (S system in systems)
                    {
                        values.Add(await system.Invoke(self,arg1));
                    }
                    bit = true;
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return values;
        }

        #endregion
    }
}
