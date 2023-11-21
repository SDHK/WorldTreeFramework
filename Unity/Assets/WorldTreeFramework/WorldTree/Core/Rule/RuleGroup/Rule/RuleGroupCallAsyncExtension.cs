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
        where R : ICallRuleAsyncBase<OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }


        public static async TreeTask<OutT> CallAsync<R, T1, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }
        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }

        public static async TreeTask<OutT> CallAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return TypeInfo<OutT>.Default;
            }
        }


        #endregion

        #region Calls


        public static async TreeTask<UnitList<OutT>> CallsAsync<R, OutT>(this IRuleGroup<R> group, INode self, OutT defaultOutT)
        where R : ICallRuleAsyncBase<OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsAsync<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
        {
            if ((group as RuleGroup).TryGetValue(self.Type, out RuleList ruleList))
            {
                return await ((IRuleList<R>)ruleList).CallsAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }

        #endregion

    }
}
