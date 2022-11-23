using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListExtension
    {
        #region Send

        public static void Send(this List<ISystem> systems, Entity self)
        {
            foreach (ISendSystem system in systems)
            {
                system.Invoke(self);
            }
        }

        public static void Send<T1>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            foreach (ISendSystem<T1> system in systems)
            {
                system.Invoke(self, arg1);
            }
        }

        public static void Send<T1, T2>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            foreach (ISendSystem<T1, T2> system in systems)
            {
                system.Invoke(self, arg1, arg2);
            }
        }


        public static void Send<T1, T2, T3>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (ISendSystem<T1, T2, T3> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3);
            }
        }

        public static void Send<T1, T2, T3, T4>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (ISendSystem<T1, T2, T3, T4> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3, arg4);
            }
        }
        public static void Send<T1, T2, T3, T4, T5>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (ISendSystem<T1, T2, T3, T4, T5> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
        }
        #endregion

        #region Call

        public static OutT Call<OutT>(this List<ISystem> systems, Entity self)
        {
            OutT outT = default(OutT);

            foreach (ICallSystem<OutT> system in systems)
            {
                outT = system.Invoke(self);
            }
            return outT;
        }

        public static OutT Call<T1, OutT>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1);
            }
            return outT;
        }

        public static OutT Call<T1, T2, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2, arg3);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
            {
                outT = system.Invoke(self, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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

        public static UnitList<OutT> Calls<OutT>(this List<ISystem> systems, Entity self)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<OutT> system in systems)
            {
                outT.Add(system.Invoke(self));
            }
            return outT;
        }


        public static UnitList<OutT> Calls<T1, OutT>(this List<ISystem> systems, Entity self, T1 arg1)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1));
            }
            return outT;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2, arg3));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> outT = self.PoolGet<UnitList<OutT>>();
            foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
            {
                outT.Add(system.Invoke(self, arg1, arg2, arg3, arg4));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this List<ISystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
