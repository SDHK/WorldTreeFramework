
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/22 14:27

* 描述： 系统列表系统事件发送

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemListSendExtension
    {
        #region Send

        public static void Send(this List<IEntitySystem> systems, Entity self)
        {
            foreach (ISendSystem system in systems)
            {
                system.Invoke(self);
            }
        }

        public static void Send<T1>(this List<IEntitySystem> systems, Entity self, T1 arg1)
        {
            foreach (ISendSystem<T1> system in systems)
            {
                system.Invoke(self, arg1);
            }
        }

        public static void Send<T1, T2>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2)
        {
            foreach (ISendSystem<T1, T2> system in systems)
            {
                system.Invoke(self, arg1, arg2);
            }
        }


        public static void Send<T1, T2, T3>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (ISendSystem<T1, T2, T3> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3);
            }
        }

        public static void Send<T1, T2, T3, T4>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (ISendSystem<T1, T2, T3, T4> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3, arg4);
            }
        }
        public static void Send<T1, T2, T3, T4, T5>(this List<IEntitySystem> systems, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (ISendSystem<T1, T2, T3, T4, T5> system in systems)
            {
                system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
            }
        }
        #endregion

    }
}
