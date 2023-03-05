
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 17:43

* 描述： 系统组系统事件异步发送

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class SystemGroupSendAsyncExtension
    {

        #region Send

        public static async TreeTask<bool> TrySendAsync(this RuleGroup group, Node self)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<T1>(this RuleGroup group, Node self, T1 arg1)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<T1, T2>(this RuleGroup group, Node self, T1 arg1, T2 arg2)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<T1, T2, T3>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<T1, T2, T3, T4>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }



        public static async TreeTask SendAsync(this RuleGroup group, Node self)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<T1>(this RuleGroup group, Node self, T1 arg1)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<T1, T2>(this RuleGroup group, Node self, T1 arg1, T2 arg2)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<T1, T2, T3>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<T1, T2, T3, T4>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<T1, T2, T3, T4, T5>(this RuleGroup group, Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (group.TryGetValue(self.Type, out List<IRule> systems))
            {
                await systems.SendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        #endregion


    }
}
