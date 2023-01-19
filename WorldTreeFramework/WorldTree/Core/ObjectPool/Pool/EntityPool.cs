/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池
*
* 管理类型： Entity
*   
*   调用 ECS 生命周期系统， 生成， 获取， 回收， 销毁，
*   
*   同时对 实体 赋予 根节点 和 Id 分发。
*   
*/
using System;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace WorldTree
{

    class EntityPoolAddSystem : AddSystem<EntityPool>
    {
        public override void OnAdd(EntityPool self)
        {
            //生命周期系统
            self.newSystem = self.GetSystems<INewSystem>(self.ObjectType);
            self.getSystem = self.GetSystems<IGetSystem>(self.ObjectType);
            self.recycleSystem = self.GetSystems<IRecycleSystem>(self.ObjectType);
            self.destroySystem = self.GetSystems<IDestroySystem>(self.ObjectType);
        }
    }


    /// <summary>
    /// 实体对象池
    /// </summary>
    public class EntityPool : GenericPool<Entity>
    {
        public List<ISystem> newSystem;
        public List<ISystem> getSystem;
        public List<ISystem> recycleSystem;
        public List<ISystem> destroySystem;

        public EntityPool(Type type) : base()
        {

            ObjectType = type;

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;
        }

        /// <summary>
        /// 获取对象并转为指定类型
        /// </summary>
        public T Get<T>()
            where T : class
        {
            return Get() as T;
        }

        public override string ToString()
        {
            return $"[EntityPool<{ObjectType}>] : {Count} ";
        }

        private Entity ObjectNew(IPool pool)
        {
            Entity obj = Activator.CreateInstance(ObjectType, true) as Entity;
            obj.thisPool = this;
            obj.id = Root.IdManager.GetId();
            obj.Root = Root;
            return obj;
        }
        public override void Recycle(object obj) => Recycle(obj as Entity);
        public void Recycle(Entity obj)
        {
            lock (objetPool)
            {
                if (obj != null)
                {
                    if (maxLimit == -1 || objetPool.Count < maxLimit)
                    {
                        if (obj.IsRecycle) return;

                        objectOnRecycle.Invoke(obj);
                        objetPool.Enqueue(obj);
                    }
                    else
                    {
                        objectOnRecycle.Invoke(obj);
                        objectOnDestroy.Invoke(obj);
                        DestroyObject.Invoke(obj);
                    }
                }
            }
        }
        private void ObjectDestroy(Entity obj)
        {
            obj.Dispose();
        }

        private void ObjectOnNew(Entity obj)
        {
            newSystem?.Send(obj);
        }

        private void ObjectOnGet(Entity obj)
        {
            obj.IsRecycle = false;
            getSystem?.Send(obj);
        }

        private void ObjectOnRecycle(Entity obj)
        {
            obj.IsRecycle = true;
            recycleSystem?.Send(obj);
        }

        private void ObjectOnDestroy(Entity obj)
        {
            obj.IsDisposed = true;
            destroySystem?.Send(obj);
        }

    }


    //=====================

    ////实体字典基类
    //public class EntityDictionary : Entity { public IDictionary value; } //箱基类
    ////实体字典泛型类
    //public class EntityDictionary<K, V> : EntityDictionary  //泛型箱
    //{
    //    public EntityDictionary()
    //    {
    //        Type = typeof(EntityDictionary); //将这个泛型类的 匹配标签 改为基类
    //        value = new Dictionary<K, V>(); //初始化赋值
    //    }
    //    public Dictionary<K, V> Value => value as Dictionary<K, V>; //强转获取

    //}
    ////回收系统匹配到基类
    //class EntityDictionaryRemoveystem : RemoveSystem<EntityDictionary>
    //{
    //    public override void OnRemove(EntityDictionary self)
    //    {
    //        self.value.Clear();//统一清理
    //    }
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
    /// 实体池监听系统全局广播组件
    /// </summary>
    public class EntityPoolSystemGlobalBroadcast : Entity
    {
        public EntityPool pool;

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
        public void AddListener(Entity listener)
        { 
        
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
    class EntityPoolSystemGlobalBroadcastAddSystem : AddSystem<EntityPoolSystemGlobalBroadcast>
    {
        public override void OnAdd(EntityPoolSystemGlobalBroadcast self)
        {
            if (self.TryGetParent(out self.pool))
            {
                if (self.ListenerSystems is null) self.PoolGet(out self.ListenerSystems);
                if (self.ListenerSystemTypes is null) self.PoolGet(out self.ListenerSystemTypes);
                if (self.IdQueue is null) self.PoolGet(out self.IdQueue);

                if (self.Root.SystemManager.TargetSystems.TryGetValue(self.pool.ObjectType, out var systemGroups))
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
    }
    public class EntityPoolSystemGlobalBroadcastRemoveSystem : RemoveSystem<EntityPoolSystemGlobalBroadcast>
    {
        public override void OnRemove(EntityPoolSystemGlobalBroadcast self)
        {
            if (self.TryGetParent(out self.pool))
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






}
