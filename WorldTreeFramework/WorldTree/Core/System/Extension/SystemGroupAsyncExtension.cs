using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupAsyncExtension
    {

        #region Send

        public static async AsyncTask<bool> TrySendAsyncSystem(this SystemGroup group, Entity self)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<AsyncTask> system in systems)
                {
                    await system.Invoke(self);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async void SendAsyncSystem(this SystemGroup group, Entity self)
        {
            await group.TrySendAsyncSystem(self);
        }
        public static async void SendAsyncSystem<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            await group.TrySendAsyncSystem(self, arg1);
        }

        public static async void SendAsyncSystem<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            await group.TrySendAsyncSystem(self, arg1, arg2);
        }
        public static async void SendAsyncSystem<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            await group.TrySendAsyncSystem(self, arg1, arg2, arg3);
        }

        #endregion


        #region Call

        public static async AsyncTask<OutT> CallAsyncSystem<OutT>(this SystemGroup group, Entity self)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return outT;
        }


        public static async AsyncTask<OutT> CallAsyncSystem<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self, arg1);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return outT;
        }


        #endregion

        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<OutT>(this SystemGroup group, Entity self)
        {
            bool bit = false;
            UnitList<OutT> values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self));
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            UnitList<OutT> values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self, arg1));
                }
                bit = true;
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
