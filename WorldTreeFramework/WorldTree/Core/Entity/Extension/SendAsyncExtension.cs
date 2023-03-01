/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:22

* 描述： 实体系统事件异步发送

*/

namespace WorldTree
{
    public static class SendAsyncExtension
    {
        public static async AsyncTask<bool> TrySendAsync(this Entity self) => await self.TrySendSystemAsync<ICallSystem<AsyncTask>>();
        public static async AsyncTask<bool> TrySendAsync<T1>(this Entity self, T1 arg1) => await self.TrySendSystemAsync<ICallSystem<T1, AsyncTask>, T1>(arg1);
        public static async AsyncTask<bool> TrySendAsync<T1, T2>(this Entity self, T1 arg1, T2 arg2) => await self.TrySendSystemAsync<ICallSystem<T1, T2, AsyncTask>, T1, T2>(arg1, arg2);
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => await self.TrySendSystemAsync<ICallSystem<T1, T2, T3, AsyncTask>, T1, T2, T3>(arg1, arg2, arg3);
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => await self.TrySendSystemAsync<ICallSystem<T1, T2, T3, T4, AsyncTask>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        public static async AsyncTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => await self.TrySendSystemAsync<ICallSystem<T1, T2, T3, T4, T5, AsyncTask>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);


        public static async AsyncTask SendAsync(this Entity self) => await self.TrySendAsync();
        public static async AsyncTask SendAsync<T1>(this Entity self, T1 arg1) => await self.TrySendAsync(arg1);
        public static async AsyncTask SendAsync<T1, T2>(this Entity self, T1 arg1, T2 arg2) => await self.TrySendAsync(arg1, arg2);
        public static async AsyncTask SendAsync<T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => await self.TrySendAsync(arg1, arg2, arg3);
        public static async AsyncTask SendAsync<T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => await self.TrySendAsync(arg1, arg2, arg3, arg4);
        public static async AsyncTask SendAsync<T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => await self.TrySendAsync(arg1, arg2, arg3, arg4, arg5);

    }
}
