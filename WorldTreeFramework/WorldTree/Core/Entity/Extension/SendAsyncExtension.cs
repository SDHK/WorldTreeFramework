/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:22

* 描述： 实体系统事件异步发送

*/

namespace WorldTree
{
    public static class SendAsyncExtension
    {
        public static AsyncTask<bool> TrySendAsync(this Entity self) => self.TrySendSystemAsync<ISendSystemAsync>();
        public static AsyncTask<bool> TrySendAsync<T1>(this Entity self, T1 arg1) => self.TrySendSystemAsync<ISendSystemAsync<T1>, T1>(arg1);
        public static AsyncTask<bool> TrySendAsync<T1, T2>(this Entity self, T1 arg1, T2 arg2) => self.TrySendSystemAsync<ISendSystemAsync<T1, T2>, T1, T2>(arg1, arg2);
        public static AsyncTask<bool> TrySendAsync<T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => self.TrySendSystemAsync<ISendSystemAsync<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
        public static AsyncTask<bool> TrySendAsync<T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.TrySendSystemAsync<ISendSystemAsync<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        public static AsyncTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.TrySendSystemAsync<ISendSystemAsync<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);


        public static AsyncTask SendAsync(this Entity self) => self.SendSystemAsync<ISendSystemAsync>();
        public static AsyncTask SendAsync<T1>(this Entity self, T1 arg1) => self.SendSystemAsync<ISendSystemAsync<T1>, T1>(arg1);
        public static AsyncTask SendAsync<T1, T2>(this Entity self, T1 arg1, T2 arg2) => self.SendSystemAsync<ISendSystemAsync<T1, T2>, T1, T2>(arg1, arg2);
        public static AsyncTask SendAsync<T1, T2, T3>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => self.SendSystemAsync<ISendSystemAsync<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
        public static AsyncTask SendAsync<T1, T2, T3, T4>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.SendSystemAsync<ISendSystemAsync<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        public static AsyncTask SendAsync<T1, T2, T3, T4, T5>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.SendSystemAsync<ISendSystemAsync<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);

    }
}
