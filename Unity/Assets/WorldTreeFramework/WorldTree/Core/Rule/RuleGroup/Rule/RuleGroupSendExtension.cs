
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 法则集合通知执行

*/


namespace WorldTree
{
    public static class RuleGroupSendExtension
    {

        #region TrySend

        public static bool TrySend<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRuleBase
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<R, T1>(this IRuleGroup<R> group, INode node, T1 t1)
        where R : ISendRuleBase<T1>
        {
            if (((RuleGroup)group).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, t1);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRuleBase<T1, T2>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleBase<T1, T2, T3>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleBase<T1, T2, T3, T4>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TrySend<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion



        #region Send






        public static void Send<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRuleBase
        {
            group.TrySend(node);
        }

        public static void Send<R, T1>(this IRuleGroup<R> group, INode node, T1 arg1)
        where R : ISendRuleBase<T1>
        {
            group.TrySend(node, arg1);
        }

        public static void Send<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRuleBase<T1, T2>
        {
            group.TrySend(node, arg1, arg2);
        }
        public static void Send<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleBase<T1, T2, T3>
        {
            group.TrySend(node, arg1, arg2, arg3);
        }
        public static void Send<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleBase<T1, T2, T3, T4>
        {
            group.TrySend(node, arg1, arg2, arg3, arg4);
        }
        public static void Send<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {
            group.TrySend(node, arg1, arg2, arg3, arg4, arg5);
        }


        #endregion


    }
}
