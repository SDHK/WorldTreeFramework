
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/22 15:21

* 描述： 法则列表异步通知执行

*/


namespace WorldTree
{
    public static class RuleListSendAsyncExtension
    {
        #region Send


        public static async TreeTask SendAsync<R>(this IRuleList<R> ruleList, INode node)
            where R : ISendRuleAsyncBase
        {
            foreach (ISendRuleAsyncBase rule in ruleList as RuleList)
            {
                await rule.Invoke(node);
            }
        }

        public static async TreeTask SendAsync<R, T1>(this IRuleList<R> ruleList, INode node, T1 arg1)
            where R : ISendRuleAsyncBase<T1>
        {
            foreach (ISendRuleAsyncBase<T1> rule in ruleList as RuleList)
            {
                await rule.Invoke(node, arg1);
            }
        }

        public static async TreeTask SendAsync<R, T1, T2>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2)
            where R : ISendRuleAsyncBase<T1, T2>
        {
            foreach (ISendRuleAsyncBase<T1, T2> rule in ruleList as RuleList)
            {
                await rule.Invoke(node, arg1, arg2);
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
            where R : ISendRuleAsyncBase<T1, T2, T3>
        {
            foreach (ISendRuleAsyncBase<T1, T2, T3> rule in ruleList as RuleList)
            {
                await rule.Invoke(node, arg1, arg2, arg3);
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3, T4>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ISendRuleAsyncBase<T1, T2, T3, T4>
        {
            foreach (ISendRuleAsyncBase<T1, T2, T3, T4> rule in ruleList as RuleList)
            {
                await rule.Invoke(node, arg1, arg2, arg3, arg4);
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3, T4, T5>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ISendRuleAsyncBase<T1, T2, T3, T4, T5>
        {
            foreach (ISendRuleAsyncBase<T1, T2, T3, T4, T5> rule in ruleList as RuleList)
            {
                await rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
            }
        }

        #endregion

    }
}
