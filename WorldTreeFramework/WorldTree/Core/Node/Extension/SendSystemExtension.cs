
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 实体系统事件发送

*/

namespace WorldTree
{
    public static class SendSystemExtension
    {
        #region Send

        public static bool TrySendSystem<S>(this Node self)
        where S : ISendRule
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1>(this Node self, T1 arg1)
        where S : ISendRule<T1>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self, arg1);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1, T2>(this Node self, T1 arg1, T2 arg2)
        where S : ISendRule<T1, T2>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where S : ISendRule<T1, T2, T3>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where S : ISendRule<T1, T2, T3, T4>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendSystem<S, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where S : ISendRule<T1, T2, T3, T4, T5>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                return false;
            }
        }


        public static void SendSystem<S>(this Node self)
            where S : ISendRule
        {
            self.TrySendSystem<S>();
        }
        public static void SendSystem<S, T1>(this Node self, T1 arg1)
           where S : ISendRule<T1>
        {
            self.TrySendSystem<S, T1>(arg1);
        }
        public static void SendSystem<S, T1, T2>(this Node self, T1 arg1, T2 arg2)
           where S : ISendRule<T1, T2>
        {
            self.TrySendSystem<S, T1, T2>(arg1, arg2);
        }
        public static void SendSystem<S, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
           where S : ISendRule<T1, T2, T3>
        {
            self.TrySendSystem<S, T1, T2, T3>(arg1, arg2, arg3);
        }
        public static void SendSystem<S, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
         where S : ISendRule<T1, T2, T3, T4>
        {
            self.TrySendSystem<S, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        }
        public static void SendSystem<S, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
         where S : ISendRule<T1, T2, T3, T4, T5>
        {
            self.TrySendSystem<S, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        }
        #endregion

    }


}
