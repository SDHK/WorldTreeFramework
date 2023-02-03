using System;

namespace WorldTree
{
    /// <summary>
    /// 监听系统广播组
    /// </summary>
    public class ListenerSystemBroadcastGroup : Entity
    {
        public UnitDictionary<Type, SystemBroadcast> broadcasts = new UnitDictionary<Type, SystemBroadcast>();
        public UnitDictionary<Type, SystemBroadcast> dynamicBroadcasts = new UnitDictionary<Type, SystemBroadcast>();

        /// <summary>
        /// 获取静态监听广播器
        /// </summary>
        public SystemBroadcast GetBroadcast<T>() => GetBroadcast(typeof(T));

        /// <summary>
        /// 获取静态监听广播器
        /// </summary>
        public SystemBroadcast GetBroadcast(Type type)
        {
            if (TryGetParent<EntityPool>(out var pool))
            {
                if (Root.SystemManager.TryGetTargetSystemGroup(type, pool.ObjectType, out _))
                {
                    if (!broadcasts.TryGetValue(type, out var broadcast))
                    {
                        broadcast = new SystemBroadcast();

                        broadcast.id = Root.IdManager.GetId();
                        broadcast.Root = Root;
                        broadcasts.Add(type, broadcast);

                        AddChildren(broadcast);
                        broadcast.LoadStaticListener(type, pool.ObjectType);

                    }
                    return broadcast;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取动态监听广播器
        /// </summary>
        public SystemBroadcast GetDynamicBroadcast<T>() => GetDynamicBroadcast(typeof(T));

        /// <summary>
        /// 获取动态监听广播器
        /// </summary>
        public SystemBroadcast GetDynamicBroadcast(Type type)
        {
            if (TryGetParent<EntityPool>(out var pool))
            {
                if (Root.SystemManager.TryGetTargetSystemGroup(type, typeof(Entity), out _))
                {
                    if (!dynamicBroadcasts.TryGetValue(type, out var broadcast))
                    {
                        broadcast = new SystemBroadcast();

                        broadcast.id = Root.IdManager.GetId();
                        broadcast.Root = Root;
                        dynamicBroadcasts.Add(type, broadcast);

                        AddChildren(broadcast);
                        broadcast.LoadDynamicListener(type, pool.ObjectType);
                    }
                    return broadcast;
                }
                else
                {
                    return null;
                }
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
