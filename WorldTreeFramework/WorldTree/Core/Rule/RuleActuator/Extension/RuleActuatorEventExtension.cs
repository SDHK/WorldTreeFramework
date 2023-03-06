namespace WorldTree
{
    public partial class RuleActuator
    {
        public void Send()
        {
            if (ruleGroup != null && IsActive)
            {
                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node);
                        if(!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public void Send<T1>(T1 arg1)
        {
            if (ruleGroup != null && IsActive)
            {

                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node, arg1);
                        if (!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public void Send<T1, T2>(T1 arg1, T2 arg2)
        {
            if (ruleGroup != null && IsActive)
            {
                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node, arg1, arg2);

                        if (!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public void Send<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            if (ruleGroup != null && IsActive)
            {
                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node, arg1, arg2, arg3);
                        if (!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (ruleGroup != null && IsActive)
            {
                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node, arg1, arg2, arg3, arg4);
                        if (!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
        public void Send<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (ruleGroup != null && IsActive)
            {
                int length = nodeQueue.Count;
                for (int i = 0; i < length; i++)
                {
                    if (nodeQueue.TryDequeue(out Node node))
                    {
                        ruleGroup.Send(node, arg1, arg2, arg3, arg4, arg5);
                        if (!node.IsRecycle) nodeQueue.Enqueue(node);
                    }
                }
            }
        }
    }

}
