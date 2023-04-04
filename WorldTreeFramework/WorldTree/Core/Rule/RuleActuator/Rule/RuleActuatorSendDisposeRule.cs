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
            where R : ISendRule
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node);
                    }
                }
                self.Dispose();
            }
        }

        public static void SendDispose<R, T1>(this IRuleActuator<R> Self,T1 arg1)
            where R : ISendRule<T1>
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node, arg1);
                    }
                }
                self.Dispose();
            }
        }


        public static void SendDispose<R, T1, T2>(this IRuleActuator<R> Self, T1 arg1, T2 arg2)
            where R : ISendRule<T1, T2>
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node, arg1, arg2);
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3)
            where R : ISendRule<T1, T2, T3>
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node, arg1, arg2, arg3);
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3, T4>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where R : ISendRule<T1, T2, T3, T4>
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node, arg1, arg2, arg3, arg4);
                    }
                }
                self.Dispose();
            }
        }
        public static void SendDispose<R, T1, T2, T3, T4, T5>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where R : ISendRule<T1, T2, T3, T4, T5>
        {
            RuleActuator self = (RuleActuator)Self;
            if (self.ruleGroup != null && self.IsActive)
            {
                int length = self.nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (self.nodeQueue.TryDequeue(out INode node))
                    {
                        ((IRuleGroup<R>)self.ruleGroup).Send(node, arg1, arg2, arg3, arg4, arg5);
                    }
                }
                self.Dispose();
            }
        }


    }
}
