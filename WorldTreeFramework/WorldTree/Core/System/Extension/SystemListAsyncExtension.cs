using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListAsyncExtension
    {
        #region Send

        public static async void SendAsync(this List<ISystem> systems, Entity self)
        {
            foreach (ICallSystem<AsyncTask> system in systems)
            {
                await system.Invoke(self);
            }
        }

        public static async void SendAsync<T1>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            foreach (ICallSystem<T1, AsyncTask> system in systems)
            {
                await system.Invoke(self, arg1);
            }
        }

        public static async void SendAsync<T1, T2>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            foreach (ICallSystem<T1, T2, AsyncTask> system in systems)
            {
                await system.Invoke(self, arg1, arg2);
            }
        }
        public static async void SendAsync<T1, T2, T3>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (ICallSystem<T1, T2, T3, AsyncTask> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3);
            }
        }
        public static async void SendAsync<T1, T2, T3, T4>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (ICallSystem<T1, T2, T3, T4, AsyncTask> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3, arg4);
            }
        }
        public static async void SendAsync<T1, T2, T3, T4, T5>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask> system in systems)
            {
                await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
        }

        public static async AsyncTask<OutT> CallAsync<OutT>(this List<ISystem> systems, Entity self)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self);
            }
            return outT;
        }

        #endregion

        #region Call

        public static async AsyncTask<OutT> CallAsync<T1, OutT>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self, arg1);
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2);
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3);
            }
            return outT;
        }
        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
            return outT;
        }

        #endregion


        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsync<OutT>(this List<ISystem> systems, Entity self)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self));
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, OutT>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self, arg1));
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2));
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3));
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4));
            }
            return values;
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4, arg5));
            }
            return values;
        }

        #endregion

    }
}
