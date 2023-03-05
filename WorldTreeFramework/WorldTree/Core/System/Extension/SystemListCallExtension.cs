/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:01

* 描述： 系统列表系统事件调用

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListCallExtension
    {


        #region Call

        public static OutT Call<OutT>(this List<IRule> systems, Node self)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<OutT> system in systems)
            {
                outT = system.Invoke(self);
            }
            return outT;
        }

        public static OutT Call<T1, OutT>(this List<IRule> systems, Node self, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1);
            }
            return outT;
        }

        public static OutT Call<T1, T2, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2, arg3);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, T4, T5, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
            return outT;
        }

        #endregion


        #region Calls

        public static UnitList<OutT> Calls<OutT>(this List<IRule> systems, Node self)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<OutT> system in systems)
            {
                outT.Add(system.Invoke(self));
            }
            return outT;
        }


        public static UnitList<OutT> Calls<T1, OutT>(this List<IRule> systems, Node self, T1 arg1)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1));
            }
            return outT;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2, arg3));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2, arg3, arg4));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this List<IRule> systems, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, T4, T5, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2, arg3, arg4, arg5));
            }
            return outT;
        }

        #endregion

    }
}
