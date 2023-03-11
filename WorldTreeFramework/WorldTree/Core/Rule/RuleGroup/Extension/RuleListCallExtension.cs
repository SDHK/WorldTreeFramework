/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 11:01

* 描述： 法则列表调用执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleListCallExtension
    {


        #region Call

        public static OutT Call<OutT>(this List<IRule> ruleList, INode node)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<OutT> rule in ruleList)
            {
                outT = rule.Invoke(node);
            }
            return outT;
        }

        public static OutT Call<T1, OutT>(this List<IRule> ruleList, INode node, T1 arg1)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<T1, OutT> rule in ruleList)
            {
                outT = rule.Invoke(node, arg1);
            }
            return outT;
        }

        public static OutT Call<T1, T2, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<T1, T2, OutT> rule in ruleList)
            {
                outT = rule.Invoke(node, arg1, arg2);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<T1, T2, T3, OutT> rule in ruleList)
            {
                outT = rule.Invoke(node, arg1, arg2, arg3);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<T1, T2, T3, T4, OutT> rule in ruleList)
            {
                outT = rule.Invoke(node, arg1, arg2, arg3, arg4);
            }
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            OutT outT = default(OutT);
            foreach (ICallRule<T1, T2, T3, T4, T5, OutT> rule in ruleList)
            {
                outT = rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
            }
            return outT;
        }

        #endregion


        #region Calls

        public static UnitList<OutT> Calls<OutT>(this List<IRule> ruleList, INode node)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node));
            }
            return outT;
        }


        public static UnitList<OutT> Calls<T1, OutT>(this List<IRule> ruleList, INode node, T1 arg1)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<T1, OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node, arg1));
            }
            return outT;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<T1, T2, OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node, arg1, arg2));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<T1, T2, T3, OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node, arg1, arg2, arg3));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<T1, T2, T3, T4, OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node, arg1, arg2, arg3, arg4));
            }
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            UnitList<OutT> outT = node.PoolGet<UnitList<OutT>>();
            foreach (ICallRule<T1, T2, T3, T4, T5, OutT> rule in ruleList)
            {
                outT.Add(rule.Invoke(node, arg1, arg2, arg3, arg4, arg5));
            }
            return outT;
        }

        #endregion

    }
}
