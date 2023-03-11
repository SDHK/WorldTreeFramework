
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/1 10:50

* 描述： 法则集合调用执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleGroupCallExtension
    {
        #region Call
        public static bool TryCall<OutT>(this RuleGroup group, INode node, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<OutT>(node);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<T1, OutT>(this RuleGroup group, INode node, T1 arg1, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<T1, OutT>(node, arg1);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCall<T1, T2, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<T1, T2, OutT>(node, arg1, arg2);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<T1, T2, T3, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<T1, T2, T3, OutT>(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }
        public static bool TryCall<T1, T2, T3, T4, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<T1, T2, T3, T4, OutT>(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }

        public static bool TryCall<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out OutT outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Call<T1, T2, T3, T4, T5, OutT>(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                outT = default(OutT);
                return false;
            }
        }



        public static OutT Call<OutT>(this RuleGroup group, INode node)
        {
            group.TryCall(node, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, OutT>(this RuleGroup group, INode node, T1 arg1)
        {
            group.TryCall(node, arg1, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        {
            group.TryCall(node, arg1, arg2, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TryCall(node, arg1, arg2, arg3, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TryCall(node, arg1, arg2, arg3, arg4, out OutT outT);
            return outT;
        }

        public static OutT Call<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TryCall(node, arg1, arg2, arg3, arg4, arg5, out OutT outT);
            return outT;
        }

        #endregion



        #region Calls

        public static bool TryCalls<OutT>(this RuleGroup group, INode node, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<OutT>(node);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<T1, OutT>(this RuleGroup group, INode node, T1 arg1, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<T1, OutT>(node, arg1);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<T1, T2, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<T1, T2, OutT>(node, arg1, arg2);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }

        public static bool TryCalls<T1, T2, T3, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<T1, T2, T3, OutT>(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }
        public static bool TryCalls<T1, T2, T3, T4, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<T1, T2, T3, T4, OutT>(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }

        public static bool TryCalls<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out UnitList<OutT> outT)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                outT = ruleList.Calls<T1, T2, T3, T4, T5, OutT>(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                outT = null;
                return false;
            }
        }





        public static UnitList<OutT> Calls<OutT>(this RuleGroup group, INode node)
        {
            group.TryCalls(node, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<T1, OutT>(this RuleGroup group, INode node, T1 arg1)
        {
            group.TryCalls(node, arg1, out UnitList<OutT> outT);
            return outT;
        }
        public static UnitList<OutT> Calls<T1, T2, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        {
            group.TryCalls(node, arg1, arg2, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TryCalls(node, arg1, arg2, arg3, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TryCalls(node, arg1, arg2, arg3, arg4, out UnitList<OutT> outT);
            return outT;
        }

        public static UnitList<OutT> Calls<T1, T2, T3, T4, T5, OutT>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TryCalls(node, arg1, arg2, arg3, arg4, arg5, out UnitList<OutT> outT);
            return outT;
        }
        #endregion



    }
}
