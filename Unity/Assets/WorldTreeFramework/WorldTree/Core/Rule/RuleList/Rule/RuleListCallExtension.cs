/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:01

* 描述： 法则列表调用执行

*/


namespace WorldTree
{
    public interface ICallRuleTest : ICallRuleBase<float> { }

    public static class RuleListCallExtension
    {

        #region Call


        public static OutT Call<R, OutT>(this IRuleList<R> ruleList, INode node, out OutT outT)
            where R : ICallRuleBase<OutT>
        {
            foreach (ICallRuleBase<OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }

        public static OutT Call<R, T1, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, out OutT outT)
            where R : ICallRuleBase<T1, OutT>
        {
            foreach (ICallRuleBase<T1, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node, arg1);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }

        public static OutT Call<R, T1, T2, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, out OutT outT)
            where R : ICallRuleBase<T1, T2, OutT>
        {
            foreach (ICallRuleBase<T1, T2, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node, arg1, arg2);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }

        public static OutT Call<R, T1, T2, T3, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
            where R : ICallRuleBase<T1, T2, T3, OutT>
        {
            foreach (ICallRuleBase<T1, T2, T3, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node, arg1, arg2, arg3);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }

        public static OutT Call<R, T1, T2, T3, T4, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
            where R : ICallRuleBase<T1, T2, T3, T4, OutT>
        {
            foreach (ICallRuleBase<T1, T2, T3, T4, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node, arg1, arg2, arg3, arg4);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }

        public static OutT Call<R, T1, T2, T3, T4, T5, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
            where R : ICallRuleBase<T1, T2, T3, T4, T5, OutT>
        {
            foreach (ICallRuleBase<T1, T2, T3, T4, T5, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT = rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
                if (!rule.IsMulticast) return outT;
            }
            return outT = TypeInfo<OutT>.Default;
        }



        #endregion


        #region Calls


        public static UnitList<OutT> Calls<R, OutT>(this IRuleList<R> ruleList, INode node, out UnitList<OutT> outT)
         where R : ICallRuleBase<OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, out UnitList<OutT> outT)
         where R : ICallRuleBase<T1, OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<T1, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node, arg1));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }
        public static UnitList<OutT> Calls<R, T1, T2, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, out UnitList<OutT> outT)
         where R : ICallRuleBase<T1, T2, OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<T1, T2, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node, arg1, arg2));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> outT)
         where R : ICallRuleBase<T1, T2, T3, OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<T1, T2, T3, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node, arg1, arg2, arg3));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, T4, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> outT)
         where R : ICallRuleBase<T1, T2, T3, T4, OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<T1, T2, T3, T4, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node, arg1, arg2, arg3, arg4));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }

        public static UnitList<OutT> Calls<R, T1, T2, T3, T4, T5, OutT>(this IRuleList<R> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> outT)
         where R : ICallRuleBase<T1, T2, T3, T4, T5, OutT>
        {
            outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRuleBase<T1, T2, T3, T4, T5, OutT> rule in ruleList as RuleList)
            {
                rule.IsMulticast = true;
                outT.Add(rule.Invoke(node, arg1, arg2, arg3, arg4, arg5));
                if (!rule.IsMulticast) return outT;
            }
            return outT;
        }


        #endregion

    }
}
