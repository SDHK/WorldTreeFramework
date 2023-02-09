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
        public static bool TrySendStaticListener<T>(this Entity entity)
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

        /// <summary>
        /// 获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static bool TrySendDynamicListener<T>(this Entity entity)
            where T : IListenerSystem
        {
            if (entity.Root.DynamicListenerBroadcastManager.TryAddBroadcast(entity.Type, typeof(T), out var broadcast))
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
            //判断是否有组
            if (TryGetGroup(Target, out var group))
            {
                //判断是否有广播
                if (group.TryGetBroadcast(System, out broadcast)) { return true; }

                //没有广播 则判断这个目标类型是是否有监听系统组
                else if (Root.SystemManager.TryGetTargetSystemGroup(System, Target, out var systemGroup))
                {
                    //新建广播
                    broadcast = group.GetBroadcast(System);
                    broadcast.systems = systemGroup;
                    SystemBroadcastAddListener(broadcast);
                    return true;
                }
            }
            //没有组则判断这个目标类型是否有监听系统组
            else if (Root.SystemManager.TryGetTargetSystemGroup(System, Target, out var systemGroup))
            {
                //新建组和广播
                broadcast = GetGroup(Target).GetBroadcast(System);
                broadcast.systems = systemGroup;
                SystemBroadcastAddListener(broadcast);
                return true;
            }
            broadcast = null;
            return false;
        }

        /// <summary>
        /// 广播填装监听器
        /// </summary>
        private void SystemBroadcastAddListener(SystemBroadcast broadcast)
        {
            foreach (var listenerType in broadcast.systems)//遍历系统组获取监听器类型
            {
                //从池里拿到已存在的监听器
                if (Root.EntityPoolManager.pools.TryGetValue(listenerType.Key, out EntityPool listenerPool))
                {
                    //全部注入到广播器
                    foreach (var listener in listenerPool.Entitys)
                    {
                        broadcast.AddEntity(listener.Value);
                    }
                }
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
