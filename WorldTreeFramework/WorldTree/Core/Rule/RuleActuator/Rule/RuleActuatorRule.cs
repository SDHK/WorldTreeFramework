/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

namespace WorldTree
{
    class RuleActuatorAddRule : AddRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {
            self.AddComponent(out self.nodeQueue);
        }
    }

    class RuleActuatorRemoveRule : RemoveRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {
            self.nodeQueue = null;
            self.ruleGroup = null;
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
            self.nodeQueue.EnqueueReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue(this RuleActuator self, INode node)
        {
            self.nodeQueue.Enqueue(node);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove(this RuleActuator self, INode node)
        {
            self.nodeQueue.Remove(node);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this RuleActuator self)
        {
            self.nodeQueue.Clear();
        }

        /// <summary>
        /// 出列
        /// </summary>
        public static INode Dequeue(this RuleActuator self)
        {
            return self.nodeQueue.Dequeue();
        }
        /// <summary>
        /// 尝试出列
        /// </summary>
        public static bool TryDequeue(this RuleActuator self,out INode node)
        {
            return self.nodeQueue.TryDequeue(out node);
        }




        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>
        public static void EnqueueReferenced(this IRuleActuator self, INode node)
        {
            ((RuleActuator)self).nodeQueue.EnqueueReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue(this IRuleActuator self, INode node)
        {
            ((RuleActuator)self).nodeQueue.Enqueue(node);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove(this IRuleActuator self, INode node)
        {
            ((RuleActuator)self).nodeQueue.Remove(node);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this IRuleActuator self)
        {
            ((RuleActuator)self).nodeQueue.Clear();
        }

        /// <summary>
        /// 出列
        /// </summary>
        public static INode Dequeue(this IRuleActuator self)
        {
            return ((RuleActuator)self).nodeQueue.Dequeue();
        }
        /// <summary>
        /// 尝试出列
        /// </summary>
        public static bool TryDequeue(this IRuleActuator self, out INode node)
        {
            return ((RuleActuator)self).nodeQueue.TryDequeue(out node);
        }




    }

}
