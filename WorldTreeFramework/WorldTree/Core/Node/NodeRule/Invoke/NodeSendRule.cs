
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点法则通知执行

*/

namespace WorldTree
{
    public static class NodeSendRule
    {
        #region Send

        public static void SendRule<N, R>(this N self, R defaultRule)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase
        {
            self.TrySendRule(defaultRule);
        }
        public static void SendRule<N, R, T1>(this N self, R defaultRule, T1 arg1)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase<T1>
        {
            self.TrySendRule(defaultRule, arg1);

        }
        public static void SendRule<N, R, T1, T2>(this N self, R defaultRule, T1 arg1, T2 arg2)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase<T1, T2>
        {
            self.TrySendRule(defaultRule, arg1, arg2);

        }
        public static void SendRule<N, R, T1, T2, T3>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase<T1, T2, T3>
        {
            self.TrySendRule(defaultRule, arg1, arg2, arg3);

        }
        public static void SendRule<N, R, T1, T2, T3, T4>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase<T1, T2, T3, T4>
        {
            self.TrySendRule(defaultRule, arg1, arg2, arg3, arg4);
        }
        public static void SendRule<N, R, T1, T2, T3, T4, T5>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where N : class, INode, AsRule<R>
            where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {
            self.TrySendRule(defaultRule, arg1, arg2, arg3, arg4, arg5);
        }



        public static bool TrySendRule<R>(this INode self, R defaultRule = default)
            where R : ISendRuleBase
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1>(this INode self, R defaultRule, T1 arg1)
            where R : ISendRuleBase<T1>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self, arg1);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2>(this INode self, R defaultRule, T1 arg1, T2 arg2)
            where R : ISendRuleBase<T1, T2>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self, arg1, arg2);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3)
            where R : ISendRuleBase<T1, T2, T3>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self, arg1, arg2, arg3);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3, T4>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ISendRuleBase<T1, T2, T3, T4>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                return false;
            }
        }
        public static bool TrySendRule<R, T1, T2, T3, T4, T5>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {

            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return group.TrySend(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                return false;
            }
        }
        #endregion

    }


}
