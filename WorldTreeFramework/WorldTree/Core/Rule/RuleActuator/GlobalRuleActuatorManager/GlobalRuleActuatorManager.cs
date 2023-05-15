
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 全局法则执行器集合

*/

using System;

namespace WorldTree
{

    public static partial class NodeRule
    {
        /// <summary>
        /// 获取全局节点法则执行器
        /// </summary>
        public static IRuleActuator<R> GetGlobalNodeRuleActuator<R>(this INode self)
        where R : IRule
        {
            return self.Root.AddComponent(out GlobalRuleActuatorManager _).GetGlobalRuleActuator<R>();
        }
    }

    /// <summary>
    /// 全局法则执行器管理器
    /// </summary>
    public class GlobalRuleActuatorManager : Node, ComponentOf<WorldTreeRoot>
    {
        
        public UnitDictionary<Type, RuleActuator> ruleActuatorDictionary;

        public IRuleActuator<R> GetGlobalRuleActuator<R>()
            where R : IRule
        {
            var ruleType = typeof(R);
            if (!ruleActuatorDictionary.TryGetValue(ruleType, out var ruleActuator))
            {
                this.AddChild(out ruleActuator).LoadGlobalNode<R>();
                ruleActuatorDictionary.Add(ruleType, ruleActuator);

                ruleActuator.nodeQueue.AddComponent(out NodeAddGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
                ruleActuator.nodeQueue.AddComponent(out NodeRemoveGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
            }
            return (IRuleActuator<R>)ruleActuator;
        }
    }

    class GlobalRuleActuatorGroupAddRule : AddRule<GlobalRuleActuatorManager>
    {
        public override void OnEvent(GlobalRuleActuatorManager self)
        {
            self.PoolGet(out self.ruleActuatorDictionary);
        }
    }

    class GlobalRuleActuatorGroupRemoveRule : RemoveRule<GlobalRuleActuatorManager>
    {
        public override void OnEvent(GlobalRuleActuatorManager self)
        {
            self.ruleActuatorDictionary.Dispose();
            self.ruleActuatorDictionary = null;
        }
    }

}
