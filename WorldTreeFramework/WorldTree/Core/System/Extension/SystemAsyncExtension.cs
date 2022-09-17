using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return await group.TrySendAsyncSystem(self);
            }
            else
            {
                await self.AsyncYield();
                return false;
            }
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsyncSystem(self, arg1);
            }
            else
            {
                await self.AsyncYield();
                return false;
            }
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2>(this Entity self, T1 arg1, T2 arg2)
        where S : ICallSystem<T1, T2, AsyncTask>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsyncSystem(self, arg1, arg2);
            }
            else
            {
                await self.AsyncYield();
                return false;
            }
        }
        public static async AsyncTask<bool> TrySendAsyncSystem<S, T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3)
        where S : ICallSystem<T1, T2, T3, AsyncTask>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.TrySendAsyncSystem(self, arg1, arg2, arg3);
            }
            else
            {
                await self.AsyncYield();
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
        #endregion


        #region Call
        public static async AsyncTask<OutT> CallAsyncSystem<S, OutT>(this Entity self)
        where S : ICallSystem<AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsyncSystem<OutT>(self);
            }
            else
            {
                await self.AsyncYield();
                return default(OutT);
            }
        }

        public static async AsyncTask<OutT> CallAsyncSystem<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallAsyncSystem<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncYield();
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
                return await group.CallsAsyncSystem<OutT>(self);
            }
            else
            {
                await self.AsyncYield();
                return null;
            }
        }

        public static async AsyncTask<UnitList<OutT>> CallsAsyncSystem<S, T1, OutT>(this Entity self, T1 arg1)
        where S : ICallSystem<T1, AsyncTask<OutT>>
        {
            if (self.Root.SystemManager.TryGetGroup<S>(out SystemGroup group))
            {
                return await group.CallsAsyncSystem<T1, OutT>(self, arg1);
            }
            else
            {
                await self.AsyncYield();
                return null;
            }
        }
        #endregion
    }
}
