
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 法则执行器集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 法则执行器集合
    /// </summary>
    public class RuleActuatorGroup : Node, ComponentOf<WorldTreeRoot>
    {
        public UnitDictionary<Type, RuleActuator> ruleActuatorDictionary;

        public IRuleActuator<R> GetActuator<R>()
            where R : IRule
        {

            var ruleType = typeof(R);
            if (!ruleActuatorDictionary.TryGetValue(ruleType, out var ruleActuator))
            {
                this.AddChild(out ruleActuator).Load<R>();
                ruleActuatorDictionary.Add(ruleType, ruleActuator);
            }
            return (IRuleActuator<R>)ruleActuator;
        }
    }

    class RuleActuatorGroupAddRule : AddRule<RuleActuatorGroup>
    {
        public override void OnEvent(RuleActuatorGroup self)
        {
            self.PoolGet(out self.ruleActuatorDictionary);
        }
    }

    class RuleActuatorGroupRemoveRule : RemoveRule<RuleActuatorGroup>
    {
        public override void OnEvent(RuleActuatorGroup self)
        {
            self.ruleActuatorDictionary.Dispose();
        }
    }

}
