/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 16:55

* 描述： 

*/
using System.Linq;

namespace WorldTree
{
    public partial class SystemActuator
    {

        public void Send()
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }

        public void Send<T1>(T1 arg1)
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity, arg1);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }


        public void Send<T1, T2>(T1 arg1, T2 arg2)
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity, arg1, arg2);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }
        public void Send<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity, arg1, arg2, arg3);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }
        public void Send<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity, arg1, arg2, arg3, arg4);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }
        public void Send<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (IsActive)
            {
                (update1, update2) = (update2, update1);
                while (update1.Count != 0)
                {
                    long firstKey = update1.Keys.First();
                    Entity entity = update1[firstKey];
                    systems.Send(entity, arg1, arg2, arg3, arg4, arg5);
                    update1.Remove(firstKey);
                }
                Dispose();
            }
        }


    }
}
