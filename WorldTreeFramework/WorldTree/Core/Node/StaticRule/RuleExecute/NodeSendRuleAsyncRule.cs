/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点法则异步通知执行

*/

namespace WorldTree
{
    public static class NodeSendRuleAsyncRule
    {
        #region Send

        public static async TreeTask<bool> TrySendRuleAsync<R>(this INode self, R defaultRule)
        where R : ISendRuleAsync
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1>(this INode self, R defaultRule, T1 arg1)
        where R : ISendRuleAsync<T1>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2>(this INode self, R defaultRule, T1 arg1, T2 arg2)
        where R : ISendRuleAsync<T1, T2>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleAsync<T1, T2, T3>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3, T4>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3, T4, T5>(this INode self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.TreeTaskCompleted();
                return false;
            }
        }





        public static async TreeTask SendRuleAsync<N, R>(this N self, R defaultRule)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.SendAsync(self);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }

        public static async TreeTask SendRuleAsync<N, R, T1>(this N self, R defaultRule, T1 arg1)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync<T1>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.SendAsync(self, arg1);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }

        public static async TreeTask SendRuleAsync<N, R, T1, T2>(this N self, R defaultRule, T1 arg1, T2 arg2)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync<T1, T2>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<N, R, T1, T2, T3>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync<T1, T2, T3>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<N, R, T1, T2, T3, T4>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<N, R, T1, T2, T3, T4, T5>(this N self, R defaultRule, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where N : class, INode, AsRule<R>
            where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if (self.Core.RuleManager.TryGetRuleGroup(out IRuleGroup<R> group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.TreeTaskCompleted();
            }
        }
        #endregion

    }
}
