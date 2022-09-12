using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static class SystemGroupExtension
    {
        public static bool TrySendSystem<S>(this SystemGroup group, Entity self)
        where S : ISendSystem
        {
            bool bit = false;
            if (group.systemType == typeof(S))
            {
                if (group.TryGetValue(self.Type, out List<ISystem> sendSystems))
                {
                    foreach (ISendSystem sendSystem in sendSystems)
                    {
                        sendSystem.Invoke(self);
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
                if (group.TryGetValue(self.Type, out List<ISystem> sendSystems))
                {

                    foreach (ISendSystem<T1> sendSystem in sendSystems)
                    {
                        sendSystem.Invoke(self, arg1);
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
                if (group.TryGetValue(self.Type, out List<ISystem> sendSystems))
                {
                    foreach (ISendSystem<T1, T2> sendSystem in sendSystems)
                    {
                        sendSystem.Invoke(self, arg1, arg2);
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
    }
}
