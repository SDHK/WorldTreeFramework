
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

        #region TrySend

        public static bool TrySend<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRule
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<R, T1>(this IRuleGroup<R> group, INode node, T1 t1)
        where R : ISendRule<T1>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, t1);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRule<T1, T2>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TrySend<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRule<T1, T2, T3>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TrySend<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRule<T1, T2, T3, T4>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool TrySend<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRule<T1, T2, T3, T4, T5>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                ((IRuleList<R>)ruleList).Send(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }



        //=======

        //public static bool TrySend(this RuleGroup group, INode node)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool TrySend<T1>(this RuleGroup group, INode node, T1 arg1)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node, arg1);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool TrySend<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node, arg1, arg2);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        //public static bool TrySend<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node, arg1, arg2, arg3);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool TrySend<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node, arg1, arg2, arg3, arg4);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public static bool TrySend<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        ruleList.Send(node, arg1, arg2, arg3, arg4, arg5);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        #endregion



        #region Send






        public static void Send<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRule
        {
            group.TrySend(node);
        }

        public static void Send<R, T1>(this IRuleGroup<R> group, INode node, T1 arg1)
        where R : ISendRule<T1>
        {
            group.TrySend(node, arg1);
        }

        public static void Send<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRule<T1, T2>
        {
            group.TrySend(node, arg1, arg2);
        }
        public static void Send<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRule<T1, T2, T3>
        {
            group.TrySend(node, arg1, arg2, arg3);
        }
        public static void Send<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRule<T1, T2, T3, T4>
        {
            group.TrySend(node, arg1, arg2, arg3, arg4);
        }
        public static void Send<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRule<T1, T2, T3, T4, T5>
        {
            group.TrySend(node, arg1, arg2, arg3, arg4, arg5);
        }


        //===

        //public static void Send(this RuleGroup group, INode node)
        //{
        //    group.TrySend(node);
        //}

        //public static void Send<T1>(this RuleGroup group, INode node, T1 arg1)
        //{
        //    group.TrySend(node, arg1);
        //}

        //public static void Send<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        //{
        //    group.TrySend(node, arg1, arg2);
        //}
        //public static void Send<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        //{
        //    group.TrySend(node, arg1, arg2, arg3);
        //}
        //public static void Send<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        //{
        //    group.TrySend(node, arg1, arg2, arg3, arg4);
        //}
        //public static void Send<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        //{
        //    group.TrySend(node, arg1, arg2, arg3, arg4, arg5);
        //}

        #endregion


    }
}
