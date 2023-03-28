
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 9:42

* 描述： 静态监听法则执行器管理器

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    public static class StaticListenerRuleActuatorManagerExtension
    {
        /// <summary>
        /// 获取以实体类型为目标的 监听系统执行器
        /// </summary>
        /// <remarks>
        /// <para>可能监听器早已存在，所以会主动新建监听执行器，并查询节点引用池判断添加监听器</para>
        /// <para>只会在执行器新建时填装，后续获取不会再填装了，只返回执行器</para>
        /// </remarks>
        public static bool TrySendStaticListener<R>(this INode node)
            where R : IListenerRule
        {
            if (node.Core.StaticListenerRuleActuatorManager != null)
                if (node.Core.StaticListenerRuleActuatorManager.TryAddRuleActuator<R>(node.Type, out var actuator))
                {
                    ((IRuleActuator<R>)actuator).Send(node);
                    return true;
                }

            return false;
        }

        /// <summary>
        /// 获取以实体类型为目标的 监听系统执行器
        /// </summary>
        /// <para>可能监听器早已存在，所以会主动新建监听执行器，并查询节点引用池判断添加监听器</para>
        /// <para>只会在执行器新建时填装，后续获取不会再填装了，只返回执行器</para>
        public static bool TrySendDynamicListener<R>(this INode node)
            where R : IListenerRule
        {
            if (node.Core.DynamicListenerRuleActuatorManager != null)
                if (node.Core.DynamicListenerRuleActuatorManager.TryAddRuleActuator<R>(node.Type, out var actuator))
                {
                    ((IRuleActuator<R>)actuator).Send(node);
                    return true;
                }
            return false;
        }
    }

    /// <summary>
    /// 静态监听器执行器管理器
    /// </summary>
    public class StaticListenerRuleActuatorManager : Node, ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 目标类型 法则执行器字典
        /// </summary>
        /// <remarks>目标类型《系统，法则执行器》</remarks>
        public Dictionary<Type, ListenerRuleActuatorGroup> ListenerActuatorGroupDictionary = new Dictionary<Type, ListenerRuleActuatorGroup>();

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
            ListenerActuatorGroupDictionary.Clear();
        }

        #region 判断监听器

        /// <summary>
        /// 检测添加静态监听器
        /// </summary>
        public void TryAddListener(INode listener)
        {
            //判断是否为监听器
            if (Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                    {
                        if (TryGetRuleActuator(ruleList.Key, ruleGroup.Key, out var actuator))
                        {
                            actuator.Enqueue(listener);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测移除静态监听器
        /// </summary>
        public void RemoveListener(INode listener)
        {
            //判断是否为监听器
            if (Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                    {
                        if (TryGetRuleActuator(ruleList.Key, ruleGroup.Key, out var actuator))
                        {
                            actuator.Remove(listener);
                        }
                    }
                }
            }
        }

        #endregion

        #region 获取执行器

        /// <summary>
        /// 尝试添加静态执行器
        /// </summary>
        public bool TryAddRuleActuator<R>(Type Target, out RuleActuator actuator)
            where R : IListenerRule
        {
            Type ruleType = typeof(R);

            //判断是否有组
            if (TryGetGroup(Target, out var group))
            {
                //判断是否有执行器
                if (group.TryGetRuleActuator(ruleType,out actuator)) { return true; }

                //没有执行器 则判断这个目标类型是是否有监听法则集合
                else if (Core.RuleManager.TryGetTargetRuleGroup(ruleType,Target, out var ruleGroup))
                {
                    //新建执行器
                    actuator = group.GetRuleActuator(ruleType);
                    actuator.ruleGroup = ruleGroup;
                    RuleActuatorAddListener(actuator);
                    return true;
                }
            }
            //没有组则判断这个目标类型是否有监听法则集合
            else if (Core.RuleManager.TryGetTargetRuleGroup(ruleType,Target, out var ruleGroup))
            {
                //新建组和执行器
                actuator = GetGroup(Target).GetRuleActuator(ruleType);
                actuator.ruleGroup = ruleGroup;
                RuleActuatorAddListener(actuator);
                return true;
            }
            actuator = null;
            return false;
        }

        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        private void RuleActuatorAddListener(RuleActuator actuator)
        {
            foreach (var listenerType in actuator.ruleGroup)//遍历法则集合获取监听器类型
            {
                //从池里拿到已存在的监听器
                if (Core.NodePoolManager.pools.TryGetValue(listenerType.Key, out NodePool listenerPool))
                {
                    //全部注入到执行器
                    foreach (var listener in listenerPool.Nodes)
                    {
                        actuator.Enqueue(listener.Value);
                    }
                }
            }
        }



        /// <summary>
        /// 尝试获取静态执行器
        /// </summary>
        public bool TryGetRuleActuator(Type Target, Type RuleType, out RuleActuator actuator)
        {
            if (ListenerActuatorGroupDictionary.TryGetValue(Target, out var group))
            {
                return group.TryGetRuleActuator(RuleType, out actuator);
            }
            else
            {
                actuator = null;
                return false;
            }
        }

        #endregion

        #region 获取执行器组

        /// <summary>
        /// 获取执行器集合
        /// </summary>
        public ListenerRuleActuatorGroup GetGroup(Type Target)
        {
            if (!ListenerActuatorGroupDictionary.TryGetValue(Target, out var group))
            {
                ListenerActuatorGroupDictionary.Add(Target, this.AddChild(out group));
                group.Target = Target;
            }
            return group;
        }

        /// <summary>
        /// 尝试获取执行器集合
        /// </summary>
        public bool TryGetGroup(Type target, out ListenerRuleActuatorGroup group)
        {
            return ListenerActuatorGroupDictionary.TryGetValue(target, out group);
        }

        #endregion

    }
}
