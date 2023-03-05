/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:31

* 描述： 节点法则调用扩展

*/

namespace WorldTree
{
    public static class CallRuleExtension
    {
        #region Call

        public static bool TryCallRule<R, OutT>(this Node self, out OutT outT)
        where R : ICallRule<OutT>
        {

            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }

        }
        public static bool TryCallRule<R, T1, OutT>(this Node self, T1 arg1, out OutT outT)
        where R : ICallRule<T1, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, arg1, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCallRule<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2, out OutT outT)
        where R : ICallRule<T1, T2, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, arg1, arg2, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCallRule<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, arg1, arg2, arg3, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCallRule<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, arg1, arg2, arg3, arg4, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCallRule<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCall(self, arg1, arg2, arg3, arg4, arg5, out outT);
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }



        public static OutT CallRule<R, OutT>(this Node self)
        where R : ICallRule<OutT>
        {
            self.TryCallRule<R, OutT>(out OutT outT);
            return outT;
        }
        public static OutT CallRule<R, T1, OutT>(this Node self, T1 arg1)
        where R : ICallRule<T1, OutT>
        {
            self.TryCallRule<R, T1, OutT>(arg1, out OutT outT);
            return outT;
        }
        public static OutT CallRule<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2)
        where R : ICallRule<T1, T2, OutT>
        {
            self.TryCallRule<R, T1, T2, OutT>(arg1, arg2, out OutT outT);
            return outT;
        }
        public static OutT CallRule<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            self.TryCallRule<R, T1, T2, T3, OutT>(arg1, arg2, arg3, out OutT outT);
            return outT;
        }
        public static OutT CallRule<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            self.TryCallRule<R, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4, out OutT outT);
            return outT;
        }
        public static OutT CallRule<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            self.TryCallRule<R, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5, out OutT outT);
            return outT;
        }
        #endregion


        #region Calls
        public static bool TryCallsRule<R, OutT>(this Node self, out UnitList<OutT> values)
        where R : ICallRule<OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsRule<R, T1, OutT>(this Node self, T1 arg1, out UnitList<OutT> values)
        where R : ICallRule<T1, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, arg1, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsRule<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2, out UnitList<OutT> values)
        where R : ICallRule<T1, T2, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, arg1, arg2, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsRule<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> values)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, arg1, arg2, arg3, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsRule<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> values)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, arg1, arg2, arg3, arg4, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }
        public static bool TryCallsRule<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> values)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            if (self.Root.SystemManager.TryGetGroup<R>(out RuleGroup group))
            {
                return group.TryCalls(self, arg1, arg2, arg3, arg4, arg5, out values);
            }
            else
            {
                values = null;
                return false;
            }
        }



        public static UnitList<OutT> CallsRule<R, OutT>(this Node self)
        where R : ICallRule<OutT>
        {
            self.TryCallsRule<R, OutT>(out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsRule<R, T1, OutT>(this Node self, T1 arg1)
        where R : ICallRule<T1, OutT>
        {
            self.TryCallsRule<R, T1, OutT>(arg1, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsRule<R, T1, T2, OutT>(this Node self, T1 arg1, T2 arg2)
        where R : ICallRule<T1, T2, OutT>
        {
            self.TryCallsRule<R, T1, T2, OutT>(arg1, arg2, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsRule<R, T1, T2, T3, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            self.TryCallsRule<R, T1, T2, T3, OutT>(arg1, arg2, arg3, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsRule<R, T1, T2, T3, T4, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            self.TryCallsRule<R, T1, T2, T3, T4, OutT>(arg1, arg2, arg3, arg4, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> CallsRule<R, T1, T2, T3, T4, T5, OutT>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            self.TryCallsRule<R, T1, T2, T3, T4, T5, OutT>(arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> outT);
            return outT;
        }
        #endregion

    }
}
