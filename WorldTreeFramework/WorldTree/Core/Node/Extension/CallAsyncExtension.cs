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

        public static TreeTask<OutT> CallAsync<OutT>(this Node self) => self.CallSystemAsync<ICallRuleAsync<OutT>, OutT>();
        public static TreeTask<OutT> CallAsync<T1, OutT>(this Node self, T1 arg1) => self.CallSystemAsync<ICallRuleAsync<T1, OutT>, T1, OutT>(arg1);
        public static TreeTask<OutT> CallAsync<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) => self.CallSystemAsync<ICallRuleAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) => self.CallSystemAsync<ICallRuleAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallSystemAsync<ICallRuleAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallSystemAsync<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);

        #endregion


        #region Calls

        public static TreeTask<UnitList<OutT>> CallsAsync<OutT>(this Node self) => self.CallsSystemAsync<ICallRuleAsync<OutT>, OutT>();
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this Node self, T1 arg1) => self.CallsSystemAsync<ICallRuleAsync<T1, OutT>, T1, OutT>(arg1);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) => self.CallsSystemAsync<ICallRuleAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) => self.CallsSystemAsync<ICallRuleAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallsSystemAsync<ICallRuleAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallsSystemAsync<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);
        #endregion

    }
}
