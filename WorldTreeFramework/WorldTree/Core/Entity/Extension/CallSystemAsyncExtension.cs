/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:35

* 描述： 实体系统事件异步调用

*/

namespace WorldTree
{
    public static class CallSystemAsyncExtension
    {
        #region Call
        public static async AsyncTask<OutT> CallSystemAsync<S, OutT>(this Entity self)
        where S : ICallSystem<AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<OutT>(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }
        public static async AsyncTask<OutT> CallSystemAsync<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }
        public static async AsyncTask<OutT> CallSystemAsync<S, T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2)
        where S : ICallSystem<T1, T2, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }
        public static async AsyncTask<OutT> CallSystemAsync<S, T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }
        public static async AsyncTask<OutT> CallSystemAsync<S, T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where S : ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }
        public static async AsyncTask<OutT> CallSystemAsync<S, T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where S : ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return default(OutT);
            }
        }

        #endregion


        #region Calls

        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, OutT>(this Entity self)
        where S : ICallSystem<AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<OutT>(self);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2)
        where S : ICallSystem<T1, T2, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<T1, T2, OutT>(self, arg1, arg2);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, OutT>(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where S : ICallSystem<T1, T2, T3, T4, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, T4, OutT>(self, arg1, arg2, arg3, arg4);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        public static async AsyncTask<UnitList<OutT>> CallsSystemAsync<S, T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where S : ICallSystem<T1, T2, T3, T4, T5, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsync<T1, T2, T3, T4, T5, OutT>(self, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await self.AsyncTaskCompleted();
                return null;
            }
        }
        #endregion
    }
}
