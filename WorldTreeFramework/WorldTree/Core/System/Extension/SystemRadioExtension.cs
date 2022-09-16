using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public partial class SystemRadio
    {
        public void SendSystem<S>()
        where S : ISendSystem
        {
            if (IsActice)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    if (entity.IsActice)
                    {
                        systems.SendSystem<S>(entity);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }

        }

        public void SendSystem<S, T1>(T1 arg1)
        where S : ISendSystem<T1>
        {
            if (IsActice)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    if (entity.IsActice)
                    {
                        systems.SendSystem<S, T1>(entity, arg1);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }
        }


        public void SendSystem<S, T1, T2>(T1 arg1, T2 arg2)
        where S : ISendSystem<T1, T2>
        {
            if (IsActice)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    if (entity.IsActice)
                    {
                        systems.SendSystem<S, T1, T2>(entity, arg1, arg2);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }
        }
        public void SendSystem<S, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        where S : ISendSystem<T1, T2, T3>
        {
            if (IsActice)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    if (entity.IsActice)
                    {
                        systems.SendSystem<S, T1, T2, T3>(entity, arg1, arg2, arg3);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }
        }
    }
}
