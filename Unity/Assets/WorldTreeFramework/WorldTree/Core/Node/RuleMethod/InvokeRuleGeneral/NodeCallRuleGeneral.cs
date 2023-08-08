/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 11:19

* 描述： 节点法则调用执行

*/

namespace WorldTree
{
    public static class NodeCallRuleGeneral
    {
        #region Call

        public static bool TryCall<OutT>(this INode self, out OutT outT) => self.TryCallRule((ICallRule<OutT>)null, out outT);
        public static bool TryCall<T1, OutT>(this INode self, T1 arg1, out OutT outT) => self.TryCallRule((ICallRule<T1, OutT>)null, arg1, out outT);
        public static bool TryCall<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2, out OutT outT) => self.TryCallRule((ICallRule<T1, T2, OutT>)null, arg1, arg2, out outT);
        public static bool TryCall<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, out OutT outT) => self.TryCallRule((ICallRule<T1, T2, T3, OutT>)null, arg1, arg2, arg3, out outT);
        public static bool TryCall<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT) => self.TryCallRule((ICallRule<T1, T2, T3, T4, OutT>)null, arg1, arg2, arg3, arg4, out outT);
        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT) => self.TryCallRule((ICallRule<T1, T2, T3, T4, T5, OutT>)null, arg1, arg2, arg3, arg4, arg5, out outT);

        public static OutT Call<N, OutT>(this N self, out OutT outT) where N : class, INode, AsRule<ICallRule<OutT>> => self.CallRule((ICallRule<OutT>)null, out outT);
        public static OutT Call<N, T1, OutT>(this N self, T1 arg1, out OutT outT) where N : class, INode, AsRule<ICallRule<T1, OutT>> => self.CallRule((ICallRule<T1, OutT>)null, arg1, out outT);
        public static OutT Call<N, T1, T2, OutT>(this N self, T1 arg1, T2 arg2, out OutT outT) where N : class, INode, AsRule<ICallRule<T1, T2, OutT>> => self.CallRule((ICallRule<T1, T2, OutT>)null, arg1, arg2, out outT);
        public static OutT Call<N, T1, T2, T3, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, out OutT outT) where N : class, INode, AsRule<ICallRule<T1, T2, T3, OutT>> => self.CallRule((ICallRule<T1, T2, T3, OutT>)null, arg1, arg2, arg3, out outT);
        public static OutT Call<N, T1, T2, T3, T4, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT) where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, OutT>> => self.CallRule((ICallRule<T1, T2, T3, T4, OutT>)null, arg1, arg2, arg3, arg4, out outT);
        public static OutT Call<N, T1, T2, T3, T4, T5, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT) where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, T5, OutT>> => self.CallRule((ICallRule<T1, T2, T3, T4, T5, OutT>)null, arg1, arg2, arg3, arg4, arg5, out outT);
        #endregion

        #region Calls
        public static bool TryCalls<OutT>(this INode self, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<OutT>)null, out values);
        public static bool TryCalls<T1, OutT>(this INode self, T1 arg1, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<T1, OutT>)null, arg1, out values);
        public static bool TryCalls<T1, T2, OutT>(this INode self, T1 arg1, T2 arg2, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<T1, T2, OutT>)null, arg1, arg2, out values);
        public static bool TryCalls<T1, T2, T3, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<T1, T2, T3, OutT>)null, arg1, arg2, arg3, out values);
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<T1, T2, T3, T4, OutT>)null, arg1, arg2, arg3, arg4, out values);
        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> values) => self.TryCallsRule((ICallRule<T1, T2, T3, T4, T5, OutT>)null, arg1, arg2, arg3, arg4, arg5, out values);

        public static UnitList<OutT> Calls<N, OutT>(this N self, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<OutT>> => self.CallsRule((ICallRule<OutT>)null, out values);
        public static UnitList<OutT> Calls<N, T1, OutT>(this N self, T1 arg1, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<T1, OutT>> => self.CallsRule((ICallRule<T1, OutT>)null, arg1, out values);
        public static UnitList<OutT> Calls<N, T1, T2, OutT>(this N self, T1 arg1, T2 arg2, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<T1, T2, OutT>> => self.CallsRule((ICallRule<T1, T2, OutT>)null, arg1, arg2, out values);
        public static UnitList<OutT> Calls<N, T1, T2, T3, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<T1, T2, T3, OutT>> => self.CallsRule((ICallRule<T1, T2, T3, OutT>)null, arg1, arg2, arg3, out values);
        public static UnitList<OutT> Calls<N, T1, T2, T3, T4, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, OutT>> => self.CallsRule((ICallRule<T1, T2, T3, T4, OutT>)null, arg1, arg2, arg3, arg4, out values);
        public static UnitList<OutT> Calls<N, T1, T2, T3, T4, T5, OutT>(this N self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> values) where N : class, INode, AsRule<ICallRule<T1, T2, T3, T4, T5, OutT>> => self.CallsRule((ICallRule<T1, T2, T3, T4, T5, OutT>)null, arg1, arg2, arg3, arg4, arg5, out values);



        #endregion
    }

}
