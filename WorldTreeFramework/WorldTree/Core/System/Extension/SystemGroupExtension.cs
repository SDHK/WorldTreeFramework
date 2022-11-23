using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static class SystemGroupExtension
    {

        #region Send

        public static bool TrySend(this SystemGroup group, Entity self)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem system in systems)
                {
                    system.Invoke(self);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TrySend<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem<T1> system in systems)
                {
                    system.Invoke(self, arg1);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TrySend<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem<T1, T2> system in systems)
                {
                    system.Invoke(self, arg1, arg2);
                }
                bit = true;
            }
            return bit;
        }


        public static bool TrySend<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem<T1, T2, T3> system in systems)
                {
                    system.Invoke(self, arg1, arg2, arg3);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TrySend<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem<T1, T2, T3, T4> system in systems)
                {
                    system.Invoke(self, arg1, arg2, arg3, arg4);
                }
                bit = true;
            }
            return bit;
        }
        public static bool TrySend<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ISendSystem<T1, T2, T3, T4, T5> system in systems)
                {
                    system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
                }
                bit = true;
            }
            return bit;
        }


        public static void Send(this SystemGroup group, Entity self)
        {
            group.TrySend(self);
        }

        public static void Send<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            group.TrySend(self, arg1);
        }

        public static void Send<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            group.TrySend(self, arg1, arg2);
        }
        public static void Send<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TrySend(self, arg1, arg2, arg3);
        }
        public static void Send<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TrySend(self, arg1, arg2, arg3, arg4);
        }
        public static void Send<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TrySend(self, arg1, arg2, arg3, arg4, arg5);
        }
        #endregion


        #region Call
        public static bool TryCall<OutT>(this SystemGroup group, Entity self, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<OutT> system in systems)
                {
                    outT = system.Invoke(self);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TryCall<T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, OutT> system in systems)
                {
                    outT = system.Invoke(self, arg1);
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCall<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, OutT> system in systems)
                {
                    outT = system.Invoke(self, arg1, arg2);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TryCall<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
                {
                    outT = system.Invoke(self, arg1, arg2, arg3);
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCall<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
                {
                    outT = system.Invoke(self, arg1, arg2, arg3, arg4);
                }
                bit = true;
            }
            return bit;
        }

        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
        {
            bool bit = false;
            outT = default(OutT);
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, OutT> system in systems)
                {
                    outT = system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
                }
                bit = true;
            }
            return bit;
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
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<OutT> system in systems)
                {
                    outT.Add(system.Invoke(self));
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCalls<T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out UnitList<OutT> outT)
        {
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, OutT> system in systems)
                {
                    outT.Add(system.Invoke(self, arg1));
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCalls<T1, T2, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, out UnitList<OutT> outT)
        {
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, OutT> system in systems)
                {
                    outT.Add(system.Invoke(self, arg1, arg2));
                }
                bit = true;
            }
            return bit;
        }

        public static bool TryCalls<T1, T2, T3, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> outT)
        {
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, OutT> system in systems)
                {
                    outT.Add(system.Invoke(self, arg1, arg2, arg3));
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> outT)
        {
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, OutT> system in systems)
                {
                    outT.Add(system.Invoke(self, arg1, arg2, arg3, arg4));
                }
                bit = true;
            }
            return bit;
        }

        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> outT)
        {
            bool bit = false;
            outT = self.PoolGet<UnitList<OutT>>();
            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, OutT> system in systems)
                {
                    outT.Add(system.Invoke(self, arg1, arg2, arg3, arg4, arg5));
                }
                bit = true;
            }
            return bit;
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
