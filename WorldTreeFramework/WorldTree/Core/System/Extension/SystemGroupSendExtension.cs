
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 系统组系统事件发送

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupSendExtension
    {

        #region Send

        public static bool TrySend(this SystemGroup group, Entity self)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self, arg1);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TrySend<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                systems.Send(self, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
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


    }
}
