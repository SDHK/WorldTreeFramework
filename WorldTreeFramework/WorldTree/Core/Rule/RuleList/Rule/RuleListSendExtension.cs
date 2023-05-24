
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/22 14:27

* 描述： 法则列表通知执行

*/


namespace WorldTree
{
    public static class RuleListSendExtension
    {
        #region Send

        public static void Send<R>(this IRuleList<R> ruleList, INode node)
            where R : ISendRuleBase
        {
            foreach (ISendRuleBase rule in ruleList as RuleList)
            {
                rule.Invoke(node);
            }
        }

        public static void Send<R, T1>(this IRuleList<R> ruleList, INode node, T1 arg1)
            where R : ISendRuleBase<T1>
        {
            foreach (ISendRuleBase<T1> rule in ruleList as RuleList)
            {
                rule.Invoke(node, arg1);
            }
        }


        public static void Send<R, T1, T2>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2)
            where R : ISendRuleBase<T1, T2>
        {
            foreach (ISendRuleBase<T1, T2> rule in ruleList as RuleList)
            {
                rule.Invoke(node, arg1, arg2);
            }
        }
        public static void Send<R, T1, T2, T3>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
            where R : ISendRuleBase<T1, T2, T3>
        {
            foreach (ISendRuleBase<T1, T2, T3> rule in ruleList as RuleList)
            {
                rule.Invoke(node, arg1, arg2, arg3);
            }
        }

        public static void Send<R, T1, T2, T3, T4>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ISendRuleBase<T1, T2, T3, T4>
        {
            foreach (ISendRuleBase<T1, T2, T3, T4> rule in ruleList as RuleList)
            {
                rule.Invoke(node, arg1, arg2, arg3, arg4);
            }
        }

        public static void Send<R, T1, T2, T3, T4, T5>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {
            foreach (ISendRuleBase<T1, T2, T3, T4, T5> rule in ruleList as RuleList)
            {
                rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
            }
        }

        #endregion

    }
}
