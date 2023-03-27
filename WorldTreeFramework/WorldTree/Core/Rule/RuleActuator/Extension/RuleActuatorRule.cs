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
        public static RuleActuator Load<R>(this RuleActuator ruleActuator)
        where R : IRule
        {
            var ruleType = typeof(R);

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
        public static IRuleActuator<R> GetGlobalNodeRuleActuator<R>(this INode self)
        where R : IRule
        {
            var ruleActuator = self.Root.AddComponent(out RuleActuatorGroup _).GetActuator<R>();
            ((RuleActuator)ruleActuator).nodeQueue.AddComponent(out NodeAddGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
            ((RuleActuator)ruleActuator).nodeQueue.AddComponent(out NodeRemoveGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
            return ruleActuator;
        }
    }

}
