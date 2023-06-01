
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/30 20:35

* 描述： 全局法则执行器基类
* 
* 只有全局执行器需要动态全局监听

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 全局法则执行器基类
    /// </summary>
    public abstract class GlobalRuleActuatorBase : RuleActuatorBase, INodeListener, IRuleActuator
    {
        public ListenerState listenerState { get; set; }
        public Type listenerTarget { get; set; }

    }

    public static class GlobalRuleActuatorBaseRule
    {
        /// <summary>
        /// 填装全局节点,并设置监听目标法则
        /// </summary>
        public static void LoadGlobalNode<R>(this GlobalRuleActuatorBase self)
         where R : IRule
        {
            if (self.Core.RuleManager.TryGetRuleGroup<R>(out self.ruleGroup))
            {
                foreach (var item in self.ruleGroup)
                {
                    if (self.Core.NodePoolManager.m_Pools.TryGetValue(item.Key, out NodePool pool))
                    {
                        foreach (var node in pool.Nodes)
                        {
                            self.TryAdd(node.Value);
                        }
                    }
                }
            }
        }

        class ListenerAddRule : ListenerAddRule<GlobalRuleActuatorBase>
        {
            public override void OnEvent(GlobalRuleActuatorBase self, INode node)
            {
                self.TryAdd(node);
            }
        }

        class ListenerRemoveRule : ListenerRemoveRule<GlobalRuleActuatorBase>
        {
            public override void OnEvent(GlobalRuleActuatorBase self, INode node)
            {
                self.Remove(node);
            }
        }
    }

}
