/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:57

* 描述： 系统组系统事件异步调用

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupCallAsyncExtension
    {


        #region Call

        public static async AsyncTask<OutT> CallAsync<OutT>(this SystemGroup group, Entity self)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<OutT>(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }


        public static async AsyncTask<OutT> CallAsync<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }
        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }

        public static async AsyncTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = await systems.CallAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return outT;
        }


        #endregion

        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsAsync<OutT>(this SystemGroup group, Entity self)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<OutT>(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        public static async AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> values = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                values = await systems.CallsAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
            return values;
        }
        #endregion

    }
}
