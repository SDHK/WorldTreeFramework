
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/6 10:17

* 描述： 监听法则执行器集合

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 监听法则执行器集合
    /// </summary>
    public class ListenerRuleActuatorGroup : Node
        , IAwake
        , ChildOf<DynamicListenerRuleActuatorManager>
        , ChildOf<StaticListenerRuleActuatorManager>
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type Target;

        /// <summary>
        /// 法则执行器字典
        /// </summary>
        /// <remarks>法则类型,执行器</remarks>
        public TreeDictionary<Type, RuleActuator> actuatorDictionary;

        public override string ToString()
        {
            return $"{Type.Name} : {Target}";
        }

        /// <summary>
        /// 获取执行器
        /// </summary>
        public RuleActuator GetRuleActuator(Type listenerRuleType)
        {
            if (!actuatorDictionary.TryGetValue(listenerRuleType, out var actuator))
            {

                actuatorDictionary.Add(listenerRuleType, this.AddChild(out actuator));
            }
            return actuator;
        }

        /// <summary>
        /// 尝试获取执行器
        /// </summary>
        public bool TryGetRuleActuator(Type listenerRuleType, out RuleActuator ruleActuator)
        {
            return actuatorDictionary.TryGetValue(listenerRuleType, out ruleActuator);
        }



    }

    class ListenerRuleActuatorGroupAddRule : AddRule<ListenerRuleActuatorGroup>
    {
        public override void OnEvent(ListenerRuleActuatorGroup self)
        {
            self.AddChild(out self.actuatorDictionary);
        }
    }

    class ListenerRuleActuatorGroupRemoveRule : RemoveRule<ListenerRuleActuatorGroup>
    {
        public override void OnEvent(ListenerRuleActuatorGroup self)
        {
            self.actuatorDictionary = null;
        }
    }
}
