/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:41

* 描述： 节点法则异步调用执行

*/

namespace WorldTree
{

    public static class CallAsyncExtension
    {
        #region Call

        public static TreeTask<OutT> CallAsync<OutT>(this Node self) => self.CallRuleAsync<ICallRuleAsync<OutT>, OutT>();
        public static TreeTask<OutT> CallAsync<T1, OutT>(this Node self, T1 arg1) => self.CallRuleAsync<ICallRuleAsync<T1, OutT>, T1, OutT>(arg1);
        public static TreeTask<OutT> CallAsync<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) => self.CallRuleAsync<ICallRuleAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) => self.CallRuleAsync<ICallRuleAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallRuleAsync<ICallRuleAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallRuleAsync<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);

        #endregion


        #region Calls

        public static TreeTask<UnitList<OutT>> CallsAsync<OutT>(this Node self) => self.CallsRuleAsync<ICallRuleAsync<OutT>, OutT>();
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this Node self, T1 arg1) => self.CallsRuleAsync<ICallRuleAsync<T1, OutT>, T1, OutT>(arg1);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) => self.CallsRuleAsync<ICallRuleAsync<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) => self.CallsRuleAsync<ICallRuleAsync<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallsRuleAsync<ICallRuleAsync<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4);
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallsRuleAsync<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5);
        #endregion

    }
}
