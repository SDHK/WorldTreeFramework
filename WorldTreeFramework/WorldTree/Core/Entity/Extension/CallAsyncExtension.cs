/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:41

* 描述： 实体系统事件异步调用

*/

namespace WorldTree
{

    public static class CallAsyncExtension
    {
        #region Call

        public static AsyncTask<OutT> CallAsync<OutT>(this Entity self) => self.CallSystemAsync<ICallSystemAsync<OutT>, OutT>();
        public static AsyncTask<OutT> CallAsync<T1, OutT>(this Entity self, T1 arg1) => self.CallSystemAsync<ICallSystemAsync<T1, OutT>, T1, OutT>(arg1);
        public static AsyncTask<OutT> CallAsync<T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2) => self.CallSystemAsync<ICallSystemAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static AsyncTask<OutT> CallAsync<T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => self.CallSystemAsync<ICallSystemAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static AsyncTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallSystemAsync<ICallSystemAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static AsyncTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallSystemAsync<ICallSystemAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);

        #endregion


        #region Calls

        public static AsyncTask<UnitList<OutT>> CallsAsync<OutT>(this Entity self) => self.CallsSystemAsync<ICallSystemAsync<OutT>, OutT>();
        public static AsyncTask<UnitList<OutT>> CallsAsync<T1, OutT>(this Entity self, T1 arg1) => self.CallsSystemAsync<ICallSystemAsync<T1, OutT>, T1, OutT>(arg1);
        public static AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this Entity self, T1 arg1, T2 arg2) => self.CallsSystemAsync<ICallSystemAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3) => self.CallsSystemAsync<ICallSystemAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallsSystemAsync<ICallSystemAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static AsyncTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this Entity self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallsSystemAsync<ICallSystemAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);
        #endregion

    }
}
