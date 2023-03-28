
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:50

* 描述： 法则集合调用执行

*/


namespace WorldTree
{
    public static class RuleGroupCallExtension
    {
        #region TryCall

        public static bool TryCall<R, OutT>(this IRuleGroup<R> group, INode node, out OutT outT)
        where R : ICallRule<OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<R, T1, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, out OutT outT)
        where R : ICallRule<T1, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, arg1, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCall<R, T1, T2, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, out OutT outT)
        where R : ICallRule<T1, T2, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, arg1, arg2, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, arg1, arg2, arg3, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCall<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, arg1, arg2, arg3, arg4, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Call(node, arg1, arg2, arg3, arg4, arg5, out outT);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }


        #endregion


        #region Call

        public static OutT Call<R, OutT>(this IRuleGroup<R> group, INode node)
            where R : ICallRule<OutT>
        {
            group.TryCall(node, out OutT outT);
            return outT;
        }

        public static OutT Call<R, T1, OutT>(this IRuleGroup<R> group, INode node, T1 arg1)
            where R : ICallRule<T1, OutT>
        {
            group.TryCall(node, arg1, out OutT outT);
            return outT;
        }

        public static OutT Call<R, T1, T2, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
            where R : ICallRule<T1, T2, OutT>
        {
            group.TryCall(node, arg1, arg2, out OutT outT);
            return outT;
        }

        public static OutT Call<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
            where R : ICallRule<T1, T2, T3, OutT>
        {
            group.TryCall(node, arg1, arg2, arg3, out OutT outT);
            return outT;
        }

        public static OutT Call<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            group.TryCall(node, arg1, arg2, arg3, arg4, out OutT outT);
            return outT;
        }

        public static OutT Call<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            group.TryCall(node, arg1, arg2, arg3, arg4, arg5, out OutT outT);
            return outT;
        }


        #endregion



        #region TryCalls


        public static bool TryCalls<R, OutT>(this IRuleGroup<R> group, INode node, out UnitList<OutT> outT)
        where R : ICallRule<OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<R, T1, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, out UnitList<OutT> outT)
        where R : ICallRule<T1, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, arg1, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<R, T1, T2, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, out UnitList<OutT> outT)
        where R : ICallRule<T1, T2, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, arg1, arg2, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }

        public static bool TryCalls<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> outT)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, arg1, arg2, arg3, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> outT)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, arg1, arg2, arg3, arg4, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }

        public static bool TryCalls<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> outT)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Calls(node, arg1, arg2, arg3, arg4, arg5, out outT);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }

        #endregion


        #region Calls


        public static UnitList<OutT> Calls<R, OutT>(this IRuleGroup<R> group, INode node)
        where R : ICallRule<OutT>
        {
            group.TryCalls(node, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<R, T1, OutT>(this IRuleGroup<R> group, INode node, T1 arg1)
        where R : ICallRule<T1, OutT>
        {
            group.TryCalls(node, arg1, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<R, T1, T2, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ICallRule<T1, T2, OutT>
        {
            group.TryCalls(node, arg1, arg2, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ICallRule<T1, T2, T3, OutT>
        {
            group.TryCalls(node, arg1, arg2, arg3, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, T4, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ICallRule<T1, T2, T3, T4, OutT>
        {
            group.TryCalls(node, arg1, arg2, arg3, arg4, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, T4, T5, OutT>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ICallRule<T1, T2, T3, T4, T5, OutT>
        {
            group.TryCalls(node, arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> outT);
            return outT;
        }

        #endregion



    }
}
