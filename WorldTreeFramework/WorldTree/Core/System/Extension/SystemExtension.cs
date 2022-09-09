using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static class SystemExtension
    {

        #region Send

        public static bool TrySendSystem<S>(this Entity self)
          where S : ISendSystem
        {
            bool bit = self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems);
            if (bit)
            {
                foreach (ISendSystem sendSystem in sendSystems)
                {
                    sendSystem.Invoke(self);
                    bit = true;
                }
            }
            return bit;
        }
        public static bool TrySendSystem<S, T1>(this Entity self, T1 arg1)
          where S : ISendSystem<T1>
        {
            bool bit = self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems);
            if (bit)
            {
                foreach (ISendSystem<T1> sendSystem in sendSystems)
                {
                    sendSystem.Invoke(self, arg1);
                    bit = true;
                }
            }
            return bit;
        }
        public static bool TrySendSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
          where S : ISendSystem<T1, T2>
        {
            bool bit = self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems);
            if (bit)
            {
                foreach (ISendSystem<T1, T2> sendSystem in sendSystems)
                {
                    sendSystem.Invoke(self, arg1, arg2);
                    bit = true;
                }
            }
            return bit;
        }


        public static void SendSystem<S>(this Entity self)
            where S : ISendSystem
        {
            self.TrySendSystem<S>();
        }

        public static void SendSystem<S, T1>(this Entity self, T1 arg1)
           where S : ISendSystem<T1>
        {
            self.TrySendSystem<S, T1>(arg1);
        }

        public static void SendSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
           where S : ISendSystem<T1, T2>
        {
            self.TrySendSystem<S, T1, T2>(arg1, arg2);
        }

        #endregion

        #region Call


        public static bool TryCallSystem<S, OutT>(this Entity self, out OutT outT)
          where S : ICallSystem<OutT>
        {
            bool bit = false;
            if (self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems))
            {
                foreach (ICallSystem<OutT> sendSystem in sendSystems)
                {
                    outT = sendSystem.Invoke(self);
                }
                bit = true;
            }
            outT = default(OutT);
            return bit;
        }
        public static bool TryCallSystem<S, T1, OutT>(this Entity self, T1 arg1, out OutT outT)
          where S : ICallSystem<T1, OutT>
        {
            bool bit = false;
            if (self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems))
            {
                foreach (ICallSystem<T1, OutT> sendSystem in sendSystems)
                {
                    outT = sendSystem.Invoke(self, arg1);
                }
                bit = true;
            }
            outT = default(OutT);
            return bit;
        }


        public static OutT CallSystem<S, OutT>(this Entity self)
          where S : ICallSystem<OutT>
        {
            return self.TryCallSystem<S, OutT>(out OutT outT) ? outT : default(OutT);
        }
        public static OutT CallSystem<S, T1, OutT>(this Entity self, T1 arg1)
          where S : ICallSystem<T1, OutT>
        {
            return self.TryCallSystem<S, T1, OutT>(arg1, out OutT outT) ? outT : default(OutT);
        }



        #endregion


        #region Calls
        public static bool TryCallsSystem<S, OutT>(this Entity self, out UnitList<OutT> values)
        where S : ICallSystem<OutT>
        {
            values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            bool bit = false;
            if (self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems))
            {
                foreach (ICallSystem<OutT> sendSystem in sendSystems)
                {
                    values.Add(sendSystem.Invoke(self));
                }
                bit = true;
            }
            return bit;
        }
        public static bool TryCallsSystem<S, T1, OutT>(this Entity self, T1 arg1, out UnitList<OutT> values)
        where S : ICallSystem<T1, OutT>
        {
            values = self.Root.ObjectPoolManager.Get<UnitList<OutT>>();
            bool bit = false;
            if (self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems))
            {
                foreach (ICallSystem<T1, OutT> sendSystem in sendSystems)
                {
                    values.Add(sendSystem.Invoke(self, arg1));
                }
                bit = true;
            }
            return bit;
        }


        public static UnitList<OutT> CallsSystem<S, OutT>(this Entity self)
        where S : ICallSystem<OutT>
        {
            self.TryCallsSystem<S, OutT>(out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsSystem<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, OutT>
        {
            self.TryCallsSystem<S, T1, OutT>(arg1, out UnitList<OutT> outT);
            return outT;
        }

        #endregion


    }


}
