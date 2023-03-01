
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

        public static async AsyncTask<bool> TrySendAsync(this SystemGroup group, Entity self)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<AsyncTask> system in systems)
                {
                    await system.Invoke(self);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<T1, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<T1, T2, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            bool bit = false;
            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3, arg4);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<IEntitySystem> systems))
            {
                foreach (ICallSystem<T1, T2, T3, T4, T5, AsyncTask> system in systems)
                {
                    await system.Invoke(self, arg1, arg2, arg3, arg4, arg5);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncTaskCompleted();
            }
            return bit;
        }



        public static async void SendAsync(this SystemGroup group, Entity self)
        {
            await group.TrySendAsync(self);
        }
        public static async void SendAsync<T1>(this SystemGroup group, Entity self, T1 arg1)
        {
            await group.TrySendAsync(self, arg1);
        }
        public static async void SendAsync<T1, T2>(this SystemGroup group, Entity self, T1 arg1, T2 arg2)
        {
            await group.TrySendAsync(self, arg1, arg2);
        }
        public static async void SendAsync<T1, T2, T3>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3);
        }
        public static async void SendAsync<T1, T2, T3, T4>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
        }
        public static async void SendAsync<T1, T2, T3, T4, T5>(this SystemGroup group, Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
        }
        #endregion


    }
}
