
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
    public class RuleActuatorGroup : Node
    {
        public UnitDictionary<Type, RuleActuator> broadcasts;

        public RuleActuator GetBroadcast<T>() => GetBroadcast(typeof(T));

        public RuleActuator GetBroadcast(Type type)
        {
            if (!broadcasts.TryGetValue(type, out var broadcast))
            {
                broadcast = this.AddChildren<RuleActuator>().Load(type);
                broadcasts.Add(type, broadcast);
            }
            return broadcast;
        }
    }

    class RuleActuatorGroupAddSystem : AddRule<RuleActuatorGroup>
    {
        public override void OnEvent(RuleActuatorGroup self)
        {
            self.PoolGet(out self.broadcasts);
        }
    }

    class RuleActuatorGroupRemoveSystem : RemoveRule<RuleActuatorGroup>
    {
        public override void OnEvent(RuleActuatorGroup self)
        {
            self.broadcasts.Dispose();
        }
    }

}
