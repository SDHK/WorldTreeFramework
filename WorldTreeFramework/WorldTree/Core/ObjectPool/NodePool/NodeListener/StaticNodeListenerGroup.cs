
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/25 17:59

* 描述： 静态节点监听器集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 静态节点监听器集合
    /// </summary>
    public class StaticNodeListenerGroup : Node, ComponentOf<NodePool>
    {
        public TreeDictionary<Type, RuleActuatorBase> actuatorDictionary;

    }


    public static class StaticNodeListenerGroupRule
    {
        public static void TrySendStaticNodeListener(this INode node)
        {
            if (node.Core.NodePoolManager != null)
                if (node.Core.NodePoolManager.TryGetPool(node.Type, out NodePool nodePool))
                {
                    nodePool.AddComponent(out StaticNodeListenerGroup staticNodeListenerGroup);
                    //staticNodeListenerGroup

                }

        }


        public static bool TryAddRuleActuator<R>(this StaticNodeListenerGroup self, Type Target, out IRuleActuator<R> actuator)
            where R : IListenerRule
        {
            Type ruleType = typeof(R);


            if (self.actuatorDictionary.TryGetValue(ruleType, out RuleActuatorBase ruleActuator))
            {
                actuator = ruleActuator as IRuleActuator<R>; return true;
            }
            else if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, Target, out var ruleGroup))
            {

            }



            actuator = default;
            return false;
        }



    }




}
