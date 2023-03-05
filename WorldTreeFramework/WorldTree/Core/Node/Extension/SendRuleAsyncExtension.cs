﻿/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 节点法则异步通知执行

*/

namespace WorldTree
{
    public static class SendRuleAsyncExtension
    {
        #region Send

        public static async TreeTask<bool> TrySendRuleAsync<R>(this Node self)
        where R : ISendRuleAsync
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1>(this Node self, T1 arg1)
        where R : ISendRuleAsync<T1>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2>(this Node self, T1 arg1, T2 arg2)
        where R : ISendRuleAsync<T1, T2>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleAsync<T1, T2, T3>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendRuleAsync<R, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }


        public static async TreeTask SendRuleAsync<R>(this Node self)
        where R : ISendRuleAsync
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.SendAsync(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }

        public static async TreeTask SendRuleAsync<R, T1>(this Node self, T1 arg1)
        where R : ISendRuleAsync<T1>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.SendAsync(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }

        public static async TreeTask SendRuleAsync<R, T1, T2>(this Node self, T1 arg1, T2 arg2)
         where R : ISendRuleAsync<T1, T2>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<R, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleAsync<T1, T2, T3>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<R, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
         where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendRuleAsync<R, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
         where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if (self.Root.RuleManager.TryGetRuleGroup<R>(out RuleGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        #endregion

    }
}