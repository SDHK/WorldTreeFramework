/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:04

* 描述： 法则列表异步调用执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleListCallAsyncExtension
    {

        #region Call

        public static async TreeTask<OutT> CallAsync<OutT>(this List<IRule> ruleList, INode node)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node);
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<T1, OutT>(this List<IRule> ruleList, INode node, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<T1, OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node, arg1);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<T1, T2, OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node, arg1, arg2);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<T1, T2, T3, OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node, arg1, arg2, arg3);
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<T1, T2, T3, T4, OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            foreach (ICallRuleAsync<T1, T2, T3, T4, T5, OutT> rule in ruleList)
            {
                outT = await rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
            }
            return outT;
        }

        #endregion


        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsAsync<OutT>(this List<IRule> ruleList, INode node)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this List<IRule> ruleList, INode node, T1 arg1)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<T1, OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node, arg1));
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<T1, T2, OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node, arg1, arg2));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<T1, T2, T3, OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node, arg1, arg2, arg3));
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<T1, T2, T3, T4, OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node, arg1, arg2, arg3, arg4));
            }
            return values;
        }

        public static async TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> values = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleAsync<T1, T2, T3, T4, T5, OutT> rule in ruleList)
            {
                values.Add(await rule.Invoke(node, arg1, arg2, arg3, arg4, arg5));
            }
            return values;
        }

        #endregion

    }
}
