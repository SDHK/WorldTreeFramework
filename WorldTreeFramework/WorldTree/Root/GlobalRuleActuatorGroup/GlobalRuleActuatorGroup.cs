
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 全局法则执行器集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 全局法则执行器集合
    /// </summary>
    public class GlobalRuleActuatorGroup : Node, ComponentOf<WorldTreeRoot>
    {
        public UnitDictionary<Type, RuleActuator> ruleActuatorDictionary;

        public IRuleActuator<R> GetGlobalRuleActuator<R>()
            where R : IRule
        {
            var ruleType = typeof(R);
            if (!ruleActuatorDictionary.TryGetValue(ruleType, out var ruleActuator))
            {
                this.AddChild(out ruleActuator).LoadGlobalNode<R>();
                ruleActuator.nodeQueue.AddComponent(out NodeAddGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
                ruleActuator.nodeQueue.AddComponent(out NodeRemoveGlobalListener _).ListenerSwitchesTarget(typeof(R), ListenerState.Rule);
                ruleActuatorDictionary.Add(ruleType, ruleActuator);
            }
            return (IRuleActuator<R>)ruleActuator;
        }
    }

    class GlobalRuleActuatorGroupAddRule : AddRule<GlobalRuleActuatorGroup>
    {
        public override void OnEvent(GlobalRuleActuatorGroup self)
        {
            self.PoolGet(out self.ruleActuatorDictionary);
        }
    }

    class GlobalRuleActuatorGroupRemoveRule : RemoveRule<GlobalRuleActuatorGroup>
    {
        public override void OnEvent(GlobalRuleActuatorGroup self)
        {
            self.ruleActuatorDictionary.Dispose();
            self.ruleActuatorDictionary = null;
        }
    }

}
