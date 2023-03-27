
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 17:43

* 描述： 法则集合异步通知执行

*/

using System.Collections.Generic;

namespace WorldTree
{
    public static class RuleGroupSendAsyncExtension
    {

        #region TrySendAsync


        public static async TreeTask<bool> TrySendAsync<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRuleAsync
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<R, T1>(this IRuleGroup<R> group, INode node, T1 arg1)
        where R : ISendRuleAsync<T1>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRuleAsync<T1, T2>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleAsync<T1, T2, T3>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }
        public static async TreeTask<bool> TrySendAsync<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                await node.TreeTaskCompleted();
                return false;
            }
        }



        //====

        //public static async TreeTask<bool> TrySendAsync(this RuleGroup group, INode node)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        //public static async TreeTask<bool> TrySendAsync<T1>(this RuleGroup group, INode node, T1 arg1)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        //public static async TreeTask<bool> TrySendAsync<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        //public static async TreeTask<bool> TrySendAsync<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        //public static async TreeTask<bool> TrySendAsync<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3, arg4);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        //public static async TreeTask<bool> TrySendAsync<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3, arg4, arg5);
        //        return true;
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //        return false;
        //    }
        //}
        #endregion



        #region SendAsync


        public static async TreeTask SendAsync<R>(this IRuleGroup<R> group, INode node)
        where R : ISendRuleAsync
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<R,T1>(this IRuleGroup<R> group, INode node, T1 arg1)
        where R : ISendRuleAsync<T1>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<R, T1, T2>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2)
        where R : ISendRuleAsync<T1, T2>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3)
        where R : ISendRuleAsync<T1, T2, T3>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3, T4>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where R : ISendRuleAsync<T1, T2, T3, T4>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3, arg4);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }
        public static async TreeTask SendAsync<R, T1, T2, T3, T4, T5>(this IRuleGroup<R> group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where R : ISendRuleAsync<T1, T2, T3, T4, T5>
        {
            if ((group as RuleGroup).TryGetValue(node.Type, out RuleList ruleList))
            {
                await ((IRuleList<R>)ruleList).SendAsync(node, arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                await node.TreeTaskCompleted();
            }
        }


        //====
        //public static async TreeTask SendAsync(this RuleGroup group, INode node)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        //public static async TreeTask SendAsync<T1>(this RuleGroup group, INode node, T1 arg1)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        //public static async TreeTask SendAsync<T1, T2>(this RuleGroup group, INode node, T1 arg1, T2 arg2)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        //public static async TreeTask SendAsync<T1, T2, T3>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        //public static async TreeTask SendAsync<T1, T2, T3, T4>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3, arg4);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        //public static async TreeTask SendAsync<T1, T2, T3, T4, T5>(this RuleGroup group, INode node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        //{
        //    if (group.TryGetValue(node.Type, out RuleList ruleList))
        //    {
        //        await ruleList.SendAsync(node, arg1, arg2, arg3, arg4, arg5);
        //    }
        //    else
        //    {
        //        await node.TreeTaskCompleted();
        //    }
        //}
        #endregion


    }
}
