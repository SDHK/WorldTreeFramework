/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:55

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class NodeListenerStaticRule
    {
        /// <summary>
        /// 动态监听器切换目标
        /// </summary>
        public static bool ListenerSwitchesTarget(this Node self, Type targetType, ListenerState state)
        {
            if (self.listenerTarget != targetType)
            {
                //判断是否为监听器
                if (self.Root.RuleManager.DynamicListenerTypeHash.Contains(self.Type))
                {
                    self.DynamicListenerRemove();
                    self.listenerTarget = targetType;
                    self.listenerState = state;
                    self.DynamicListenerAdd();
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 动态监听器切换节点目标
        /// </summary>
        public static bool ListenerSwitchesEntity<T>(this Node self)
            where T : Node
        {
            return self.ListenerSwitchesTarget(typeof(T), ListenerState.Node);
        }
        /// <summary>
        /// 动态监听器切换系统目标
        /// </summary>
        public static bool ListenerSwitchesSystem<T>(this Node self)
            where T : IRule
        {
            return self.ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
        }

        /// <summary>
        /// 动态监听器清除目标
        /// </summary>
        public static void ListenerClearTarget(this Node self)
        {
            self.DynamicListenerRemove();
            self.listenerTarget = null;
            self.listenerState = ListenerState.Not;
        }

        #region 私有

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        private static void DynamicListenerAdd(this Node self)
        {
            if (self.listenerTarget != null)
            {
                if (self.listenerState == ListenerState.Node)
                {
                    if (self.listenerTarget == typeof(Node))
                    {
                        self.DynamicListenerAddAll();
                    }
                    else
                    {
                        self.DynamicListenerAddTarget(self.listenerTarget);
                    }
                }
                else if (self.listenerState == ListenerState.Rule)
                {
                    self.DynamicListenerAddSystem(self.listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器添加 所有节点
        /// </summary>
        private static void DynamicListenerAddAll(this Node self)
        {
            //获取 Node 动态目标 法则集合集合
            if (self.Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var ruleGroupDictionary))
            {
                //遍历现有执行器
                foreach (var BroadcastGroup in self.Root.DynamicListenerRuleActuatorManager.ListenerActuatorGroupDictionary)
                {
                    //遍历获取动态法则集合，并添加自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        if (BroadcastGroup.Value.TryGetRuleActuator(ruleGroup.Key, out var broadcast))
                        {
                            broadcast.Enqueue(self);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private static void DynamicListenerAddSystem(this Node self, Type type)
        {
            //获取法则集合
            if (self.Root.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历法则集合
                foreach (var targetSystems in targetSystemGroup)
                {
                    self.DynamicListenerAddTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器添加 节点目标
        /// </summary>
        private static void DynamicListenerAddTarget(this Node self, Type type)
        {
            if (self.Root.DynamicListenerRuleActuatorManager.TryGetGroup(type, out var broadcastGroup))
            {
                //获取 Node 动态目标 法则集合集合
                if (self.Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并添加自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        broadcastGroup.GetRuleActuator(ruleGroup.Key).Enqueue(self);
                    }
                }
            }
        }



        #endregion

        #region 移除

        /// <summary>
        /// 监听器根据标记移除目标
        /// </summary>
        private static void DynamicListenerRemove(this Node self)
        {
            if (self.listenerTarget != null)
            {
                if (self.listenerState == ListenerState.Node)
                {
                    if (self.listenerTarget == typeof(Node))
                    {
                        self.DynamicListenerRemoveAll();
                    }
                    else
                    {
                        self.DynamicListenerRemoveTarget(self.listenerTarget);
                    }
                }
                else if (self.listenerState == ListenerState.Rule)
                {
                    self.DynamicListenerRemoveSystem(self.listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器移除 所有节点
        /// </summary>
        private static void DynamicListenerRemoveAll(this Node self)
        {
            //获取 Node 动态目标 法则集合集合
            if (self.Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var ruleGroupDictionary))
            {
                //遍历现有全部池
                foreach (var BroadcastGroup in self.Root.DynamicListenerRuleActuatorManager.ListenerActuatorGroupDictionary)
                {
                    //遍历获取动态法则集合，并移除自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        if (BroadcastGroup.Value.TryGetRuleActuator(ruleGroup.Key, out var broadcast))
                        {
                            broadcast.Remove(self);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 监听器移除 系统目标
        /// </summary>
        private static void DynamicListenerRemoveSystem(this Node self, Type type)
        {
            //获取法则集合
            if (self.Root.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历法则集合
                foreach (var targetSystems in targetSystemGroup)
                {
                    self.DynamicListenerRemoveTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器移除 节点目标
        /// </summary>
        private static void DynamicListenerRemoveTarget(this Node self, Type type)
        {
            if (self.Root.DynamicListenerRuleActuatorManager.TryGetGroup(type, out var broadcastGroup))
            {
                //获取 Node 动态目标 法则集合集合
                if (self.Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var ruleGroupDictionary))
                {
                    //遍历获取动态法则集合，并移除自己
                    foreach (var ruleGroup in ruleGroupDictionary)
                    {
                        broadcastGroup.GetRuleActuator(ruleGroup.Key).Remove(self);
                    }
                }
            }
        }
        #endregion
        #endregion

    }
}
