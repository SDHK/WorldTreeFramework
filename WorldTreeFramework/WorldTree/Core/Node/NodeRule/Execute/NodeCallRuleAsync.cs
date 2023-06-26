/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:35

* 描述： 节点法则异步调用执行

*/

namespace WorldTree
{
    public static class NodeCallRuleAsync
    {
        #region Call
        public static async TreeTask<OutT> CallRuleAsync<N, R, OutT>(this N self, R defaultRule, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<OutT>
            => await self.TryCallRuleAsync(defaultRule, defaultOutT);
        public static async TreeTask<OutT> CallRuleAsync<N, R, T1, OutT>(this N self, R defaultRule, T1 arg1, OutT defaultOutT)
           where N : class, INode, AsRule<R>
           where R : ICallRuleAsyncBase<T1, OutT>
           => await self.TryCallRuleAsync(defaultRule, arg1, defaultOutT);
        public static async TreeTask<OutT> CallRuleAsync<N, R, T1, T2, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, OutT>
           => await self.TryCallRuleAsync(defaultRule, arg1, arg2, defaultOutT);
        public static async TreeTask<OutT> CallRuleAsync<N, R, T1, T2, T3, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
          => await self.TryCallRuleAsync(defaultRule, arg1, arg2, arg3, defaultOutT);
        public static async TreeTask<OutT> CallRuleAsync<N, R, T1, T2, T3, T4, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
           => await self.TryCallRuleAsync(defaultRule, arg1, arg2, arg3, arg4, defaultOutT);
        public static async TreeTask<OutT> CallRuleAsync<N, R, T1, T2, T3, T4, T5, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
            => await self.TryCallRuleAsync(defaultRule, arg1, arg2, arg3, arg4, arg5, defaultOutT);



        public static async TreeTask<OutT> TryCallRuleAsync<R, OutT>(this INode self, R defaultRule, OutT defaultOutT)
        where R : ICallRuleAsyncBase<OutT>
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

        public static async TreeTask<OutT> TryCallRuleAsync<R, T1, OutT>(this INode self, R defaultRule, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, OutT>
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
        public static async TreeTask<OutT> TryCallRuleAsync<R, T1, T2, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, OutT>
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
        public static async TreeTask<OutT> TryCallRuleAsync<R, T1, T2, T3, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
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
        public static async TreeTask<OutT> TryCallRuleAsync<R, T1, T2, T3, T4, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
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
        public static async TreeTask<OutT> TryCallRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
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

        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, OutT>(this N self, R defaultRule, OutT defaultOutT)
         where N : class, INode, AsRule<R>
         where R : ICallRuleAsyncBase<OutT>
         => await self.TryCallsRuleAsync(defaultRule, defaultOutT);

        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, T1, OutT>(this N self, R defaultRule, T1 arg1, OutT defaultOutT)
           where N : class, INode, AsRule<R>
           where R : ICallRuleAsyncBase<T1, OutT>
           => await self.TryCallsRuleAsync(defaultRule, arg1, defaultOutT);
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, T1, T2, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, OutT>
           => await self.TryCallsRuleAsync(defaultRule, arg1, arg2, defaultOutT);
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, T1, T2, T3, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
          => await self.TryCallsRuleAsync(defaultRule, arg1, arg2, arg3, defaultOutT);
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, T1, T2, T3, T4, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
           => await self.TryCallsRuleAsync(defaultRule, arg1, arg2, arg3, arg4, defaultOutT);
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<N, R, T1, T2, T3, T4, T5, OutT>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
            where N : class, INode, AsRule<R>
            where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
            => await self.TryCallsRuleAsync(defaultRule, arg1, arg2, arg3, arg4, arg5, defaultOutT);

        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, OutT>(this INode self, R defaultRule, OutT defaultOutT)
        where R : ICallRuleAsyncBase<OutT>
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
        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, T1, OutT>(this INode self, R defaultRule, T1 arg1, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, OutT>
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
        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, T1, T2, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, OutT>
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
        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, T1, T2, T3, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, OutT>
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
        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, T1, T2, T3, T4, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, OutT>
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
        public static async TreeTask<UnitList<OutT>> TryCallsRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT)
        where R : ICallRuleAsyncBase<T1, T2, T3, T4, T5, OutT>
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
