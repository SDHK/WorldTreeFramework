/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:04

* 描述： 法则列表异步调用执行

*/

namespace WorldTree
{
    public static class RuleListCallAsyncExtension
    {

        #region Call

        public static async TreeTask<OutT> CallAsync<R, OutT>(this IRuleList<R> ruleList, INode node, OutT defaultOutT)
            where R : ICallRuleAsyncBase<OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }
        public static async TreeTask<OutT> CallAsync<R, T1, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, OutT defaultOutT)
            where R : ICallRuleAsyncBase<T1, OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<T1, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node, arg1);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, OutT defaultOutT)
            where R : ICallRuleAsyncBase<T1, T2, OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<T1, T2, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node, arg1, arg2);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
            where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<T1, T2, T3, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node, arg1, arg2, arg3);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }
        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<T1, T2, T3, T4, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node, arg1, arg2, arg3, arg4);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
        {
            OutT outT;
            foreach (ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT = await rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
                if (!rule.IsMulticast) return outT;
            }
            return DefaultType<OutT>.Default;
        }


        #endregion


        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsAsync<R, OutT>(this IRuleList<R> ruleList, INode node, OutT defaultOutT)
         where R : ICallRuleAsyncBase<OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, OutT defaultOutT)
         where R : ICallRuleAsyncBase<T1, OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<T1, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node, arg1));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, OutT defaultOutT)
         where R : ICallRuleAsyncBase<T1, T2, OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<T1, T2, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node, arg1, arg2));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
         where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<T1, T2, T3, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node, arg1, arg2, arg3));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
         where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<T1, T2, T3, T4, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node, arg1, arg2, arg3, arg4));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
         where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT> rule in (RuleList)ruleList)
            {
                rule.IsMulticast = true;
                outT.Add(await rule.Invoke(node, arg1, arg2, arg3, arg4, arg5));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }


        #endregion

    }
}
