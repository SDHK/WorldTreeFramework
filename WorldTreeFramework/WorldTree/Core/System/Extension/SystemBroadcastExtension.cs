/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/
using System.Linq;

namespace WorldTree
{
    public partial class SystemBroadcast
    {
        public void SendSystem()
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
                        systems.Send(entity);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }

        }

        public void SendSystem<T1>(T1 arg1)
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
                        systems.Send(entity, arg1);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }
        }


        public void SendSystem<T1, T2>(T1 arg1, T2 arg2)
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
                        systems.Send(entity, arg1, arg2);
                    }
                    update1.Remove(firstKey);
                    if (!entity.IsRecycle)
                    {
                        update2.Add(firstKey, entity);
                    }
                }
            }
        }
        public void SendSystem<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
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
                        systems.Send(entity, arg1, arg2, arg3);
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
