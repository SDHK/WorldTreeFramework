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
        public Entity listenerEntity;

        public UnitDictionary<Type, IListenerSystem> listenerSystems;

        public override void OnRecycle()
        {
            listenerSystems.Dispose();
            listenerSystems = null;
        }
    }



    public class EntityPoolSystemGlobalBroadcast : Entity//需要组件化，注册到对象池发送
    {
        public EntityPool pool;

        public UnitQueue<long> addQueue;
        public UnitDictionary<long, EntityListenerSystems> addListenerSystems;

        public UnitQueue<long> removeQueue;
        public UnitDictionary<long, EntityListenerSystems> removeListenerSystems;

        public void BroadcastEntityAdd(Entity entity)
        {
            int length = addQueue.Count;
            for (int i = 0; i < length; i++)
            { 
                long id = addQueue.Dequeue();
                if (addListenerSystems.TryGetValue(id, out EntityListenerSystems entityListenerSystems))
                {
                    foreach (var listenerSystem in entityListenerSystems.listenerSystems)
                    {
                        listenerSystem.Value.Invoke(entityListenerSystems.listenerEntity, entity);
                    }
                    addQueue.Enqueue(id);
                }
            }

        }

        public void BroadcastEntityRemove(Entity entity)
        {


        }

        public void AddAddListener(Entity listener, IListenerSystem listenerSystem)
        {
            if (!addListenerSystems.TryGetValue(listener.id, out EntityListenerSystems entityListener))
            {
                entityListener = this.PoolGet<EntityListenerSystems>();
                entityListener.listenerEntity = listener;
                entityListener.listenerSystems = this.PoolGet<UnitDictionary<Type, IListenerSystem>>();
                entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                addListenerSystems.Add(listener.id, entityListener);
                addQueue.Enqueue(listener.id);
            }
            else //监听器已经注册
            {
                if (!entityListener.listenerSystems.ContainsKey(listenerSystem.SystemType))
                {
                    entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                }
            }
        }

        public void AddRemoveListener(Entity listener, IListenerSystem listenerSystem)
        {
            if (!removeListenerSystems.TryGetValue(listener.id, out EntityListenerSystems entityListener))
            {
                entityListener = this.PoolGet<EntityListenerSystems>();
                entityListener.listenerEntity = listener;
                entityListener.listenerSystems = this.PoolGet<UnitDictionary<Type, IListenerSystem>>();
                entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                removeListenerSystems.Add(listener.id, entityListener);
                removeQueue.Enqueue(listener.id);
            }
            else //监听器已经注册
            {
                if (!entityListener.listenerSystems.ContainsKey(listenerSystem.SystemType))
                {
                    entityListener.listenerSystems.Add(listenerSystem.SystemType, listenerSystem);
                }
            }
        }

        public void RemoveAddListener(long id)
        {
            if (addListenerSystems.ContainsKey(id))
            {
                addListenerSystems.Remove(id);

            }
        }
        public void RemoveRemoveListener(long id)
        {
            removeListenerSystems.Remove(id);
        }


    }
    class EntityPoolSystemGlobalBroadcastAddSystem : AddSystem<EntityPoolSystemGlobalBroadcast>
    {



        public override void OnAdd(EntityPoolSystemGlobalBroadcast self)
        {

            if (self.TryGetParent(out self.pool))
            {
                self.addListenerSystems ??= self.PoolGet<UnitDictionary<long, EntityListenerSystems>>();
                self.removeListenerSystems ??= self.PoolGet<UnitDictionary<long, EntityListenerSystems>>();

                //实体添加系统
                if (self.TryGetListenerTargetSystems<IEntityAddSystem>(self.pool.ObjectType, out List<ISystem> addEntitySystems))
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)//遍历 实体添加监听系统 列表
                    {
                        if (self.Root.EntityAddListeners.TryGetValue(listenerSystem.EntityType, out UnitDictionary<long, Entity> listeners))
                        {
                            foreach (var listener in listeners)
                            {
                                self.AddAddListener(listener.Value, listenerSystem);
                            }
                        }
                    }
                }
                if (self.TryGetListenerTargetSystems<IEntityRemoveSystem>(typeof(Entity), out addEntitySystems))
                {
                    foreach (IEntityAddSystem listenerSystem in addEntitySystems)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.TargetSystemType == typeof(ISystem) ? true : self.TryGetSystems(self.pool.ObjectType, listenerSystem.TargetSystemType, out _))
                            if (self.Root.EntityAddListeners.TryGetValue(listenerSystem.EntityType, out UnitDictionary<long, Entity> listeners))
                            {
                                foreach (var listener in listeners)
                                {
                                    self.AddAddListener(listener.Value, listenerSystem);
                                }
                            }
                    }
                }

                //实体移除系统
                if (self.TryGetListenerTargetSystems<IEntityRemoveSystem>(self.pool.ObjectType, out List<ISystem> removeEntityListenerSystems))//指定实体，不指定系统
                {
                    foreach (IEntityRemoveSystem listenerSystem in removeEntityListenerSystems)//遍历 实体添加监听系统 列表
                    {
                        if (self.Root.EntityRemoveListeners.TryGetValue(listenerSystem.EntityType, out UnitDictionary<long, Entity> listeners))
                        {
                            foreach (var listener in listeners)
                            {
                                self.AddRemoveListener(listener.Value, listenerSystem);
                            }
                        }
                    }
                }
                if (self.TryGetListenerTargetSystems<IEntityRemoveSystem>(typeof(Entity), out removeEntityListenerSystems))
                {
                    foreach (IEntityRemoveSystem listenerSystem in removeEntityListenerSystems)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.TargetSystemType == typeof(ISystem) ? true : self.TryGetSystems(self.pool.ObjectType, listenerSystem.TargetSystemType, out _))
                            if (self.Root.EntityRemoveListeners.TryGetValue(listenerSystem.EntityType, out UnitDictionary<long, Entity> listeners))
                            {
                                foreach (var listener in listeners)
                                {
                                    self.AddRemoveListener(listener.Value, listenerSystem);
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
                foreach (var item in self.addListenerSystems)
                {
                    item.Value.Dispose();
                }

                foreach (var item in self.removeListenerSystems)
                {
                    item.Value.Dispose();
                }

                self.addListenerSystems.Dispose();
                self.removeListenerSystems.Dispose();

                self.addListenerSystems = null;
                self.removeListenerSystems = null;
            }


        }
    }






}
