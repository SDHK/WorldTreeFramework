/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class SystemBroadcastExtension
    {

        /// <summary>
        /// 广播初始化填装实体
        /// </summary>
        public static SystemBroadcast Load<T>(this SystemBroadcast broadcast) where T : ISystem => Load(broadcast, typeof(T));
        /// <summary>
        /// 广播初始化填装实体
        /// </summary>
        public static SystemBroadcast Load(this SystemBroadcast broadcast, Type systemType)
        {
            if (broadcast.Root.SystemManager.TryGetGroup(systemType, out broadcast.systems))
            {
                broadcast.Clear();
                foreach (var item in broadcast.systems)
                {
                    if (broadcast.PoolManager().pools.TryGetValue(item.Key, out EntityPool pool))
                    {
                        foreach (var entity in pool.Entitys)
                        {
                            broadcast.AddEntity(entity.Value);
                        }
                    }
                }
            }
            return broadcast;
        }

        //=====================================

        /// <summary>
        /// 监听器广播初始化填装  (静态)
        /// </summary>
        public static SystemBroadcast LoadStaticListener<T>(this SystemBroadcast broadcast, Type targetType) where T : ISystem => LoadStaticListener(broadcast, typeof(T), targetType);

        /// <summary>
        /// 监听器广播初始化填装  (静态)
        /// </summary>
        public static SystemBroadcast LoadStaticListener(this SystemBroadcast broadcast, Type systemType, Type targetType)
        {
            if (broadcast.Root.SystemManager.TryGetTargetSystemGroup(systemType, targetType, out broadcast.systems))
            {
                broadcast.Clear();
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
            return broadcast;
        }


        /// <summary>
        /// 监听器广播初始化填装  (动态)
        /// </summary>
        public static SystemBroadcast LoadDynamicListener<T>(this SystemBroadcast broadcast, Type targetType) where T : ISystem => LoadDynamicListener(broadcast, typeof(T), targetType);

        /// <summary>
        /// 监听器广播初始化填装  (动态)
        /// </summary>
        public static SystemBroadcast LoadDynamicListener(this SystemBroadcast broadcast, Type systemType, Type targetType)
        {
            //获取动态系统组
            if (broadcast.Root.SystemManager.TryGetTargetSystemGroup(systemType, typeof(Entity), out broadcast.systems))
            {
                broadcast.Clear();
                foreach (var listenerType in broadcast.systems)
                {
                    if (broadcast.Root.DynamicListenerPool.TryGetValue(listenerType.Key, out var listeners))//获取监听器列表
                    {
                        foreach (var listener in listeners)//遍历动态监听器
                        {
                            if (listener.Value.listenerState == ListenerState.Entity) //监听目标是实体
                            {
                                if (listener.Value.listenerTarget == typeof(Entity) || listener.Value.listenerTarget == targetType)
                                {
                                    broadcast.AddEntity(listener.Value);
                                }
                            }
                            else if (listener.Value.listenerState == ListenerState.System)//监听目标是系统
                            {
                                if (broadcast.Root.SystemManager.TryGetSystems(targetType, listener.Value.listenerTarget, out _))
                                {
                                    broadcast.AddEntity(listener.Value);
                                }
                            }
                        }
                    }
                }
            }
            return broadcast;
        }



        //===

        /// <summary>
        /// 获取对应系统的全局广播
        /// </summary>
        public static SystemBroadcast GetSystemGlobalBroadcast<T>(this Entity self)
        where T : ISystem
        {
            var systemBroadcast = self.Root.AddComponent<SystemBroadcastGroup>().GetBroadcast<T>();
            systemBroadcast.AddComponent<SystemBroadcastGlobalAddListener>().ListenerSwitchesTarget(typeof(T), ListenerState.System);
            systemBroadcast.AddComponent<SystemBroadcastGlobalRemoveListener>().ListenerSwitchesTarget(typeof(T), ListenerState.System);
            return systemBroadcast;
        }

        /// <summary>
        /// 获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static SystemBroadcast GetStaticListenerSystemGlobalBroadcast<T>(this Entity self)
        where T : ISystem
        {
            var group = new ListenerSystemBroadcastGroup();
            group.Root = self.Root;
            group.id = self.IdManager().GetId();
            (self.thisPool as EntityPool)?.AddComponent(group);
            return group?.GetBroadcast<T>();
        }

        /// <summary>
        /// 获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static SystemBroadcast GetDynamicListenerSystemGlobalBroadcast<T>(this Entity self)
        where T : ISystem
        {
            var group = new ListenerSystemBroadcastGroup();
            group.Root = self.Root;
            group.id = self.IdManager().GetId();
            (self.thisPool as EntityPool)?.AddComponent(group);
            return group?.GetDynamicBroadcast<T>();
            //return (self.thisPool as EntityPool)?.AddComponent<ListenerSystemBroadcastGroup>()?.GetDynamicBroadcast<T>();
        }
    }

}
