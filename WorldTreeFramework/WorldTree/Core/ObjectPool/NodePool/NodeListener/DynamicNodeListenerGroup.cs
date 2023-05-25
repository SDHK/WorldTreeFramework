
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/25 18:00

* 描述： 动态节点监听器集合

*/

using System;
using Unity.VisualScripting;

namespace WorldTree
{
    /// <summary>
    /// 动态节点监听器集合
    /// </summary>
    public class DynamicNodeListenerGroup : Node, ComponentOf<NodePool>
    {
        public TreeDictionary<Type, RuleActuator> actuatorDictionary;


    }

    public static class DynamicNodeListenerGroupRule
    {

        public static void TrySendDynamicNodeListener(this INode node)
        {
            if (node.Core.NodePoolManager != null)
                if (node.Core.NodePoolManager.TryGetPool(node.Type, out NodePool nodePool))
                {
                    nodePool.AddComponent(out DynamicNodeListenerGroup dynamicNodeListenerGroup);

                }

        }
    }
}
