using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupAsyncExtension
    {

        #region Send

        public static async AsyncTask<bool> TrySendAsync(this SystemGroup group, Entity self)
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
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1>(this SystemGroup group, Entity self, T1 arg1)
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
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
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
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
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
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3, arg4);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }



        public static async void SendAsync(this SystemGroup group, Entity self)
        {
            await group.TrySendAsync(self);
        }
        public static async void SendAsync<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            await group.TrySendAsync(self, arg1);
        }
        public static async void SendAsync<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            await group.TrySendAsync(self, arg1, arg2);
        }
        public static async void SendAsync<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3);
        }
        public static async void SendAsync<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
        }
        public static async void SendAsync<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
        }
        #endregion


        #region Call

        public static async AsyncTask<OutT> CallAsync<OutT>(this SystemGroup group, Entity self)
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
                await self.AsyncTaskCompleted();
            }
            return outT;
        }


        public static async AsyncTask<OutT> CallAsync<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
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
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self, arg1, arg2);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self, arg1, arg2, arg3);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }
        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self, arg1, arg2, arg3, arg4);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            bool bit = false;
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>> system in systems)
                {
                    outT = await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }


        #endregion

        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsync<OutT>(this SystemGroup group, Entity self)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
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
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
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
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self, arg1, arg2));
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self, arg1, arg2, arg3));
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4));
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            bool bit = false;
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>> system in systems)
                {
                    values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4, arg5));
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        #endregion
    }
}
