namespace WorldTree
{
    public static class SystemAsyncExtension
    {
        #region Send

        public static async AsyncTask<bool> TrySendAsyncSystem<S>(this Entity self)
        where S : ICallSystem<AsyncTask>
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

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask>
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

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
        where S : ICallSystem<T1, T2, AsyncTask>
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
        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask>
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

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where S : ICallSystem<T1, T2, T3, T4, AsyncTask>
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
        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where S : ICallSystem<T1, T2, T3, T4, T5, AsyncTask>
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




        public static async void SendAsyncSystem<S>(this Entity self)
        where S : ICallSystem<AsyncTask>
        {
            await self.TrySendAsyncSystem<S>();
        }

        public static async void SendAsyncSystem<S, T1>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask>
        {
            await self.TrySendAsyncSystem<S, T1>(arg1);
        }

        public static async void SendAsyncSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
         where S : ICallSystem<T1, T2, AsyncTask>
        {
            await self.TrySendAsyncSystem<S, T1, T2>(arg1, arg2);
        }
        public static async void SendAsyncSystem<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask>
        {
            await self.TrySendAsyncSystem<S, T1, T2, T3>(arg1, arg2, arg3);
        }
        public static async void SendAsyncSystem<S, T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
         where S : ICallSystem<T1, T2, T3, T4, AsyncTask>
        {
            await self.TrySendAsyncSystem<S, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        }
        public static async void SendAsyncSystem<S, T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
         where S : ICallSystem<T1, T2, T3, T4, T5, AsyncTask>
        {
            await self.TrySendAsyncSystem<S, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        }
        #endregion


        #region Call
        public static async AsyncTask<OutT> CallAsyncSystem<S, OutT>(this Entity self)
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

        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, OutT>(this Entity self, T1 arg1)
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

        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2)
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
        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
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
        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, OutT>(this Entity self)
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

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, OutT>(this Entity self, T1 arg1)
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

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2)
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
        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
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
        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
