
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
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type Target;

        /// <summary>
        /// 法则执行器字典
        /// </summary>
        /// <remarks>法则类型,执行器</remarks>
        public Dictionary<Type, RuleActuator> actuatorDictionary = new Dictionary<Type, RuleActuator>();

        public override string ToString()
        {
            return $"{Type.Name} : {Target}" ; 
        }

        /// <summary>
        /// 获取执行器
        /// </summary>
        public RuleActuator GetRuleActuator(Type listenerRuleType)
        {
            if (!actuatorDictionary.TryGetValue(listenerRuleType, out var actuator))
            {
                actuator = new RuleActuator();
                actuator.nodeQueue = new DynamicNodeQueue();
                actuator.nodeQueue.idQueue = new UnitQueue<long>();
                actuator.nodeQueue.removeIdDictionary = new UnitDictionary<long, int>();
                actuator.nodeQueue.nodeDictionary = new UnitDictionary<long, Node>();

                actuator.id = Root.IdManager.GetId();
                actuator.Root = Root;
                actuatorDictionary.Add(listenerRuleType, actuator);
                this.AddChildren(actuator);
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
}
