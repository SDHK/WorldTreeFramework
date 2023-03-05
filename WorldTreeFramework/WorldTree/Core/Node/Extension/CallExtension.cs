/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 11:19

* 描述： 实体系统事件调用

*/

namespace WorldTree
{
    public static class CallExtension
    {
        #region Call

        public static bool TryCall<OutT>(this Node self, out OutT outT) => self.TryCallSystem<ICallRule<OutT>, OutT>(out outT);
        public static bool TryCall<T1, OutT>(this Node self, T1 arg1, out OutT outT) => self.TryCallSystem<ICallRule<T1, OutT>, T1, OutT>(arg1, out outT);
        public static bool TryCall<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2, out OutT outT) => self.TryCallSystem<ICallRule<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2, out outT);
        public static bool TryCall<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, out OutT outT) => self.TryCallSystem<ICallRule<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3, out outT);
        public static bool TryCall<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT) => self.TryCallSystem<ICallRule<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4, out outT);
        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT) => self.TryCallSystem<ICallRule<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5, out outT);

        public static OutT Call<OutT>(this Node self) { self.TryCall(out OutT outT); return outT; }
        public static OutT Call<T1, OutT>(this Node self, T1 arg1) { self.TryCall(arg1, out OutT outT); return outT; }
        public static OutT Call<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) { self.TryCall(arg1, arg2, out OutT outT); return outT; }
        public static OutT Call<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) { self.TryCall(arg1, arg2, arg3, out OutT outT); return outT; }
        public static OutT Call<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) { self.TryCall(arg1, arg2, arg3, arg4, out OutT outT); return outT; }
        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { self.TryCall(arg1, arg2, arg3, arg4, arg5, out OutT outT); return outT; }
        #endregion

        #region Calls
        public static bool TryCalls<OutT>(this Node self, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<OutT>, OutT>(out values);
        public static bool TryCalls<T1, OutT>(this Node self, T1 arg1, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<T1, OutT>, T1, OutT>(arg1, out values);
        public static bool TryCalls<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<T1, T2, OutT>, T1, T2, OutT>(arg1, arg2, out values);
        public static bool TryCalls<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<T1, T2, T3, OutT>, T1, T2, T3, OutT>(arg1, arg2, arg3, out values);
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<T1, T2, T3, T4, OutT>, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4, out values);
        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> values) => self.TryCallsSystem<ICallRule<T1, T2, T3, T4, T5, OutT>, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5, out values);

        public static UnitList<OutT> Calls<OutT>(this Node self) { self.TryCalls(out UnitList<OutT> values); return values; }
        public static UnitList<OutT> Calls<T1, OutT>(this Node self, T1 arg1) { self.TryCalls(arg1, out UnitList<OutT> values); return values; }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this Node self, T1 arg1, T2 arg2) { self.TryCalls(arg1, arg2, out UnitList<OutT> values); return values; }
        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3) { self.TryCalls(arg1, arg2, arg3, out UnitList<OutT> values); return values; }
        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4) { self.TryCalls(arg1, arg2, arg3, arg4, out UnitList<OutT> values); return values; }
        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { self.TryCalls(arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> values); return values; }



        #endregion
    }

}
