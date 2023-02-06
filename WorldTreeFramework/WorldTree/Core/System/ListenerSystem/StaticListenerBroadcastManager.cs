using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace WorldTree
{
    public static class ListenerBroadcastManagerExtension
    {
        /// <summary>
        /// 获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static bool TrySendListener<T>(this Entity entity)
            where T : IListenerSystem
        {
            if (entity.Root.StaticListenerBroadcastManager.TryAddBroadcast(entity.Type, typeof(T), out var broadcast))
            {
                broadcast.Send(entity);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 静态监听器广播管理器
    /// </summary>
    public class StaticListenerBroadcastManager : Entity
    {
        /// <summary>
        /// 目标，系统，广播
        /// </summary>
        public Dictionary<Type, ListenerBroadcastGroup> BroadcastGroups = new Dictionary<Type, ListenerBroadcastGroup>();

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
            BroadcastGroups.Clear();
        }

        #region 判断监听器

        /// <summary>
        /// 检测添加静态监听器
        /// </summary>
        public void TryAddListener(Entity listener)
        {
            //判断是否为监听器
            if (Root.SystemManager.ListenerSystems.TryGetValue(listener.Type, out var systemGroups))
            {
                foreach (var systemGroup in systemGroups)//遍历系统组集合获取系统类型
                {
                    foreach (var systems in systemGroup.Value)//遍历系统组获取目标类型
                    {
                        if (TryGetBroadcast(systems.Key, systemGroup.Key, out var broadcast))
                        {
                            broadcast.AddEntity(listener);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测移除静态监听器
        /// </summary>
        public void RemoveListener(Entity listener)
        {
            //判断是否为监听器
            if (Root.SystemManager.ListenerSystems.TryGetValue(listener.Type, out var systemGroups))
            {
                foreach (var systemGroup in systemGroups)//遍历系统组集合获取系统类型
                {
                    foreach (var systems in systemGroup.Value)//遍历系统组获取目标类型
                    {
                        if (TryGetBroadcast(systems.Key, systemGroup.Key, out var broadcast))
                        {
                            broadcast.RemoveEntity(listener);
                        }
                    }
                }
            }
        }

        #endregion

        #region 获取广播

        /// <summary>
        /// 尝试添加静态广播
        /// </summary>
        public bool TryAddBroadcast(Type Target, Type System, out SystemBroadcast broadcast)
        {
            //则判断这个目标类型是是否有监听系统组
            if (Root.SystemManager.TryGetTargetSystemGroup(System, Target, out var systemGroup))
            {
                var group = GetGroup(Target);
                if (!group.TryGetBroadcast(System, out broadcast))
                {
                    broadcast = group.GetBroadcast(System);
                    broadcast.systems = systemGroup;

                    foreach (var listenerType in broadcast.systems)
                    {
                        if (broadcast.PoolManager().pools.TryGetValue(listenerType.Key, out EntityPool listenerPool))
                        {
                            foreach (var listener in listenerPool.Entitys)
                            {
                                broadcast.AddEntity(listener.Value);
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                broadcast = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试获取静态广播
        /// </summary>
        public bool TryGetBroadcast(Type Target, Type System, out SystemBroadcast broadcast)
        {
            if (BroadcastGroups.TryGetValue(Target, out var group))
            {
                return group.TryGetBroadcast(System, out broadcast);
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
            if (!BroadcastGroups.TryGetValue(Target, out var group))
            {
                group = new ListenerBroadcastGroup();
                group.Target = Target;
                group.id = Root.IdManager.GetId();
                group.Root = Root;
                BroadcastGroups.Add(Target, group);
                AddChildren(group);
            }
            return group;
        }

        /// <summary>
        /// 尝试获取广播组
        /// </summary>
        public bool TryGetGroup(Type target, out ListenerBroadcastGroup group)
        {
            return BroadcastGroups.TryGetValue(target, out group);
        }

        #endregion

    }
}
