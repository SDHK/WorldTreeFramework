/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class RuleActuatorRule
    {

        /// <summary>
        /// 执行器初始化全局填装节点
        /// </summary>
        public static RuleActuator Load<T>(this RuleActuator ruleActuator) where T : IRule => Load(ruleActuator, typeof(T));
        /// <summary>
        /// 执行器初始化全局填装节点
        /// </summary>
        public static RuleActuator Load(this RuleActuator ruleActuator, Type ruleType)
        {
            if (ruleActuator.Core.RuleManager.TryGetRuleGroup(ruleType, out ruleActuator.ruleGroup))
            {
                ruleActuator.Clear();
                foreach (var item in ruleActuator.ruleGroup)
                {
                    if (ruleActuator.Core.NodePoolManager.pools.TryGetValue(item.Key, out NodePool pool))
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
        /// 获取全局节点法则执行器
        /// </summary>
        public static RuleActuator GetGlobalNodeRuleActuator<T>(this INode self)
        where T : IRule
        {
            var ruleActuator = self.Root.AddComponent(out RuleActuatorGroup _).GetActuator<T>();
            ruleActuator.nodeQueue.AddComponent(out NodeAddGlobalListener _).ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            ruleActuator.nodeQueue.AddComponent(out NodeRemoveGlobalListener _).ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            return ruleActuator;
        }
    }

}
