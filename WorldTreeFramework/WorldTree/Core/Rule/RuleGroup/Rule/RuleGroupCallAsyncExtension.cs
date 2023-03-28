/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:57

* 描述： 法则集合异步调用执行

*/

namespace WorldTree
{
    public static class RuleGroupCallAsyncExtension
    {


        #region Call

        public static async TreeTask<OutT> CallAsync<R, OutT>(this IRuleGroup<R> group, INode self, OutT defaultOutT)
        where R : ICallRuleAsync<OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }


        public static async TreeTask<OutT> CallAsync<R, T1, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsync<T1, OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }
        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
        {
            OutT outT = default(OutT);
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                outT = await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return outT;
        }


        #endregion

        #region Calls


        public static async TreeTask<UnitList<OutT>> CallsAsync<R, OutT>(this IRuleGroup<R> group, INode self, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
        {
            UnitList<OutT> values = null;
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                values = await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
            return values;
        }

        #endregion

    }
}
