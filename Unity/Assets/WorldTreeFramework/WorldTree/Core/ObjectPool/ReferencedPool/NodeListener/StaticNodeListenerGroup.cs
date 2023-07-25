
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
    public class StaticNodeListenerGroup : CoreNode, ComponentOf<ReferencedPool>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 监听器执行器字典集合
        /// </summary>
        public UnitDictionary<Type, ListenerRuleActuator> actuatorDictionary = new UnitDictionary<Type, ListenerRuleActuator>();

    }


    public static class StaticNodeListenerGroupRule
    {

        class AddRule : AddRule<StaticNodeListenerGroup>
        {
            public override void OnEvent(StaticNodeListenerGroup self)
            {
                //self.PoolGet(out self.actuatorDictionary);
            }
        }



        /// <summary>
        /// 获取以实体类型为目标的 监听系统执行器
        /// </summary>
        public static void SendStaticNodeListener<R>(this INode node)
            where R : IListenerRule
        {
            if (node.Core.ReferencedPoolManager != null)
                if (node.Core.ReferencedPoolManager.TryGetPool(node.Type, out ReferencedPool nodePool))
                {
                    if (nodePool.AddComponent(out StaticNodeListenerGroup _).TryAddRuleActuator(node.Type, out IRuleActuator<R> actuator))
                    {
                        actuator.Send(node);
                    }
                }
        }


        #region 判断添加监听执行器



        /// <summary>
        /// 添加静态监听执行器,并自动填装监听器
        /// </summary>
        public static bool TryAddRuleActuator<R>(this StaticNodeListenerGroup self, Type target, out IRuleActuator<R> actuator)
            where R : IListenerRule
        {
            Type ruleType = typeof(R);

            //执行器已存在，直接返回
            if (self.actuatorDictionary.TryGetValue(ruleType, out ListenerRuleActuator ruleActuator))
            {
                actuator = ruleActuator as IRuleActuator<R>; return true;
            }
            //执行器不存在，检测获取目标法则集合，并新建执行器
            else if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, target, out var ruleGroup))
            {
                self.actuatorDictionary.Add(ruleType, self.AddChild(out ruleActuator, ruleGroup));
                self.RuleActuatorAddListener(ruleActuator);
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
        private static void RuleActuatorAddListener(this StaticNodeListenerGroup self, ListenerRuleActuator actuator)
        {
            //遍历法则集合获取监听器类型
            foreach (var listenerType in actuator.ruleGroup)
            {
                //从池里拿到已存在的监听器
                if (self.Core.ReferencedPoolManager.TryGetPool(listenerType.Key, out ReferencedPool listenerPool))
                {
                    //全部注入到执行器
                    foreach (var listener in listenerPool)
                    {
                        actuator.TryAdd(listener.Value);
                    }
                }
            }
        }
        #endregion


        #region 判断添加监听器

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
                            //是否有这个目标池
                            if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
                            {
                                //是否有静态监听器组件
                                if (nodePool.TryGetComponent(out StaticNodeListenerGroup staticNodeListenerGroup))
                                {
                                    //是否有这个监听类型的执行器
                                    if (staticNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out var listenerRuleActuator))
                                    {
                                        listenerRuleActuator.TryAdd(listener);//监听器添加到执行器
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
                                //是否有静态监听器组件
                                if (nodePool.TryGetComponent(out StaticNodeListenerGroup staticNodeListenerGroup))
                                {
                                    //是否有这个监听类型的执行器
                                    if (staticNodeListenerGroup.actuatorDictionary.TryGetValue(ruleGroup.Key, out var listenerRuleActuator))
                                    {
                                        listenerRuleActuator.Remove(listener);//执行器移除监听器
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion




    }




}
