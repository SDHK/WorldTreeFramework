using System;

namespace WorldTree
{
    /// <summary>
    /// 监听系统广播组
    /// </summary>
    public class ListenerSystemBroadcastGroup : Entity
    {
        public UnitDictionary<Type, SystemBroadcast> broadcasts;
        public UnitDictionary<Type, SystemBroadcast> dynamicBroadcasts;

        public SystemBroadcast GetBroadcast<T>() => GetBroadcast(typeof(T));

        public SystemBroadcast GetBroadcast(Type type)
        {
            if (TryGetParent<EntityPool>(out var pool))
            {
                if (!broadcasts.TryGetValue(type, out var broadcast))
                {
                    broadcast = this.AddChildren<SystemBroadcast>().Load(type, pool.ObjectType);
                    broadcasts.Add(type, broadcast);
                }
                return broadcast;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取动态广播器
        /// </summary>
        public SystemBroadcast GetDynamicBroadcast<T>() => GetDynamicBroadcast(typeof(T));

        /// <summary>
        /// 获取动态广播器
        /// </summary>
        public SystemBroadcast GetDynamicBroadcast(Type type)
        {
            if (TryGetParent<EntityPool>(out var pool))
            {
                if (!dynamicBroadcasts.TryGetValue(type, out var broadcast))
                {
                    if (Root.SystemManager.TryGetTargetSystemGroup(type, typeof(Entity), out broadcast.systems))
                    {
                        broadcast = this.AddChildren<SystemBroadcast>();
                        dynamicBroadcasts.Add(type, broadcast);
                    }
                }
                return broadcast;
            }
            else
            {
                return null;
            }
        }
    }

    class ListenerSystemBroadcastGroupAddSystem : AddSystem<ListenerSystemBroadcastGroup>
    {
        public override void OnAdd(ListenerSystemBroadcastGroup self)
        {
            self.PoolGet(out self.broadcasts);
            self.PoolGet(out self.dynamicBroadcasts);
        }
    }

    class ListenerSystemBroadcastGroupRemoveSystem : RemoveSystem<ListenerSystemBroadcastGroup>
    {
        public override void OnRemove(ListenerSystemBroadcastGroup self)
        {
            self.broadcasts.Dispose();
            self.dynamicBroadcasts.Dispose();
            self.broadcasts = null;
            self.dynamicBroadcasts = null;
        }
    }
}
