
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点法则通知执行

*/

namespace WorldTree
{
    public static class SendRuleExtension
    {
        #region Send

        public static bool TrySendRule<R>(this INode self)
        where R : ISendRule
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1>(this INode self, T1 arg1)
        where R : ISendRule<T1>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self, arg1);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2>(this INode self, T1 arg1, T2 arg2)
        where R : ISendRule<T1, T2>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRule<T1, T2, T3>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRule<T1, T2, T3, T4>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRule<T1, T2, T3, T4, T5>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                return false;
            }
        }


        public static void SendRule<R>(this INode self)
            where R : ISendRule
        {
            self.TrySendRule<R>();
        }
        public static void SendRule<R, T1>(this INode self, T1 arg1)
           where R : ISendRule<T1>
        {
            self.TrySendRule<R, T1>(arg1);
        }
        public static void SendRule<R, T1, T2>(this INode self, T1 arg1, T2 arg2)
           where R : ISendRule<T1, T2>
        {
            self.TrySendRule<R, T1, T2>(arg1, arg2);
        }
        public static void SendRule<R, T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3)
           where R : ISendRule<T1, T2, T3>
        {
            self.TrySendRule<R, T1, T2, T3>(arg1, arg2, arg3);
        }
        public static void SendRule<R, T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
         where R : ISendRule<T1, T2, T3, T4>
        {
            self.TrySendRule<R, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        }
        public static void SendRule<R, T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
         where R : ISendRule<T1, T2, T3, T4, T5>
        {
            self.TrySendRule<R, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        }
        #endregion

    }


}
