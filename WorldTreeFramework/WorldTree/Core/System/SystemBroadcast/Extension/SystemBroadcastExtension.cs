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
        /// 广播填装实体
        /// </summary>
        public static SystemBroadcast Load<T>(this SystemBroadcast broadcast) where T : ISystem => Load(broadcast, typeof(T));
        /// <summary>
        /// 广播填装实体
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

        /// <summary>
        /// 广播填装监听器
        /// </summary>
        public static SystemBroadcast Load<T>(this SystemBroadcast broadcast, Type targetType) where T : ISystem => Load(broadcast, typeof(T), targetType);

        /// <summary>
        /// 广播填装监听器
        /// </summary>
        public static SystemBroadcast Load(this SystemBroadcast broadcast, Type systemType, Type targetType)
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
        /// 获取对应系统的全局广播
        /// </summary>
        public static SystemBroadcast GetSystemGlobalBroadcast<T>(this Entity self)
        where T : ISystem
        {
            var systemBroadcast = self.Root.AddComponent<SystemBroadcastGroup>().GetBroadcast<T>();
            systemBroadcast.AddComponent<SystemBroadcastGlobalAddListener>();
            systemBroadcast.AddComponent<SystemBroadcastGlobalRemoveListener>();
            return systemBroadcast;
        }

        /// <summary>
        /// 获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static SystemBroadcast GetListenerSystemGlobalBroadcast<T>(this Entity self, Type targetType)
        where T : ISystem
        {
            return self.Root.EntityPoolManager.GetPool(targetType).AddComponent<ListenerSystemBroadcastGroup>().GetBroadcast<T>();
        }

        /// <summary>
        /// 尝试获取以实体类型为目标的 监听系统广播
        /// </summary>
        public static bool TryGetListenerSystemGlobalBroadcast<T>(this Entity self, Type targetType, out SystemBroadcast broadcast)
        where T : ISystem
        {
            if (self.Root.EntityPoolManager.TryGetPool(targetType, out var pool))
            {
                broadcast = pool.AddComponent<ListenerSystemBroadcastGroup>().GetBroadcast<T>();
                return true;
            }
            else
            {
                broadcast = null;
                return false;
            }
        }
    }

}
