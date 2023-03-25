
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 法则集合通知执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleGroupSendExtension
    {

        #region Send

        public static  bool TrySendTest<T, T1>(this IRuleGroup<T> group, INode node, T1 t1)
        where T : ISendRule<T1>
        {
            return (group as RuleGroup).TrySend(node, t1);
        }


        public static bool TrySend(this RuleGroup group, INode node)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1>(this RuleGroup group, INode node, T1 arg1)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node, arg1);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TrySend<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (group.TryGetValue(node.Type, out List<IRule> ruleList))
            {
                ruleList.Send(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void Send(this RuleGroup group, INode node)
        {
            group.TrySend(node);
        }

        public static void Send<T1>(this RuleGroup group, INode node, T1 arg1)
        {
            group.TrySend(node, arg1);
        }

        public static void Send<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        {
            group.TrySend(node, arg1, arg2);
        }
        public static void Send<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            group.TrySend(node, arg1, arg2, arg3);
        }
        public static void Send<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            group.TrySend(node, arg1, arg2, arg3, arg4);
        }
        public static void Send<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            group.TrySend(node, arg1, arg2, arg3, arg4, arg5);
        }
        #endregion


    }
}
