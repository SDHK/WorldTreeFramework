/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 混合型监听器法则执行器组

*/

namespace WorldTree
{
    /// <summary>
    /// 混合型监听器法则执行器组
    /// </summary>
    public class HybridListenerRuleActuatorGroup : CoreNode, ComponentOf<ReferencedPool>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 监听器执行器字典集合
        /// </summary>
        public UnitDictionary<long, HybridListenerRuleActuator> actuatorDictionary = new();
    }

    public static class HybridListenerRuleActuatorGroupRule
    {

        /// <summary>
        /// 获取节点监听执行器
        /// </summary>
        public static IRuleActuator<R> GetListenerActuator<R>(this INode node)
           where R : IListenerRule
        {
            if (node.Core.ReferencedPoolManager != null)
            {
                if (node.Core.ReferencedPoolManager.TryGetPool(node.Type, out ReferencedPool nodePool))
                {

                    if (nodePool.AddNewComponent(out HybridListenerRuleActuatorGroup _).TryAddRuleActuator(node.Type, out IRuleActuator<R> actuator))
                    {
                        return actuator;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 添加监听执行器,并自动填装监听器
        /// </summary>
        public static bool TryAddRuleActuator<R>(this HybridListenerRuleActuatorGroup self, long nodeType, out IRuleActuator<R> actuator)
            where R : IListenerRule
        {
            long ruleType = TypeInfo<R>.HashCode64;

            //执行器已存在，直接返回
            if (self.actuatorDictionary.TryGetValue(ruleType, out HybridListenerRuleActuator ruleActuator))
            {
                actuator = ruleActuator as IRuleActuator<R>;
                //检测是否存在目标法则集合，不存在则添加
                if (ruleActuator.staticListenerRuleActuator == null)
                {
                    if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, nodeType, out RuleGroup staticRuleGroup1))
                    {
                        ruleActuator.AddComponent(out ruleActuator.staticListenerRuleActuator, staticRuleGroup1).RuleActuatorAddListener();
                    }
                }
                if (ruleActuator.dynamicListenerRuleActuator == null)
                {
                    if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, TypeInfo<INode>.HashCode64, out RuleGroup dynamicRuleGroup1))
                    {
                        ruleActuator.AddComponent(out ruleActuator.dynamicListenerRuleActuator, dynamicRuleGroup1).RuleActuatorAddListener(nodeType);
                    }
                }
                return true;
            }
            //执行器不存在，检测获取目标法则集合，并新建执行器
            bool checkStatic = self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, nodeType, out RuleGroup staticRuleGroup);
            bool checkDynamic = self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, TypeInfo<INode>.HashCode64, out RuleGroup dynamicRuleGroup);
            if (checkStatic || checkDynamic)
            {
                self.actuatorDictionary.Add(ruleType, self.AddNewComponent(out ruleActuator));
                if (checkStatic) ruleActuator.AddComponent(out ruleActuator.staticListenerRuleActuator, staticRuleGroup).RuleActuatorAddListener();
                if (checkDynamic) ruleActuator.AddComponent(out ruleActuator.dynamicListenerRuleActuator, dynamicRuleGroup).RuleActuatorAddListener(nodeType);
            }

            actuator = ruleActuator as IRuleActuator<R>;
            return actuator != null;
        }

        #region 判断添加监听器

        #region 静态监听器

        /// <summary>
        /// 尝试添加监听器监听目标池
        /// </summary>
        /// <remarks>只在目标池存在时有效</remarks>
        public static void TryAddStaticListener(this ReferencedPoolManager self, INodeListener listener)
        {
            //判断是否为监听器
            if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    //判断监听法则集合 是否有这个 监听器节点类型
                    if (ruleGroup.Value.ContainsKey(listener.Type))
                    {
                        foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                        {
                            //是否有这个目标缓存池
                            if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
                            {
                                //是否有监听器集合组件
                                if (nodePool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
                                {
                                    //是否有这个监听类型的执行器
                                    if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator listenerRuleActuator))
                                    {
                                        if (listenerRuleActuator.staticListenerRuleActuator != null)
                                        {
                                            //监听器添加到执行器
                                            listenerRuleActuator.staticListenerRuleActuator.TryAdd(listener);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除这个监听器
        /// </summary>
        /// <remarks>只在目标池存在时有效</remarks>
        public static void RemoveStaticListener(this ReferencedPoolManager self, INodeListener listener)
        {
            //判断是否为监听器
            if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    //判断监听法则集合 是否有这个 监听器节点类型
                    if (ruleGroup.Value.ContainsKey(listener.Type))
                    {
                        foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                        {
                            //是否有这个目标池
                            if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
                            {
                                //是否有监听器集合组件
                                if (nodePool.TryGetComponent(out HybridListenerRuleActuatorGroup ListenerRuleActuatorGroup))
                                {
                                    //是否有这个监听类型的执行器
                                    if (ListenerRuleActuatorGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out HybridListenerRuleActuator listenerRuleActuator))
                                    {
                                        if (listenerRuleActuator.staticListenerRuleActuator != null)
                                        {
                                            //监听器添加到执行器
                                            listenerRuleActuator.staticListenerRuleActuator.Remove(listener);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region 动态监听器


        #endregion


        #endregion


    }



}
