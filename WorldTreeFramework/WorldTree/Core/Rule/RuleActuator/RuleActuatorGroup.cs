
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

        public RuleActuator GetActuator<T>() => GetActuator(typeof(T));

        public RuleActuator GetActuator(Type type)
        {
            if (!ruleActuatorDictionary.TryGetValue(type, out var ruleActuator))
            {
                this.AddChild(out ruleActuator).Load(type);
                ruleActuatorDictionary.Add(type, ruleActuator);
            }
            return ruleActuator;
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
