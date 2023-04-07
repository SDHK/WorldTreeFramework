
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 9:42

* 描述： 动态监听法则执行器管理器

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{

    /// <summary>
    /// 动态监听法则执行器管理器
    /// </summary>
    public class DynamicListenerRuleActuatorManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {
        /// <summary>
        /// 目标类型 法则执行器字典
        /// </summary>
        /// <remarks>目标类型《系统，法则执行器》</remarks>
        public TreeDictionary<Type, ListenerRuleActuatorGroup> ListenerActuatorGroupDictionary;

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
        }

        #region 判断监听器

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        public void ListenerAdd(INode node)
        {
            if (node.listenerTarget != null)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == typeof(INode))
                    {
                        AddAllTarget(node);
                    }
                    else
                    {
                        AddNodeTarget(node, node.listenerTarget);
                    }
                }
                else if (node.listenerState == ListenerState.Rule)
                {
                    AddRuleTarget(node, node.listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器添加 所有节点
        /// </summary>
        private void AddAllTarget(INode node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
            {
                //遍历现有执行器
                foreach (var ActuatorGroup in ListenerActuatorGroupDictionary)
                {
                    //遍历获取动态法则集合，并添加自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        if (ActuatorGroup.Value.TryGetRuleActuator(ruleGroup.Key, out var ruleActuator))
                        {
                            ruleActuator.Enqueue(node);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private void AddRuleTarget(INode node, Type type)
        {
            //获取法则集合
            if (node.Core.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历法则集合
                foreach (var targetSystems in targetSystemGroup)
                {

                    AddNodeTarget(node, targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器添加 节点目标
        /// </summary>
        private void AddNodeTarget(INode node, Type type)
        {
            if (TryGetGroup(type, out var ActuatorGroup))
            {
                //获取 INode 动态目标 法则集合集合
                if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并添加自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        ActuatorGroup.GetRuleActuator(ruleGroup.Key).Enqueue(node);
                    }
                }
               

            }
        }
        #endregion



        #region 移除

        /// <summary>
        /// 监听器根据标记移除目标
        /// </summary>
        public void ListenerRemove(INode node)
        {
            if (node.listenerTarget != null)
            {
                if (node.listenerState == ListenerState.Node)
                {
                    if (node.listenerTarget == typeof(INode))
                    {
                        RemoveAllTarget(node);
                    }
                    else
                    {
                        RemoveNodeTarget(node, node.listenerTarget);
                    }
                }
                else if (node.listenerState == ListenerState.Rule)
                {
                    RemoveRuleTarget(node, node.listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器移除 所有节点
        /// </summary>
        private void RemoveAllTarget(INode node)
        {
            //获取 INode 动态目标 法则集合集合
            if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
            {
                //遍历现有全部池
                foreach (var BroadcastGroup in ListenerActuatorGroupDictionary)
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
        private void RemoveRuleTarget(INode node, Type type)
        {
            //获取法则集合
            if (node.Core.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历法则集合
                foreach (var targetSystems in targetSystemGroup)
                {
                    RemoveNodeTarget(node, targetSystems.Key);
                }
            }
        }


        /// <summary>
        /// 监听器移除 节点目标
        /// </summary>
        private void RemoveNodeTarget(INode node, Type type)
        {
            if (TryGetGroup(type, out var broadcastGroup))
            {
                //获取 INode 动态目标 法则集合集合
                if (node.Core.RuleManager.TargetRuleListenerGroupDictionary.TryGetValue(typeof(INode), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并移除自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        broadcastGroup.GetRuleActuator(ruleGroup.Key).Remove(node);
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
        public bool TryAddRuleActuator<R>(Type Target, out RuleActuator actuator)
        where R : IListenerRule
        {
            Type RuleType = typeof(R);


            //判断是否有组
            if (TryGetGroup(Target, out var group))
            {
                //判断是否有执行器
                if (group.TryGetRuleActuator(RuleType, out actuator)) { return true; }

                //没有执行器 则判断这个系统类型是否有动态类型监听法则集合
                else if (Core.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(INode), out var ruleGroup))
                {
                    //新建执行器
                    actuator = group.GetRuleActuator(RuleType);
                    actuator.ruleGroup = ruleGroup;
                    RuleActuatorAddListener(actuator, Target);
                    return true;
                }
            }
            else if (Core.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(INode), out var ruleGroup))
            {

                //新建组和执行器
                actuator = GetGroup(Target).GetRuleActuator(RuleType);
                actuator.ruleGroup = ruleGroup;

                RuleActuatorAddListener(actuator, Target);
                return true;
            }
            actuator = null;
            return false;
        }

        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        private void RuleActuatorAddListener(RuleActuator actuator, Type Target)
        {
            foreach (var listenerType in actuator.ruleGroup)//遍历监听类型
            {
                //获取监听器对象池
                if (Core.NodePoolManager.pools.TryGetValue(listenerType.Key, out NodePool listenerPool))
                {
                    //遍历已存在的监听器
                    foreach (var listener in listenerPool.Nodes)
                    {
                        //判断目标是否被该监听器监听
                        if (listener.Value.listenerTarget != null)
                        {
                            if (listener.Value.listenerState == ListenerState.Node)
                            {
                                //判断是否全局监听 或 是指定的目标类型
                                if (listener.Value.listenerTarget == typeof(INode) || listener.Value.listenerTarget == Target)
                                {
                                    actuator.Enqueue(listener.Value);
                                }
                            }
                            else if (listener.Value.listenerState == ListenerState.Rule)
                            {
                                //判断的实体类型是否拥有目标系统
                                if (Core.RuleManager.TryGetRuleList(Target, listener.Value.listenerTarget, out _))
                                {

                                    actuator.Enqueue(listener.Value);
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
}
