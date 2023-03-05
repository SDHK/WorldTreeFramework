
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 19:08

* 描述： 节点监听器
* 
* 用于节点动态监听全局任意节点事件

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 监听器状态
    /// </summary>
    public enum ListenerState
    {
        /// <summary>
        /// 不是监听器
        /// </summary>
        Not,
        /// <summary>
        /// 监听目标是节点
        /// </summary>
        Node,
        /// <summary>
        /// 监听目标是法则
        /// </summary>
        Rule
    }

    public abstract partial class Node
    {
        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState listenerState = ListenerState.Not;

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public Type listenerTarget = null;

        /// <summary>
        /// 动态监听器切换目标
        /// </summary>
        public bool ListenerSwitchesTarget(Type targetType, ListenerState state)
        {
            if (listenerTarget != targetType)
            {
                //判断是否为监听器
                if (Root.RuleManager.DynamicListenerTypeHash.Contains(Type))
                {
                    DynamicListenerRemove();
                    listenerTarget = targetType;
                    listenerState = state;
                    DynamicListenerAdd();
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
        public bool ListenerSwitchesEntity<T>()
            where T : Node
        {
            return ListenerSwitchesTarget(typeof(T), ListenerState.Node);
        }
        /// <summary>
        /// 动态监听器切换系统目标
        /// </summary>
        public bool ListenerSwitchesSystem<T>()
            where T : IRule
        {
            return ListenerSwitchesTarget(typeof(T), ListenerState.Rule);
        }

        /// <summary>
        /// 动态监听器清除目标
        /// </summary>
        public void ListenerClearTarget()
        {
            DynamicListenerRemove();
            listenerTarget = null;
            listenerState = ListenerState.Not;
        }

        #region 私有

        #region 添加

        /// <summary>
        /// 监听器根据标记添加目标
        /// </summary>
        private void DynamicListenerAdd()
        {
            if (listenerTarget != null)
            {
                if (listenerState == ListenerState.Node)
                {
                    if (listenerTarget == typeof(Node))
                    {
                        DynamicListenerAddAll();
                    }
                    else
                    {
                        DynamicListenerAddTarget(listenerTarget);
                    }
                }
                else if (listenerState == ListenerState.Rule)
                {
                    DynamicListenerAddSystem(listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器添加 所有节点
        /// </summary>
        private void DynamicListenerAddAll()
        {
            //获取 Node 动态目标 系统组集合
            if (Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var systemGroups))
            {
                //遍历现有广播
                foreach (var BroadcastGroup in Root.DynamicListenerBroadcastManager.ListenerBroadcastGroupDictionary)
                {
                    //遍历获取动态系统组，并添加自己
                    foreach (var systemGroup in systemGroups)
                    {
                        if (BroadcastGroup.Value.TryGetBroadcast(systemGroup.Key, out var broadcast))
                        {
                            broadcast.Enqueue(this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监听器添加 系统目标
        /// </summary>
        private void DynamicListenerAddSystem(Type type)
        {
            //获取系统组
            if (Root.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历系统组
                foreach (var targetSystems in targetSystemGroup)
                {
                    DynamicListenerAddTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器添加 节点目标
        /// </summary>
        private void DynamicListenerAddTarget(Type type)
        {
            if (Root.DynamicListenerBroadcastManager.TryGetGroup(type, out var broadcastGroup))
            {
                //获取 Node 动态目标 系统组集合
                if (Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var systemGroups))
                {
                    //遍历获取动态系统组，并添加自己
                    foreach (var systemGroup in systemGroups)
                    {
                        broadcastGroup.GetBroadcast(systemGroup.Key).Enqueue(this);
                    }
                }
            }
        }



        #endregion

        #region 移除

        /// <summary>
        /// 监听器根据标记移除目标
        /// </summary>
        private void DynamicListenerRemove()
        {
            if (listenerTarget != null)
            {
                if (listenerState == ListenerState.Node)
                {
                    if (listenerTarget == typeof(Node))
                    {
                        DynamicListenerRemoveAll();
                    }
                    else
                    {
                        DynamicListenerRemoveTarget(listenerTarget);
                    }
                }
                else if (listenerState == ListenerState.Rule)
                {
                    DynamicListenerRemoveSystem(listenerTarget);
                }
            }
        }

        /// <summary>
        /// 监听器移除 所有节点
        /// </summary>
        private void DynamicListenerRemoveAll()
        {
            //获取 Node 动态目标 系统组集合
            if (Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var systemGroups))
            {
                //遍历现有全部池
                foreach (var BroadcastGroup in Root.DynamicListenerBroadcastManager.ListenerBroadcastGroupDictionary)
                {
                    //遍历获取动态系统组，并移除自己
                    foreach (var systemGroup in systemGroups)
                    {
                        if (BroadcastGroup.Value.TryGetBroadcast(systemGroup.Key, out var broadcast))
                        {
                            broadcast.Remove(this);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 监听器移除 系统目标
        /// </summary>
        private void DynamicListenerRemoveSystem(Type type)
        {
            //获取系统组
            if (Root.RuleManager.TryGetRuleGroup(type, out var targetSystemGroup))
            {
                //遍历系统组
                foreach (var targetSystems in targetSystemGroup)
                {
                    DynamicListenerRemoveTarget(targetSystems.Key);
                }
            }
        }

        /// <summary>
        /// 监听器移除 节点目标
        /// </summary>
        private void DynamicListenerRemoveTarget(Type type)
        {
            if (Root.DynamicListenerBroadcastManager.TryGetGroup(type, out var broadcastGroup))
            {
                //获取 Node 动态目标 系统组集合
                if (Root.RuleManager.TargetRuleDictionary.TryGetValue(typeof(Node), out var systemGroups))
                {
                    //遍历获取动态系统组，并移除自己
                    foreach (var systemGroup in systemGroups)
                    {
                        broadcastGroup.GetBroadcast(systemGroup.Key).Remove(this);
                    }
                }
            }
        }
        #endregion
        #endregion

    }
}
