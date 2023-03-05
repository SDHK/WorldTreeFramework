using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 动态监听器广播管理器
    /// </summary>
    public class DynamicListenerBroadcastManager : Node
    {
        /// <summary>
        /// 目标类型 监听器广播字典
        /// </summary>
        /// <remarks>目标类型《系统，广播》</remarks>
        public Dictionary<Type, ListenerBroadcastGroup> ListenerBroadcastGroupDictionary = new Dictionary<Type, ListenerBroadcastGroup>();

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
            ListenerBroadcastGroupDictionary.Clear();
        }

        #region 判断监听器



        #endregion


        #region 获取广播

        /// <summary>
        /// 尝试添加动态广播
        /// </summary>
        public bool TryAddBroadcast(Type Target, Type RuleType, out SystemBroadcast broadcast)
        {
            //判断是否有组
            if (TryGetGroup(Target, out var group))
            {
                //判断是否有广播
                if (group.TryGetBroadcast(RuleType, out broadcast)) { return true; }

                //没有广播 则判断这个系统类型是否有动态类型监听系统组
                else if (Root.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(Node), out var systemGroup))
                {
                    //新建广播
                    broadcast = group.GetBroadcast(RuleType);
                    broadcast.systems = systemGroup;
                    SystemBroadcastAddListener(broadcast, Target);
                    return true;
                }
            }
            else if (Root.RuleManager.TryGetTargetRuleGroup(RuleType, typeof(Node), out var systemGroup))
            {
                //新建组和广播
                broadcast = GetGroup(Target).GetBroadcast(RuleType);
                broadcast.systems = systemGroup;
                SystemBroadcastAddListener(broadcast, Target);
                return true;
            }
            broadcast = null;
            return false;
        }

        /// <summary>
        /// 广播填装监听器
        /// </summary>
        private void SystemBroadcastAddListener(SystemBroadcast broadcast, Type Target)
        {
            foreach (var listenerType in broadcast.systems)//遍历监听类型
            {
                //获取监听器对象池
                if (Root.EntityPoolManager.pools.TryGetValue(listenerType.Key, out EntityPool listenerPool))
                {
                    //遍历已存在的监听器
                    foreach (var listener in listenerPool.Entitys)
                    {
                        //判断目标是否被该监听器监听
                        if (listener.Value.listenerTarget != null)
                        {
                            if (listener.Value.listenerState == ListenerState.Node)
                            {
                                //判断是否全局监听 或 是指定的目标类型
                                if (listener.Value.listenerTarget == typeof(Node) || listener.Value.listenerTarget == Target)
                                {
                                    broadcast.Enqueue(listener.Value);
                                }
                            }
                            else if (listener.Value.listenerState == ListenerState.Rule)
                            {
                                //判断的实体类型是否拥有目标系统
                                if (Root.RuleManager.TryGetRuleList(Target, listener.Value.listenerTarget, out _))
                                {
                                    broadcast.Enqueue(listener.Value);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 尝试获取动态广播
        /// </summary>
        public bool TryGetBroadcast(Type Target, Type RuleType, out SystemBroadcast broadcast)
        {
            if (ListenerBroadcastGroupDictionary.TryGetValue(Target, out var group))
            {
                return group.TryGetBroadcast(RuleType, out broadcast);
            }
            else
            {
                broadcast = null;
                return false;
            }
        }
        #endregion

        #region 获取广播组

        /// <summary>
        /// 获取广播组
        /// </summary>
        public ListenerBroadcastGroup GetGroup(Type Target)
        {
            if (!ListenerBroadcastGroupDictionary.TryGetValue(Target, out var group))
            {
                group = new ListenerBroadcastGroup();
                group.Target = Target;
                group.id = Root.IdManager.GetId();
                group.Root = Root;
                ListenerBroadcastGroupDictionary.Add(Target, group);
                AddChildren(group);
            }
            return group;
        }

        /// <summary>
        /// 尝试获取广播组
        /// </summary>
        public bool TryGetGroup(Type target, out ListenerBroadcastGroup group)
        {
            return ListenerBroadcastGroupDictionary.TryGetValue(target, out group);
        }

        #endregion
    }
}
