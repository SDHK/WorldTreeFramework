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

    public class EntityListenerSystems : UnitPoolItem
    {
        public Entity listenerEntity;

        public UnitDictionary<Type, IListenerSystem> listenerSystems;
    }

    public class EntityPoolSystemGlobalBroadcast : Entity
    {
        public EntityPool pool;

        public Dictionary<long, EntityListenerSystems> addListenerSystems;

        public Dictionary<long, EntityListenerSystems> removeListenerSystems;

    }


    public class EntityPoolSystemGlobalBroadcastAddSystem : AddSystem<EntityPoolSystemGlobalBroadcast>
    {
        //注册监听器到池
        private void RegisterListener(EntityPoolSystemGlobalBroadcast self, Dictionary<long, EntityListenerSystems> registerListenerSystems, IEntityAddSystem listenerSystem)
        {
            if (self.Root.EntityAddListeners.TryGetValue(listenerSystem.ListenerEntityType, out UnitDictionary<long, Entity> listeners))
            {
                foreach (var listener in listeners)
                {
                    if (!registerListenerSystems.TryGetValue(listener.Key, out EntityListenerSystems entityListener))
                    {
                        entityListener = self.PoolGet<EntityListenerSystems>();
                        entityListener.listenerEntity = listener.Value;
                        entityListener.listenerSystems = self.PoolGet<UnitDictionary<Type, IListenerSystem>>();
                        entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                        registerListenerSystems.Add(listener.Key, entityListener);
                    }
                    else //监听器已经存在
                    {
                        if (!entityListener.listenerSystems.ContainsKey(listenerSystem.SystemType))
                        {
                            entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                        }
                    }
                }
            }
        }

        public override void OnAdd(EntityPoolSystemGlobalBroadcast self)
        {
            if (self.TryGetParent(out self.pool))
            {
                //实体添加系统
                if (self.Root.entityAddSystems.TryGetValue(self.pool.ObjectType, out List<ISystem> addEntitySystems))//指定实体，不指定系统
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)//遍历 实体添加监听系统 列表
                    {
                        RegisterListener(self, self.addListenerSystems, listenerSystem);
                    }
                }
                if (self.Root.entityAddSystems.TryGetValue(typeof(Entity), out addEntitySystems))
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.ListenerSystemType == typeof(ISystem) ? true : self.Root.SystemManager.TryGetSystems(self.pool.ObjectType, listenerSystem.ListenerSystemType, out _))
                            RegisterListener(self, self.addListenerSystems, listenerSystem);
                    }
                }

                //实体移除系统
                if (self.Root.entityRemoveSystems.TryGetValue(self.pool.ObjectType, out List<ISystem> removeEntityListenerSystems))//指定实体，不指定系统
                {
                    foreach (IEntityAddSystem listenerSystem in removeEntityListenerSystems)//遍历 实体添加监听系统 列表
                    {
                        RegisterListener(self, self.removeListenerSystems, listenerSystem);
                    }
                }
                if (self.Root.entityRemoveSystems.TryGetValue(typeof(Entity), out removeEntityListenerSystems))
                {
                    foreach (IEntityAddSystem listenerSystem in removeEntityListenerSystems)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.ListenerSystemType == typeof(ISystem) ? true : self.Root.SystemManager.TryGetSystems(self.pool.ObjectType, listenerSystem.ListenerSystemType, out _))
                            RegisterListener(self, self.removeListenerSystems, listenerSystem);
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

                //实体添加系统
                if (self.Root.entityAddSystems.TryGetValue(self.pool.ObjectType, out List<ISystem> addEntitySystems))//指定实体，不指定系统
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)//遍历 实体添加监听系统 列表
                    {
                        //RegisterListener(self, self.addListenerSystems, listenerSystem);
                    }
                }
                if (self.Root.entityAddSystems.TryGetValue(typeof(Entity), out addEntitySystems))
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.ListenerSystemType == typeof(ISystem) ? true : self.Root.SystemManager.TryGetSystems(self.pool.ObjectType, listenerSystem.ListenerSystemType, out _))
                            //RegisterListener(self, self.addListenerSystems, listenerSystem);
                    }
                }

            }


        }
    }






}
