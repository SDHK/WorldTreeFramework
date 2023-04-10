
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/6 10:17

* 描述： 监听法则执行器集合

*/

using System;

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


    public static class ListenerRuleActuatorGroupRule
    {
        /// <summary>
        /// 获取执行器
        /// </summary>
        public static RuleActuator GetRuleActuator(this ListenerRuleActuatorGroup self, Type listenerRuleType)
        {
            if (!self.actuatorDictionary.TryGetValue(listenerRuleType, out var actuator))
            {
                self.actuatorDictionary.Add(listenerRuleType, self.AddChild(out actuator));
            }
            return actuator;
        }

        /// <summary>
        /// 尝试获取执行器
        /// </summary>
        public static bool TryGetRuleActuator(this ListenerRuleActuatorGroup self, Type listenerRuleType, out RuleActuator ruleActuator)
        {
            return self.actuatorDictionary.TryGetValue(listenerRuleType, out ruleActuator);
        }
    }
}
