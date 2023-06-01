
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 9:42

* 描述： 动态监听法则执行器管理器

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 动态监听法则执行器管理器
    /// </summary>
    public class DynamicListenerRuleActuatorManager : Node, ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 目标类型 法则执行器字典
        /// </summary>
        /// <remarks>目标类型《系统，法则执行器》</remarks>
        public TreeDictionary<Type, ListenerRuleActuatorGroup> ListenerActuatorGroupDictionary;
    }

    public class DynamicListenerRuleActuatorManagerAddRule : AddRule<DynamicListenerRuleActuatorManager>
    {
        public override void OnEvent(DynamicListenerRuleActuatorManager self)
        {
            self.AddChild(out self.ListenerActuatorGroupDictionary);
        }
    }

    class DynamicListenerRuleActuatorManagerRemoveRule : RemoveRule<DynamicListenerRuleActuatorManager>
    {
        public override void OnEvent(DynamicListenerRuleActuatorManager self)
        {
            self.ListenerActuatorGroupDictionary = null;
        }
    }

    public static class DynamicListenerRuleActuatorManagerRule
    {

        #region 判断监听器

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        public static void ListenerAdd(this DynamicListenerRuleActuatorManager self, INodeListener node)
        {
            if (node.listenerTarget != null)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == typeof(INode))
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
        private static void AddAllTarget(this DynamicListenerRuleActuatorManager self, INodeListener node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
            {
                //遍历获取动态法则集合
                foreach (var ruleGroup in ruleGroupDictionary)
                {
                    //判断监听法则集合 是否有这个 监听器节点类型
                    if (ruleGroup.Value.ContainsKey(node.Type))
                    {
                        //遍历现有执行器
                        foreach (var ActuatorGroup in self.ListenerActuatorGroupDictionary)
                        {
                            //从执行器集合 提取这个 监听法则类型 的执行器，进行添加
                            if (ActuatorGroup.Value.TryGetRuleActuator(ruleGroup.Key, out var ruleActuator))
                            {
                                ruleActuator.TryAdd(node);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private static void AddRuleTarget(this DynamicListenerRuleActuatorManager self, INodeListener node, Type targetRuleType)
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
        private static void AddNodeTarget(this DynamicListenerRuleActuatorManager self, INodeListener node, Type listenerTarget)
        {
            if (self.TryGetGroup(listenerTarget, out var ActuatorGroup))
            {
                //获取 INode 动态目标 法则集合集合
                if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并添加自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        //判断监听法则集合 是否有这个 监听器节点类型
                        if (ruleGroup.Value.ContainsKey(node.Type))
                        {
                            if (ActuatorGroup.TryGetRuleActuator(ruleGroup.Key, out ListenerRuleActuator ruleActuator))
                            {
                                ruleActuator.TryAdd(node);
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
        public static void ListenerRemove(this DynamicListenerRuleActuatorManager self, INodeListener node)
        {
            if (node.listenerTarget != null)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == typeof(INode))
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
            }
        }

        /// <summary>
        /// 监听器移除 所有节点
        /// </summary>
        private static void RemoveAllTarget(this DynamicListenerRuleActuatorManager self, INodeListener node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
            {
                //遍历现有全部池
                foreach (var BroadcastGroup in self.ListenerActuatorGroupDictionary)
                {
                    //遍历获取动态法则集合，并移除自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        if (BroadcastGroup.Value.TryGetRuleActuator(ruleGroup.Key, out var broadcast))
                        {
                            broadcast.Remove(node);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 监听器移除 系统目标
        /// </summary>
        private static void RemoveRuleTarget(this DynamicListenerRuleActuatorManager self, INodeListener node, Type type)
        {
            //获取法则集合
            if (node.Core.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历法则集合
                foreach (var targetSystems in targetSystemGroup)
                {
                    self.RemoveNodeTarget(node, targetSystems.Key);
                }
            }
        }


        /// <summary>
        /// 监听器移除 节点目标
        /// </summary>
        private static void RemoveNodeTarget(this DynamicListenerRuleActuatorManager self, INodeListener node, Type type)
        {
            if (self.TryGetGroup(type, out var actuatorGroup))
            {
                //获取 INode 动态目标 法则集合集合
                if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并移除自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        if (actuatorGroup.TryGetRuleActuator(ruleGroup.Key, out ListenerRuleActuator ruleActuator))
                        {
                            ruleActuator.Remove(node);
                        }
                    }
                }
            }
        }
        #endregion


        #endregion


        #region 获取执行器

        /// <summary>
        /// 尝试添加动态执行器
        /// </summary>
        public static bool TryAddRuleActuator<R>(this DynamicListenerRuleActuatorManager self, Type Target, out ListenerRuleActuator actuator)
        where R : IListenerRule
        {
            Type RuleType = typeof(R);


            //判断是否有组
            if (self.TryGetGroup(Target, out var group))
            {
                //判断是否有执行器
                if (group.TryGetRuleActuator(RuleType, out actuator)) { return true; }

                //没有执行器 则判断这个系统类型是否有动态类型监听法则集合
                else if (self.Core.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(INode), out var ruleGroup))
                {
                    //新建执行器
                    actuator = group.GetRuleActuator(RuleType);
                    actuator.ruleGroup = ruleGroup;
                    self.RuleActuatorAddListener(actuator, Target);
                    return true;
                }
            }
            else if (self.Core.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(INode), out var ruleGroup))
            {

                //新建组和执行器
                actuator = self.GetGroup(Target).GetRuleActuator(RuleType);
                actuator.ruleGroup = ruleGroup;

                self.RuleActuatorAddListener(actuator, Target);
                return true;
            }
            actuator = null;
            return false;
        }

        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        private static void RuleActuatorAddListener(this DynamicListenerRuleActuatorManager self, GlobalRuleActuatorBase actuator, Type Target)
        {
            foreach (var listenerType in actuator.ruleGroup)//遍历监听类型
            {
                //获取监听器对象池
                if (self.Core.NodePoolManager.m_Pools.TryGetValue(listenerType.Key, out NodePool listenerPool))
                {
                    //遍历已存在的监听器
                    foreach (var listener in listenerPool.Nodes)
                    {
                        INodeListener nodeListener = (listener.Value as INodeListener);

                        //判断目标是否被该监听器监听
                        if (nodeListener.listenerTarget != null)
                        {
                            if (nodeListener.listenerState == ListenerState.Node)
                            {
                                //判断是否全局监听 或 是指定的目标类型
                                if (nodeListener.listenerTarget == typeof(INode) || nodeListener.listenerTarget == Target)
                                {
                                    actuator.TryAdd(nodeListener);
                                }
                            }
                            else if (nodeListener.listenerState == ListenerState.Rule)
                            {
                                //判断的实体类型是否拥有目标系统
                                if (self.Core.RuleManager.TryGetRuleList(Target, nodeListener.listenerTarget, out _))
                                {

                                    actuator.TryAdd(nodeListener);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 尝试获取动态执行器
        /// </summary>
        public static bool TryGetRuleActuator(this DynamicListenerRuleActuatorManager self, Type Target, Type RuleType, out ListenerRuleActuator actuator)
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
        private static ListenerRuleActuatorGroup GetGroup(this DynamicListenerRuleActuatorManager self, Type Target)
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
        private static bool TryGetGroup(this DynamicListenerRuleActuatorManager self, Type target, out ListenerRuleActuatorGroup group)
        {
            return self.ListenerActuatorGroupDictionary.TryGetValue(target, out group);
        }

        #endregion

    }
}
