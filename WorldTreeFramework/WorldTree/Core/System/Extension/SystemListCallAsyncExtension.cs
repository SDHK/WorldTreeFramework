/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:04

* 描述： 系统列表系统事件异步调用

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListCallAsyncExtension
    {

        #region Call

        public static async TreeTask<OutT> CallAsync<OutT>(this List<IRule> systems, Node self)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<OutT> system in systems)
            {
                outT = await system.Invoke(self);
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<T1, OutT>(this List<IRule> systems, Node self, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<T1, OutT> system in systems)
            {
                outT = await system.Invoke(self, arg1);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<T1, T2, OutT> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<T1, T2, T3, OutT> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3);
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<T1, T2, T3, T4, OutT> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            foreach (ICallSystemAsync<T1, T2, T3, T4, T5, OutT> system in systems)
            {
                outT = await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
            return outT;
        }

        #endregion


        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsAsync<OutT>(this List<IRule> systems, Node self)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<OutT> system in systems)
            {
                values.Add(await system.Invoke(self));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this List<IRule> systems, Node self, T1 arg1)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<T1, OutT> system in systems)
            {
                values.Add(await system.Invoke(self, arg1));
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<T1, T2, OutT> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<T1, T2, T3, OutT> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3));
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<T1, T2, T3, T4, OutT> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> values = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystemAsync<T1, T2, T3, T4, T5, OutT> system in systems)
            {
                values.Add(await system.Invoke(self, arg1, arg2, arg3, arg4, arg5));
            }
            return values;
        }

        #endregion

    }
}
