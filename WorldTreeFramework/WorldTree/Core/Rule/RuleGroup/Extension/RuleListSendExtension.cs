
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/22 14:27

* 描述： 法则列表通知执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleListSendExtension
    {
        #region Send


        public static void Send1<T,T1>(this IRuleList<T> ruleList, INode node,T1 t1)
            where T : ISendRule<T1>
        { 
        
        
        }


        public static void Send(this List<IRule> ruleList, INode node)
        {
            foreach (ISendRule rule in ruleList)
            {
                rule.Invoke(node);
            }
        }

        public static void Send<T1>(this List<IRule> ruleList, INode node, T1 arg1)
        {
            foreach (ISendRule<T1> rule in ruleList)
            {
                rule.Invoke(node, arg1);
            }
        }

        public static void Send<T1, T2>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2)
        {
            foreach (ISendRule<T1, T2> rule in ruleList)
            {
                rule.Invoke(node, arg1, arg2);
            }
        }


        public static void Send<T1, T2, T3>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (ISendRule<T1, T2, T3> rule in ruleList)
            {
                rule.Invoke(node, arg1, arg2, arg3);
            }
        }

        public static void Send<T1, T2, T3, T4>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (ISendRule<T1, T2, T3, T4> rule in ruleList)
            {
                rule.Invoke(node, arg1, arg2, arg3, arg4);
            }
        }
        public static void Send<T1, T2, T3, T4, T5>(this List<IRule> ruleList, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (ISendRule<T1, T2, T3, T4, T5> rule in ruleList)
            {
                rule.Invoke(node, arg1, arg2, arg3, arg4, arg5);
            }
        }
        #endregion

    }
}
