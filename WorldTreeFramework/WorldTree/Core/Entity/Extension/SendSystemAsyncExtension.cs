/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 实体系统事件异步发送

*/

namespace WorldTree
{
    public static class SendSystemAsyncExtension
    {
        #region Send

        public static async AsyncTask<bool> TrySendSystemAsync<S>(this Entity self)
        where S : ISendSystemAsync
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async AsyncTask<bool> TrySendSystemAsync<S, T1>(this Entity self, T1 arg1)
        where S : ISendSystemAsync<T1>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async AsyncTask<bool> TrySendSystemAsync<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
        where S : ISendSystemAsync<T1, T2>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async AsyncTask<bool> TrySendSystemAsync<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ISendSystemAsync<T1, T2, T3>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }

        public static async AsyncTask<bool> TrySendSystemAsync<S, T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where S : ISendSystemAsync<T1, T2, T3, T4>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }
        public static async AsyncTask<bool> TrySendSystemAsync<S, T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where S : ISendSystemAsync<T1, T2, T3, T4, T5>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsync(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return false;
            }
        }


        public static async AsyncTask SendSystemAsync<S>(this Entity self)
        where S : ISendSystemAsync
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                await group.SendAsync(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }

        public static async AsyncTask SendSystemAsync<S, T1>(this Entity self, T1 arg1)
        where S : ISendSystemAsync<T1>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                await group.SendAsync(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }

        public static async AsyncTask SendSystemAsync<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
         where S : ISendSystemAsync<T1, T2>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async AsyncTask SendSystemAsync<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ISendSystemAsync<T1, T2, T3>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async AsyncTask SendSystemAsync<S, T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
         where S : ISendSystemAsync<T1, T2, T3, T4>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                await group.TrySendAsync(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
            }
        }
        public static async AsyncTask SendSystemAsync<S, T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
         where S : ISendSystemAsync<T1, T2, T3, T4, T5>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
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
