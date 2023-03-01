
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:50

* 描述： 系统组系统事件调用

*/

using System.Collections.Generic;
using System.Linq;

namespace WorldTree
{
    public static class SystemGroupCallExtension
    {
        #region Call
        public static bool TryCall<OutT>(this SystemGroup group, Entity self, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<OutT>(self);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryCall<T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<T1, OutT>(self, arg1);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryCall<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<T1, T2, OutT>(self, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryCall<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryCall<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
        {
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Call<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }



        public static OutT Call<OutT>(this SystemGroup group, Entity self)
        {
            group.TryCall(self, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            group.TryCall(self, arg1, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            group.TryCall(self, arg1, arg2, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TryCall(self, arg1, arg2, arg3, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TryCall(self, arg1, arg2, arg3, arg4, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TryCall(self, arg1, arg2, arg3, arg4, arg5, out OutT outT);
            return outT;
        }

        #endregion



        #region Calls

        public static bool TryCalls<OutT>(this SystemGroup group, Entity self, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<OutT>(self);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryCalls<T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<T1, OutT>(self, arg1);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryCalls<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<T1, T2, OutT>(self, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryCalls<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> outT)
        {
            outT = null;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                outT = systems.Calls<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }





        public static UnitList<OutT> Calls<OutT>(this SystemGroup group, Entity self)
        {
            group.TryCalls(self, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
        {
            group.TryCalls(self, arg1, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            group.TryCalls(self, arg1, arg2, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TryCalls(self, arg1, arg2, arg3, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TryCalls(self, arg1, arg2, arg3, arg4, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TryCalls(self, arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> outT);
            return outT;
        }
        #endregion



    }
}
