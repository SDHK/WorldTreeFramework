
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 9:42

* 描述： 静态监听法则执行器管理器

*/

using System;

namespace WorldTree
{
    public static partial class NodeRule
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
        /// <remarks>
        /// <para>可能监听器早已存在，所以会主动新建监听执行器，并查询节点引用池判断添加监听器</para>
        /// <para>只会在执行器新建时填装，后续获取不会再填装了，只返回执行器</para>
        /// </remarks>
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
    public class StaticListenerRuleActuatorManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 目标类型 法则执行器字典
        /// </summary>
        /// <remarks>目标类型《系统，法则执行器》</remarks>
        public TreeDictionary<Type, ListenerRuleActuatorGroup> ListenerActuatorGroupDictionary;
    }

    class StaticListenerRuleActuatorManagerAddRule : AddRule<StaticListenerRuleActuatorManager>
    {
        public override void OnEvent(StaticListenerRuleActuatorManager self)
        {
            self.AddChild(out self.ListenerActuatorGroupDictionary);
        }
    }
    class StaticListenerRuleActuatorManagerRemoveRule : RemoveRule<StaticListenerRuleActuatorManager>
    {
        public override void OnEvent(StaticListenerRuleActuatorManager self)
        {
            self.ListenerActuatorGroupDictionary = null;
        }
    }

    public static class StaticListenerRuleActuatorManagerRule
    {

        #region 判断监听器

        /// <summary>
        /// 检测添加静态监听器
        /// </summary>
        public static void TryAddListener(this StaticListenerRuleActuatorManager self, INode listener)
        {
            //判断是否为监听器
            if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                    {
                        if (self.TryGetRuleActuator(ruleList.Key, ruleGroup.Key, out var actuator))
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
        public static void RemoveListener(this StaticListenerRuleActuatorManager self, INode listener)
        {
            //判断是否为监听器
            if (self.Core.RuleManager.ListenerRuleTargetGroupDictionary.TryGetValue(listener.Type, out var ruleGroupDictionary))
            {
                foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
                {
                    foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
                    {
                        if (self.TryGetRuleActuator(ruleList.Key, ruleGroup.Key, out var actuator))
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
        public static bool TryAddRuleActuator<R>(this StaticListenerRuleActuatorManager self, Type Target, out RuleActuator actuator)
            where R : IListenerRule
        {
            Type ruleType = typeof(R);

            //判断是否有组
            if (self.TryGetGroup(Target, out var group))
            {
                //判断是否有执行器
                if (group.TryGetRuleActuator(ruleType, out actuator)) { return true; }

                //没有执行器 则判断这个目标类型是是否有监听法则集合
                else if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, Target, out var ruleGroup))
                {
                    //新建执行器
                    actuator = group.GetRuleActuator(ruleType);
                    actuator.ruleGroup = ruleGroup;
                    self.RuleActuatorAddListener(actuator);
                    return true;
                }
            }
            //没有组则判断这个目标类型是否有监听法则集合
            else if (self.Core.RuleManager.TryGetTargetRuleGroup(ruleType, Target, out var ruleGroup))
            {
                //新建组和执行器
                actuator = self.GetGroup(Target).GetRuleActuator(ruleType);
                actuator.ruleGroup = ruleGroup;
                self.RuleActuatorAddListener(actuator);
                return true;
            }
            actuator = null;
            return false;
        }

        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        private static void RuleActuatorAddListener(this StaticListenerRuleActuatorManager self, RuleActuator actuator)
        {
            foreach (var listenerType in actuator.ruleGroup)//遍历法则集合获取监听器类型
            {
                //从池里拿到已存在的监听器
                if (self.Core.NodePoolManager.m_Pools.TryGetValue(listenerType.Key, out NodePool listenerPool))
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
        public static bool TryGetRuleActuator(this StaticListenerRuleActuatorManager self, Type Target, Type RuleType, out RuleActuator actuator)
        {
            if (self.ListenerActuatorGroupDictionary.TryGetValue(Target, out var group))
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
        public static ListenerRuleActuatorGroup GetGroup(this StaticListenerRuleActuatorManager self, Type Target)
        {
            if (!self.ListenerActuatorGroupDictionary.TryGetValue(Target, out var group))
            {
                self.ListenerActuatorGroupDictionary.Add(Target, self.AddChild(out group));
                group.Target = Target;
            }
            return group;
        }

        /// <summary>
        /// 尝试获取执行器集合
        /// </summary>
        public static bool TryGetGroup(this StaticListenerRuleActuatorManager self, Type target, out ListenerRuleActuatorGroup group)
        {
            return self.ListenerActuatorGroupDictionary.TryGetValue(target, out group);
        }

        #endregion

    }

}
