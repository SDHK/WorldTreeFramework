
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/25 18:00

* 描述： 动态节点监听器集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 动态节点监听器集合
    /// </summary>
    public class DynamicNodeListenerGroup : CoreNode, ComponentOf<ReferencedPool>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 监听器执行器字典集合
        /// </summary>
        public UnitDictionary<long, ListenerRuleActuator> actuatorDictionary = new();
    }


    public static class DynamicNodeListenerGroupRule
    {
        /// <summary>
        /// 获取以实体类型为目标的 监听系统执行器
        /// </summary>
        public static void SendDynamicNodeListener<R>(this INode node)
            where R : IListenerRule
        {
            if (node.Core.ReferencedPoolManager != null)
                if (node.Core.ReferencedPoolManager.TryGetPool(node.Type, out ReferencedPool nodePool))
                {
                    if (nodePool.AddNewComponent(out DynamicNodeListenerGroup _).TryAddRuleActuator(node.Type, out IRuleActuator<R> actuator))
                    {

                        actuator.Send(node);
                    }
                }
        }

        #region 判断添加监听执行器

        /// <summary>
        /// 添加静态监听执行器,并自动填装监听器
        /// </summary>
        public static bool TryAddRuleActuator<R>(this DynamicNodeListenerGroup self, long target, out IRuleActuator<R> actuator)
            where R : IListenerRule
        {
            long ruleType = TypeInfo<R>.HashCode64;

            //执行器已存在，直接返回
            if (self.actuatorDictionary.TryGetValue(ruleType, out ListenerRuleActuator ruleActuator))
            {
                actuator = ruleActuator as IRuleActuator<R>; return true;
            }
            //执行器不存在，检测获取目标法则集合，并新建执行器
            else if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, TypeInfo<INode>.HashCode64, out var ruleGroup))
            {
                self.actuatorDictionary.Add(ruleType, self.AddNewChild(out ruleActuator, ruleGroup));

                self.RuleActuatorAddListener(ruleActuator, target);

                actuator = ruleActuator as IRuleActuator<R>;
                return true;
            }

            //监听目标法则不存在
            actuator = default;
            return false;
        }


        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        private static void RuleActuatorAddListener(this DynamicNodeListenerGroup self, ListenerRuleActuator actuator, long target)
        {
            //遍历法则集合获取监听器类型
            foreach (var listenerType in actuator.ruleGroup)
            {
                //从池里拿到已存在的监听器
                if (self.Core.ReferencedPoolManager.TryGetPool(listenerType.Key, out ReferencedPool listenerPool))
                {
                    //全部注入到执行器
                    foreach (var listenerPair in listenerPool)
                    {
                        INodeListener nodeListener = (listenerPair.Value as INodeListener);

                        //判断目标是否被该监听器监听
                        if (nodeListener.listenerTarget != 0)
                        {
                            if (nodeListener.listenerState == ListenerState.Node)
                            {
                                //判断是否全局监听 或 是指定的目标类型
                                if (nodeListener.listenerTarget == TypeInfo<INode>.HashCode64 || nodeListener.listenerTarget == target)
                                {
                                    actuator.TryAdd(nodeListener);
                                }
                            }
                            else if (nodeListener.listenerState == ListenerState.Rule)
                            {
                                //判断的实体类型是否拥有目标系统
                                if (self.Core.RuleManager.TryGetRuleList(target, nodeListener.listenerTarget, out _))
                                {
                                    actuator.TryAdd(nodeListener);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion


        #region 判断监听器

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        public static void TryAddDynamicListener(this ReferencedPoolManager self, INodeListener node)
        {
            if (node.listenerTarget != 0)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == TypeInfo<INode>.HashCode64)
                    {
                        self.AddAllTarget(node);
                    }
                    else
                    {
                        self.AddNodeTarget(node, node.listenerTarget);
                    }
                }
                else if (node.listenerState == ListenerState.Rule)
                {
                    self.AddRuleTarget(node, node.listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器添加 所有节点
        /// </summary>
        private static void AddAllTarget(this ReferencedPoolManager self, INodeListener node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.HashCode64, out var ruleGroupDictionary))
            {
                //遍历获取动态法则集合
                foreach (var ruleGroup in ruleGroupDictionary)
                {
                    //判断监听法则集合 是否有这个 监听器节点类型
                    if (ruleGroup.Value.ContainsKey(node.Type))
                    {
                        //遍历现有池
                        foreach (var poolPair in self.pools)
                        {
                            //尝试获取动态执行器集合组件
                            if (poolPair.Value.TryGetComponent(out DynamicNodeListenerGroup dynamicNodeListenerGroup))
                            {
                                //从执行器集合 提取这个 监听法则类型 的监听执行器，进行添加
                                if (dynamicNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out var ruleActuator))
                                {
                                    ruleActuator.TryAdd(node);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private static void AddRuleTarget(this ReferencedPoolManager self, INodeListener node, long targetRuleType)
        {
            //获取法则集合
            if (node.Core.RuleManager.TryGetRuleGroup(targetRuleType, out var targetRuleGroup))
            {
                //遍历法则集合
                foreach (var targetNode_RuleList in targetRuleGroup)
                {
                    self.AddNodeTarget(node, targetNode_RuleList.Key);
                }
            }
        }

        /// <summary>
        /// 监听器添加 节点目标
        /// </summary>
        private static void AddNodeTarget(this ReferencedPoolManager self, INodeListener node, long listenerTarget)
        {
            if (self.Core.NodePoolManager.TryGetPool(listenerTarget, out NodePool listenerPool))
            {
                if (listenerPool.TryGetComponent(out DynamicNodeListenerGroup dynamicNodeListenerGroup))
                {
                    //获取 INode 动态目标 法则集合集合
                    if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.HashCode64, out var ruleGroupDictionary))
                    {
                        //遍历获取动态法则集合，并添加自己
                        foreach (var ruleGroup in ruleGroupDictionary)
                        {
                            //判断监听法则集合 是否有这个 监听器节点类型
                            if (ruleGroup.Value.ContainsKey(node.Type))
                            {
                                if (dynamicNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out ListenerRuleActuator ruleActuator))
                                {
                                    ruleActuator.TryAdd(node);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region 移除


        /// <summary>
        /// 监听器根据标记移除目标
        /// </summary>
        public static void RemoveDynamicListener(this ReferencedPoolManager self, INodeListener node)
        {
            if (node.listenerTarget != 0)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == TypeInfo<INode>.HashCode64)
                    {
                        self.RemoveAllTarget(node);
                    }
                    else
                    {
                        self.RemoveNodeTarget(node, node.listenerTarget);
                    }
                }
                else if (node.listenerState == ListenerState.Rule)
                {
                    self.RemoveRuleTarget(node, node.listenerTarget);
                }
                node.listenerTarget = 0;
                node.listenerState = ListenerState.Not;
            }
        }


        /// <summary>
        /// 监听器移除 所有节点
        /// </summary>
        private static void RemoveAllTarget(this ReferencedPoolManager self, INodeListener node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.HashCode64, out var ruleGroupDictionary))
            {
                //遍历获取动态法则集合
                foreach (var ruleGroupPair in ruleGroupDictionary)
                {
                    //判断监听法则集合 是否有这个 监听器节点类型
                    if (ruleGroupPair.Value.ContainsKey(node.Type))
                    {
                        //遍历现有池
                        foreach (var poolPair in self.pools)
                        {
                            //尝试获取动态执行器集合组件
                            if (poolPair.Value.TryGetComponent(out DynamicNodeListenerGroup dynamicNodeListenerGroup))
                            {
                                //从执行器集合 提取这个 监听法则类型 的监听执行器，进行移除
                                if (dynamicNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroupPair.Key, out var ruleActuator))
                                {
                                    ruleActuator.Remove(node);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监听器移除 系统目标
        /// </summary>
        private static void RemoveRuleTarget(this ReferencedPoolManager self, INodeListener node, long targetRuleType)
        {
            //获取法则集合
            if (node.Core.RuleManager.TryGetRuleGroup(targetRuleType, out var targetRuleGroup))
            {
                //遍历法则集合
                foreach (var targetNode_RuleList in targetRuleGroup)
                {
                    self.RemoveNodeTarget(node, targetNode_RuleList.Key);
                }
            }
        }


        /// <summary>
        /// 监听器移除 节点目标
        /// </summary>
        private static void RemoveNodeTarget(this ReferencedPoolManager self, INodeListener node, long listenerTarget)
        {
            if (self.Core.ReferencedPoolManager.TryGetPool(listenerTarget, out ReferencedPool listenerPool))
            {
                if (listenerPool.TryGetComponent(out DynamicNodeListenerGroup dynamicNodeListenerGroup))
                {
                    //获取 INode 动态目标 法则集合集合
                    if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(TypeInfo<INode>.HashCode64, out var ruleGroupDictionary))
                    {
                        //遍历获取动态法则集合，移除自己
                        foreach (var ruleGroup in ruleGroupDictionary)
                        {
                            //判断监听法则集合 是否有这个 监听器节点类型
                            if (ruleGroup.Value.ContainsKey(node.Type))
                            {
                                if (dynamicNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out ListenerRuleActuator ruleActuator))
                                {
                                    ruleActuator.Remove(node);
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #endregion

    }
}
