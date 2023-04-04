namespace WorldTree
{
    public static class RuleActuatorSendRule
    {
        public static void Send<R>(this IRuleActuator<R> Self)
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
                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public static void Send<R,T1>(this IRuleActuator<R> Self,T1 arg1)
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
                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public static void Send<R, T1, T2>(this IRuleActuator<R> Self, T1 arg1, T2 arg2)
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

                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public static void Send<R, T1, T2, T3>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3)
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
                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public static void Send<R, T1, T2, T3, T4>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public static void Send<R, T1, T2, T3, T4, T5>(this IRuleActuator<R> Self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
                        if (!node.IsRecycle) self.nodeQueue.Enqueue(node);
                    }
                }
            }
        }
    }

}
