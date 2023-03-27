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

        public static TreeTask<OutT> CallAsync<OutT>(this INode self) => self.CallRuleAsync(default(ICallRuleAsync<OutT>), default(OutT));
        public static TreeTask<OutT> CallAsync<T1, OutT>(this INode self, T1 arg1) => self.CallRuleAsync(default(ICallRuleAsync<T1, OutT>), arg1, default(OutT));
        public static TreeTask<OutT> CallAsync<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2) => self.CallRuleAsync(default(ICallRuleAsync<T1, T2, OutT>), arg1, arg2, default(OutT));
        public static TreeTask<OutT> CallAsync<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3) => self.CallRuleAsync(default(ICallRuleAsync<T1, T2, T3, OutT>), arg1, arg2, arg3, default(OutT));
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallRuleAsync(default(ICallRuleAsync<T1, T2, T3, T4, OutT>), arg1, arg2, arg3, arg4, default(OutT));
        public static TreeTask<OutT> CallAsync<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallRuleAsync(default(ICallRuleAsync<T1, T2, T3, T4, T5, OutT>), arg1, arg2, arg3, arg4, arg5, default(OutT));

        #endregion


        #region Calls

        public static TreeTask<UnitList<OutT>> CallsAsync<OutT>(this INode self) => self.CallsRuleAsync(default(ICallRuleAsync<OutT>), default(UnitList<OutT>));
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, OutT>(this INode self, T1 arg1) => self.CallsRuleAsync(default(ICallRuleAsync<T1, OutT>), arg1, default(UnitList<OutT>));
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2) => self.CallsRuleAsync(default(ICallRuleAsync<T1, T2, OutT>), arg1, arg2, default(UnitList<OutT>));
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3) => self.CallsRuleAsync(default(ICallRuleAsync<T1, T2, T3, OutT>), arg1, arg2, arg3, default(UnitList<OutT>));
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => self.CallsRuleAsync(default(ICallRuleAsync<T1, T2, T3, T4, OutT>), arg1, arg2, arg3, arg4, default(UnitList<OutT>));
        public static TreeTask<UnitList<OutT>> CallsAsync<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => self.CallsRuleAsync(default(ICallRuleAsync<T1, T2, T3, T4, T5, OutT>), arg1, arg2, arg3, arg4, arg5, default(UnitList<OutT>));
        #endregion

    }
}
