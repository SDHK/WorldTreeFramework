/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:35

* 描述： 节点法则异步调用执行

*/

namespace WorldTree
{
    public static class CallRuleAsyncExtension
    {
        #region Call
        public static async TreeTask<OutT> CallRuleAsync<R, OutT>(this INode self, R defaultRule, OutT defaultOutT)
        where R : ICallRuleAsync<OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, OutT>(this INode self, R defaultRule, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsync<T1, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, T4, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }

        #endregion


        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, OutT>(this INode self, R defaultRule, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, OutT>(this INode self, R defaultRule, T1 arg1, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, arg1, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, arg1, arg2, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, arg1, arg2, arg3, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, T4, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, arg1, arg2, arg3, arg4, defaultOutT);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, UnitList<OutT> defaultOutT)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5, OutT>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.CallsAsync(self, arg1, arg2, arg3, arg4, arg5, defaultOutT);
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
