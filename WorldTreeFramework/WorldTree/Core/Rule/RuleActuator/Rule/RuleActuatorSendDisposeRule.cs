/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 16:55

* 描述： 广播发送

*/

namespace WorldTree
{
    public static class RuleActuatorSendDisposeRule
    {

        public static void SendDispose<R>(this IRuleActuator<R> Self)
            where R : ISendRuleBase
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node);
                        }
                    }
                }
                self.Dispose();
            }
        }

        public static void SendDispose<R, T1>(this IRuleActuator<R> Self, T1 arg1)
            where R : ISendRuleBase<T1>
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node, arg1);
                        }
                    }
                }
                self.Dispose();
            }
        }


        public static void SendDispose<R, T1, T2>(this IRuleActuator<R> Self, T1 arg1, T2 arg2)
            where R : ISendRuleBase<T1, T2>
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node, arg1, arg2);
                        }
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3)
            where R : ISendRuleBase<T1, T2, T3>
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node, arg1, arg2, arg3);
                        }
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3, T4>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ISendRuleBase<T1, T2, T3, T4>
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node, arg1, arg2, arg3, arg4);
                        }
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3, T4, T5>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ISendRuleBase<T1, T2, T3, T4, T5>
        {
            RuleActuatorBase self = (RuleActuatorBase)Self;
            if (self.IsActive)
            {
                self.RefreshTraversalCount();
                for (int i = 0; i < self.traversalCount; i++)
                {
                    if (self.TryDequeue(out INode node))
                    {
                        if (self.TryGetNodeRuleGroup(node, out RuleGroup ruleGroup))
                        {
                            ((IRuleGroup<R>)ruleGroup).Send(node, arg1, arg2, arg3, arg4, arg5);
                        }
                    }
                }
                self.Dispose();
            }
        }


    }
}
