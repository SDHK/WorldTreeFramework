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
        public static async TreeTask<OutT> CallRuleAsync<R, OutT>(this Node self)
        where R : ICallRuleAsync<OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<OutT>(self);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, OutT>(this Node self, T1 arg1)
        where R : ICallRuleAsync<T1,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2)
        where R : ICallRuleAsync<T1, T2,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ICallRuleAsync<T1, T2, T3,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ICallRuleAsync<T1, T2, T3, T4,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }
        public static async TreeTask<OutT> CallRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.TreeTaskCompleted();
                return default(OutT);
            }
        }

        #endregion


        #region Calls

        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, OutT>(this Node self)
        where R : ICallRuleAsync<OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<OutT>(self);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, OutT>(this Node self, T1 arg1)
        where R : ICallRuleAsync<T1,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2)
        where R : ICallRuleAsync<T1, T2,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ICallRuleAsync<T1, T2, T3,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ICallRuleAsync<T1, T2, T3, T4,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
                return null;
            }
        }
        public static async TreeTask<UnitList<OutT>> CallsRuleAsync<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ICallRuleAsync<T1, T2, T3, T4, T5,OutT>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
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
