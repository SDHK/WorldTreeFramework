/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

namespace WorldTree
{
    class RuleActuatorRemoveRule : RemoveRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {
            self.nodeQueue = null;
            self.ruleGroup = null;
        }
    }


    class RuleActuatorReferencedChildRemoveRule : ReferencedChildRemoveRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self, INode node)
        {
            self.nodeQueue?.Remove(node);
            if (self.nodeQueue != null && self.nodeQueue.Count == 0)
            {
                self.nodeQueue.Dispose();
                self.nodeQueue = null;
            }
        }
    }

    public static class RuleActuatorRule
    {

        /// <summary>
        /// 执行器初始化全局填装节点
        /// </summary>
        public static RuleActuator LoadGlobalNode<R>(this RuleActuator ruleActuator)
        where R : IRule
        {
            var ruleType = typeof(R);

            if (ruleActuator.Core.RuleManager.TryGetRuleGroup(ruleType, out ruleActuator.ruleGroup))
            {
                ruleActuator.Clear();
                foreach (var item in ruleActuator.ruleGroup)
                {
                    if (ruleActuator.Core.NodePoolManager.m_Pools.TryGetValue(item.Key, out NodePool pool))
                    {
                        foreach (var node in pool.Nodes)
                        {
                            ruleActuator.Enqueue(node.Value);
                        }
                    }
                }
            }
            return ruleActuator;
        }

        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>

        public static void EnqueueReferenced(this RuleActuator self, INode node)
        {
            if (self.ruleGroup.ContainsKey(node.Type))
            {
                self.Referenced(node);
                self.nodeQueue ??= self.AddComponent(out self.nodeQueue);
                self.nodeQueue.Enqueue(node);
            }
            else
            {
                throw new RuleActuatorException($"节点 {node.Type} 未实现 {self.ruleGroup.RuleType} 法则，无法添加");
            }
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue(this RuleActuator self, INode node)
        {
            if (self.ruleGroup.ContainsKey(node.Type))
            {
                self.nodeQueue ??= self.AddComponent(out self.nodeQueue);
                self.nodeQueue.Enqueue(node);
            }
            else
            {
                throw new RuleActuatorException($"节点 {node.Type} 未实现 {self.ruleGroup.RuleType} 法则，无法添加");
            }
        }


        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove(this RuleActuator self, INode node)
        {
            self.DeReferenced(node);
            self.nodeQueue?.Remove(node);
            if (self.nodeQueue != null && self.nodeQueue.Count == 0)
            {
                self.nodeQueue.Dispose();
                self.nodeQueue = null;
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this RuleActuator self)
        {
            self.DeReferencedAll();
            self.nodeQueue?.Clear();
            self.nodeQueue?.Dispose();
            self.nodeQueue = null;
        }


        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>
        public static void EnqueueReferenced<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).EnqueueReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).Enqueue(node);
        }


        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuator)self).Remove(node);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this IRuleActuator self)
        {
            ((RuleActuator)self).Clear();
        }

        /// <summary>
        /// 添加法则节点并建立引用关系
        /// </summary>
        public static void Add<N, R, RN>(this IRuleActuator<R> self, N node, out RN ruleNode)
            where N : class, INode
            where R : IRule
            where RN : class, INode, ComponentOf<N>, AsRule<R>
        {
            ((RuleActuator)self).EnqueueReferenced(node.AddComponent(out ruleNode));
        }

    }

}
