using System;

namespace WorldTree
{

    //public class QueueDictionary<K, V>
    //{
    //    UnitDictionary<K, V> keyValues;
    //    UnitQueue<K> keys;
    //}

    public class EntityListenerSystems : UnitPoolItem
    {
        /// <summary>
        /// 监听器实体
        /// </summary>
        public Entity listenerEntity;

        /// <summary>
        /// 监听器系统
        /// </summary>
        public UnitDictionary<Type, IListenerSystem> listenerSystems;

        public override void OnRecycle()
        {
            listenerSystems.Dispose();
            listenerSystems = null;
        }
    }


    /// <summary>
    /// 监听器系统广播
    /// </summary>
    public class ListenerSystemBroadcast : Entity
    {
        public Type entityType;

        /// <summary>
        /// 监听器注册表  （系统类型（监听器，系统集合））
        /// </summary>
        public UnitDictionary<Type, UnitDictionary<long, EntityListenerSystems>> ListenerSystems;

        /// <summary>
        /// 用于遍历的 ID队列 （系统类型，实体ID）
        /// </summary>
        public UnitDictionary<Type, UnitQueue<long>> IdQueue;

        /// <summary>
        /// 用于删除的  监听器实体系统类型（监听器实体，系统类型）
        /// </summary>
        public UnitDictionary<long, UnitHashSet<Type>> ListenerSystemTypes;

        public ListenerSystemBroadcast(Type type)
        {
            entityType = type;
        }


        /// <summary>
        /// 广播
        /// </summary>
        public void Broadcast<T>(Entity entity)
            where T : IListenerSystem
        {
            if (IdQueue.TryGetValue(typeof(T), out var ids))
            {
                if (ListenerSystems.TryGetValue(typeof(T), out var entityListenerSystems))
                {
                    int length = ids.Count;
                    for (int i = 0; i < length; i++)
                    {
                        long id = ids.Dequeue();

                        if (entityListenerSystems.TryGetValue(id, out var listenerSystems))
                        {
                            ids.Enqueue(id);
                            foreach (var listenerSystem in listenerSystems.listenerSystems)
                            {
                                listenerSystem.Value.Invoke(listenerSystems.listenerEntity, entity);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 添加监听器
        /// </summary>
        public void AddListener(Entity listener)//是否添加any
        {
            if (this.Root.SystemManager.ListenerSystems.TryGetValue(listener.Type, out var systemGroups))//获取监听系统集合
            {
                foreach (var systemGroup in systemGroups)//遍历系统集合
                {
                    if (!this.ListenerSystems.TryGetValue(systemGroup.Key, out var entityListener)) this.PoolGet(out entityListener);
                    if (!this.IdQueue.TryGetValue(systemGroup.Key, out var ids)) this.PoolGet(out ids);

                    if (systemGroup.Value.TryGetValue(this.entityType, out var listenerSystems))
                    {
                        if (!entityListener.TryGetValue(listener.id, out var entityListenerSystems))
                        {
                            ids.Enqueue(listener.id);
                            this.PoolGet(out entityListenerSystems);
                            entityListenerSystems.listenerEntity = listener;
                        }

                        foreach (IListenerSystem listenerSystem in listenerSystems)//遍历监听器系统进行注册
                        {
                            if (!this.ListenerSystemTypes.TryGetValue(listener.id, out var systemTypes)) this.PoolGet(out systemTypes);
                            if (systemTypes.Contains(listenerSystem.SystemType)) { systemTypes.Add(listenerSystem.SystemType); }

                            entityListenerSystems.listenerSystems.TryAdd(listenerSystem.GetType(), listenerSystem);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 移除监听器
        /// </summary>
        public void RemoveListener(long id)
        {
            if (ListenerSystemTypes.TryGetValue(id, out var types))
            {
                foreach (var type in types)
                {
                    if (ListenerSystems.TryGetValue(type, out var Listeners))
                    {
                        foreach (var Listener in Listeners)
                        {
                            Listener.Value.Dispose();
                        }
                        Listeners.Dispose();
                        ListenerSystems.Remove(type);
                    }
                    if (IdQueue.TryGetValue(type, out var ids))
                    {
                        ids.Dequeue();
                        IdQueue.Remove(type);
                    }
                }
                ListenerSystemTypes.Remove(id);
            }
        }

    }
    class EntityPoolSystemGlobalBroadcastAddSystem : AddSystem<ListenerSystemBroadcast>
    {
        public override void OnAdd(ListenerSystemBroadcast self)
        {
            if (self.ListenerSystems is null) self.PoolGet(out self.ListenerSystems);
            if (self.ListenerSystemTypes is null) self.PoolGet(out self.ListenerSystemTypes);
            if (self.IdQueue is null) self.PoolGet(out self.IdQueue);

            if (self.Root.SystemManager.TargetSystems.TryGetValue(self.entityType, out var systemGroups))
            {
                foreach (var systemGroup in systemGroups)//系统类型
                {
                    foreach (var listenerSystems in systemGroup.Value)//监听类型
                    {
                        if (self.Root.EntityListeners.TryGetValue(listenerSystems.Key, out var listeners))//拿到对应类型的监听器实体集合
                        {
                            if (!self.ListenerSystems.TryGetValue(systemGroup.Key, out var entityListener)) self.PoolGet(out entityListener);
                            if (!self.IdQueue.TryGetValue(systemGroup.Key, out var ids)) self.PoolGet(out ids);

                            foreach (var listener in listeners)//遍历监听器实体进行注册
                            {
                                if (!entityListener.TryGetValue(listener.Key, out var entityListenerSystems))
                                {
                                    ids.Enqueue(listener.Key);
                                    self.PoolGet(out entityListenerSystems);
                                    entityListenerSystems.listenerEntity = listener.Value;
                                }

                                foreach (IListenerSystem listenerSystem in listenerSystems.Value)//遍历监听器系统进行注册
                                {
                                    if (!self.ListenerSystemTypes.TryGetValue(listener.Key, out var systemTypes)) self.PoolGet(out systemTypes);
                                    if (systemTypes.Contains(listenerSystem.SystemType)) { systemTypes.Add(listenerSystem.SystemType); }

                                    entityListenerSystems.listenerSystems.TryAdd(listenerSystem.GetType(), listenerSystem);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class EntityPoolSystemGlobalBroadcastRemoveSystem : RemoveSystem<ListenerSystemBroadcast>
    {
        public override void OnRemove(ListenerSystemBroadcast self)
        {
            foreach (var Listeners in self.ListenerSystems)
            {
                foreach (var Listener in Listeners.Value)
                {
                    Listener.Value.Dispose();
                }
                Listeners.Value.Dispose();
            }
            self.ListenerSystems.Dispose();
            self.ListenerSystems = null;


            foreach (var types in self.ListenerSystemTypes)
            {
                types.Value.Dispose();
            }
            self.ListenerSystemTypes.Dispose();
            self.ListenerSystemTypes = null;


            foreach (var ids in self.IdQueue)
            {
                ids.Value.Dispose();
            }
            self.IdQueue.Dispose();
            self.IdQueue = null;
        }
    }
}
