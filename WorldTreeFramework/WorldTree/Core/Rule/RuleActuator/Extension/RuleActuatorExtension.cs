/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class RuleActuatorExtension
    {

        /// <summary>
        /// 执行器初始化填装节点
        /// </summary>
        public static RuleActuator Load<T>(this RuleActuator ruleActuator) where T : IRule => Load(ruleActuator, typeof(T));
        /// <summary>
        /// 执行器初始化填装节点
        /// </summary>
        public static RuleActuator Load(this RuleActuator ruleActuator, Type ruleType)
        {
            if (ruleActuator.Root.RuleManager.TryGetRuleGroup(ruleType, out ruleActuator.ruleGroup))
            {
                ruleActuator.Clear();
                foreach (var item in ruleActuator.ruleGroup)
                {
                    if (ruleActuator.Root.NodePoolManager.pools.TryGetValue(item.Key, out NodePool pool))
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
        /// 获取全局节点法制执行器
        /// </summary>
        public static RuleActuator GetGlobalNodeRuleActuator<T>(this Node self)
        where T : IRule
        {
            var ruleActuator = self.Root.AddComponent<RuleActuatorGroup>().GetBroadcast<T>();
            ruleActuator.AddComponent<GlobalNodeAddListener>().ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            ruleActuator.AddComponent<GlobalNodeRemoveListener>().ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
            return ruleActuator;
        }
    }

}
