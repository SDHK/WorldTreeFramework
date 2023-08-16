/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 15:41

* 描述： 节点法则异步调用执行

*/

namespace WorldTree
{

    public static class NodeCallAsyncRuleGeneral
    {
        #region Call
        public static TreeTask<OutT> TryCallAsync<OutT>(this INode self, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<OutT>>.Null, defaultOutT);
        public static TreeTask<OutT> TryCallAsync<T1, OutT>(this INode self, T1 arg1, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<T1, OutT>>.Null, arg1, defaultOutT);
        public static TreeTask<OutT> TryCallAsync<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<T1, T2, OutT>>.Null, arg1, arg2, defaultOutT);
        public static TreeTask<OutT> TryCallAsync<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, OutT>>.Null, arg1, arg2, arg3, defaultOutT);
        public static TreeTask<OutT> TryCallAsync<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, OutT>>.Null, arg1, arg2, arg3, arg4, defaultOutT);
        public static TreeTask<OutT> TryCallAsync<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT) => self.TryCallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>>.Null, arg1, arg2, arg3, arg4, arg5, defaultOutT);

        public static TreeTask<OutT> CallAsync<N, OutT>(this N self, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<OutT>>.Null, defaultOutT);
        public static TreeTask<OutT> CallAsync<N, T1, OutT>(this N self, T1 arg1, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<T1, OutT>>.Null, arg1, defaultOutT);
        public static TreeTask<OutT> CallAsync<N, T1, T2, OutT>(this N self, T1 arg1, T2 arg2, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<T1, T2, OutT>>.Null, arg1, arg2, defaultOutT);
        public static TreeTask<OutT> CallAsync<N, T1, T2, T3, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, OutT>>.Null, arg1, arg2, arg3, defaultOutT);
        public static TreeTask<OutT> CallAsync<N, T1, T2, T3, T4, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, T4, OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, OutT>>.Null, arg1, arg2, arg3, arg4, defaultOutT);
        public static TreeTask<OutT> CallAsync<N, T1, T2, T3, T4, T5, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>> => self.CallRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>>.Null, arg1, arg2, arg3, arg4, arg5, defaultOutT);
       
        #endregion

        #region Calls

        public static TreeTask<UnitList<OutT>> TryCallsAsync<OutT>(this INode self, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<OutT>>.Null, defaultOutT);
        public static TreeTask<UnitList<OutT>> TryCallsAsync<T1, OutT>(this INode self, T1 arg1, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<T1, OutT>>.Null, arg1, defaultOutT);
        public static TreeTask<UnitList<OutT>> TryCallsAsync<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<T1, T2, OutT>>.Null, arg1, arg2, defaultOutT);
        public static TreeTask<UnitList<OutT>> TryCallsAsync<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, OutT>>.Null, arg1, arg2, arg3, defaultOutT);
        public static TreeTask<UnitList<OutT>> TryCallsAsync<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, OutT>>.Null, arg1, arg2, arg3, arg4, defaultOutT);
        public static TreeTask<UnitList<OutT>> TryCallsAsync<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT) => self.TryCallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>>.Null, arg1, arg2, arg3, arg4, arg5, defaultOutT);

        public static TreeTask<UnitList<OutT>> CallsAsync<N, OutT>(this N self, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<OutT>>.Null, defaultOutT);
        public static TreeTask<UnitList<OutT>> CallsAsync<N, T1, OutT>(this N self, T1 arg1, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<T1, OutT>>.Null, arg1, defaultOutT);
        public static TreeTask<UnitList<OutT>> CallsAsync<N, T1, T2, OutT>(this N self, T1 arg1, T2 arg2, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<T1, T2, OutT>>.Null, arg1, arg2, defaultOutT);
        public static TreeTask<UnitList<OutT>> CallsAsync<N, T1, T2, T3, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, OutT>>.Null, arg1, arg2, arg3, defaultOutT);
        public static TreeTask<UnitList<OutT>> CallsAsync<N, T1, T2, T3, T4, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, T4, OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, OutT>>.Null, arg1, arg2, arg3, arg4, defaultOutT);
        public static TreeTask<UnitList<OutT>> CallsAsync<N, T1, T2, T3, T4, T5, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, OutT defaultOutT) where N : class, INode, AsRule<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>> => self.CallsRuleAsync(NullType<ICallRuleAsync<T1, T2, T3, T4, T5, OutT>>.Null, arg1, arg2, arg3, arg4, arg5, defaultOutT);

        #endregion

    }
}
