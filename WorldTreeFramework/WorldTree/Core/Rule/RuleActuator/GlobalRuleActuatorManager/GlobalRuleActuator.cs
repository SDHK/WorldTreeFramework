
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 全局单法则执行器
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

        /// <summary>
        /// 法则集合
        /// </summary>
        public RuleGroup ruleGroup;
    }


    /// <summary>
    /// 全局法则执行器
    /// </summary>
    public class GlobalRuleActuator<R> : GlobalRuleActuatorBase, IRuleActuator<R>, ComponentOf<GlobalRuleActuatorManager>
     where R : IRule
    {
        public override bool TryGetNodeRuleGroup(INode node, out RuleGroup ruleGroup)
        {
            ruleGroup = this.ruleGroup;
            return this.ruleGroup != null;
        }
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
                            self.Enqueue(node.Value);
                        }
                    }
                }
            }
            self.ListenerSwitchesRule<R>();
        }

        class ListenerAddRule : ListenerAddRule<GlobalRuleActuatorBase>
        {
            public override void OnEvent(GlobalRuleActuatorBase self, INode node)
            {
              
                self.Enqueue(node);
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

    public static class GlobalRuleActuatorRule
    {
        class GlobalRuleActuatorAddRule<R> : AddRule<GlobalRuleActuator<R>>
        where R : IRule
        {
            public override void OnEvent(GlobalRuleActuator<R> self)
            {
                self.AddChild(out self.idQueue);
                self.AddChild(out self.removeIdDictionary);
                self.AddChild(out self.nodeDictionary);
                self.LoadGlobalNode<R>();
            }
        }




    }
}
