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

        public static bool TrySendSystem<S>(this SystemGroup group, Entity self)
        where S : ISendSystem
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ISendSystem system in systems)
                    {
                        system.Invoke(self);
                    }
                    bit = true;
                }
            }
            return bit;
        }

        public static bool TrySendSystem<S, T1>(this SystemGroup group, Entity self, T1 arg1)
        where S : ISendSystem<T1>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ISendSystem<T1> system in systems)
                    {
                        system.Invoke(self, arg1);
                    }
                    bit = true;
                }
            }
            return bit;
        }

        public static bool TrySendSystem<S, T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
         where S : ISendSystem<T1, T2>
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ISendSystem<T1, T2> system in systems)
                    {
                        system.Invoke(self, arg1, arg2);
                    }
                    bit = true;
                }
            }
            return bit;
        }


        public static void SendSystem<S>(this SystemGroup group, Entity self)
          where S : ISendSystem
        {
            group.TrySendSystem<S>(self);
        }

        public static void SendSystem<S, T1>(this SystemGroup group, Entity self, T1 arg1)
          where S : ISendSystem<T1>
        {
            group.TrySendSystem<S, T1>(self, arg1);
        }

        public static void SendSystem<S, T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
           where S : ISendSystem<T1, T2>
        {
            group.TrySendSystem<S, T1, T2>(self, arg1, arg2);
        }
        #endregion


        #region Call
        public static bool TryCallSystem<S, OutT>(this SystemGroup group, Entity self, out OutT outT)
          where S : ICallSystem<OutT>
        {
            bool bit = false;
            outT = default(OutT);
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ICallSystem<OutT> system in systems)
                    {
                        outT = system.Invoke(self);
                    }
                    bit = true;
                }
            }
            return bit;
        }

        public static bool TryCallSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out OutT outT)
          where S : ICallSystem<T1, OutT>
        {
            bool bit = false;
            outT = default(OutT);
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ICallSystem<T1, OutT> system in systems)
                    {
                        outT = system.Invoke(self, arg1);
                    }
                    bit = true;
                }
            }
            return bit;
        }



        public static OutT CallSystem<S, OutT>(this SystemGroup group, Entity self)
        where S : ICallSystem<OutT>
        {
            group.TryCallSystem<S, OutT>(self, out OutT outT);
            return outT;
        }

        public static OutT CallSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
          where S : ICallSystem<T1, OutT>
        {
            group.TryCallSystem<S, T1, OutT>(self, arg1, out OutT outT);
            return outT;
        }
        #endregion



        #region Calls

        public static bool TryCallsSystem<S, OutT>(this SystemGroup group, Entity self, out UnitList<OutT> values)
        where S : ICallSystem<OutT>
        {
            bool bit = false;
            values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ICallSystem<OutT> system in systems)
                    {
                        values.Add(system.Invoke(self));
                    }
                    bit = true;
                }
            }
            return bit;
        }
        public static bool TryCallsSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1, out UnitList<OutT> values)
        where S : ICallSystem<T1, OutT>
        {
            bool bit = false;
            values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> systems))
                {
                    foreach (ICallSystem<T1, OutT> system in systems)
                    {
                        values.Add(system.Invoke(self, arg1));
                    }
                    bit = true;
                }
            }
            return bit;
        }


        public static UnitList<OutT> CallsSystem<S, OutT>(this SystemGroup group, Entity self)
        where S : ICallSystem<OutT>
        {
            group.TryCallsSystem<S, OutT>(self, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsSystem<S, T1, OutT>(this SystemGroup group, Entity self, T1 arg1)
       where S : ICallSystem<T1, OutT>
        {
            group.TryCallsSystem<S, T1, OutT>(self, arg1, out UnitList<OutT> outT);
            return outT;
        }
        #endregion

    }
}
