
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 实体管理器，根节点组件
* 
* 是ECS框架的启动入口
* 
* 同时启动系统管理器和对象池管理器
* 
* 还有单例启动
* 
* 管理分发全局的实体与组件的生命周期
* 

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    //剩余
    //异常处理？
    //思考监听器注入到对象池

    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : Entity
    {
        public UnitDictionary<long, Entity> allEntity = new UnitDictionary<long, Entity>();

        /// <summary>
        /// 添加监听器
        /// </summary>
        public UnitDictionary<Type, UnitDictionary<long, Entity>> EntityAddListeners = new UnitDictionary<Type, UnitDictionary<long, Entity>>();
        /// <summary>
        /// 移除监听器
        /// </summary>
        public UnitDictionary<Type, UnitDictionary<long, Entity>> EntityRemoveListeners = new UnitDictionary<Type, UnitDictionary<long, Entity>>();

        private SystemGroup entityAddSystems;
        private SystemGroup entityRemoveSystems;
        private HashSet<Type> AddListenerTypes;
        private HashSet<Type> RemoveListenerTypes;

        //private SystemGroup singletonEagerSystems;


        private SystemGroup addSystems;
        private SystemGroup removeSystems;
        private SystemGroup enableSystems;
        private SystemGroup disableSystems;


        public IdManager IdManager;
        public SystemManager SystemManager;
        public UnitPoolManager UnitPoolManager;
        public EntityPoolManager EntityPoolManager;


        public EntityManager() : base()
        {
            //此时没有对象池，直接新建容器
            Children = new UnitDictionary<long, Entity>();
            Components = new UnitDictionary<Type, Entity>();

            //框架运转的核心组件
            IdManager = new IdManager();
            SystemManager = new SystemManager();
            UnitPoolManager = new UnitPoolManager();
            EntityPoolManager = new EntityPoolManager();

            //赋予根节点
            Root = this;
            IdManager.Root = this;
            SystemManager.Root = this;
            UnitPoolManager.Root = this;
            EntityPoolManager.Root = this;

            //赋予id
            Root.id = IdManager.GetId();
            IdManager.id = IdManager.GetId();
            SystemManager.id = IdManager.GetId();
            UnitPoolManager.id = IdManager.GetId();
            EntityPoolManager.id = IdManager.GetId();

            //实体管理器系统事件获取
            addSystems = Root.SystemManager.GetGroup<IAddSystem>();
            removeSystems = Root.SystemManager.GetGroup<IRemoveSystem>();
            enableSystems = Root.SystemManager.GetGroup<IEnableSystem>();
            disableSystems = Root.SystemManager.GetGroup<IDisableSystem>();
            entityAddSystems = Root.SystemManager.GetListenerGroup<IEntityAddSystem>();
            entityRemoveSystems = Root.SystemManager.GetListenerGroup<IEntityRemoveSystem>();

            Root.SystemManager.ListenerTypes.TryGetValue(typeof(IEntityAddSystem), out AddListenerTypes);
            Root.SystemManager.ListenerTypes.TryGetValue(typeof(IEntityRemoveSystem), out RemoveListenerTypes);
            //singletonEagerSystems = SystemManager.GetGroup<ISingletonEagerSystem>();


            //激活自己
            SetActive(true);

            //核心组件添加
            AddComponent(IdManager);
            AddComponent(SystemManager);
            AddComponent(UnitPoolManager);
            AddComponent(EntityPoolManager);

            //饿汉单例启动
            //singletonEagerSystems?.Send(this);


        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            RemoveAll();
            EntityAddListeners.Clear();
            EntityRemoveListeners.Clear();
        }



        public void Add(Entity entity)
        {
            Type typeKey = entity.Type;

            //广播给全部监听器。
            if (entityAddSystems != null)
            {
                if (entityAddSystems.TryGetValue(entity.Type, out List<ISystem> Listeners))//指定实体，不指定系统
                {
                    foreach (IEntityAddSystem listenerSystem in Listeners)
                    {
                        if (EntityAddListeners.TryGetValue(listenerSystem.ListenerType, out UnitDictionary<long, Entity> listeners))
                        {
                            foreach (var listener in listeners)
                            {
                                listenerSystem.Invoke(listener.Value, entity);
                            }
                        }
                    }
                }
                if (entityAddSystems.TryGetValue(typeof(Entity), out Listeners))
                {
                    foreach (IEntityAddSystem listenerSystem in Listeners)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.ListenerSystemType == typeof(ISystem) ? true : SystemManager.TryGetSystems(entity.Type, listenerSystem.ListenerSystemType, out _))
                            if (EntityAddListeners.TryGetValue(listenerSystem.ListenerType, out UnitDictionary<long, Entity> listeners))
                            {
                                foreach (var listener in listeners)
                                {
                                    listenerSystem.Invoke(listener.Value, entity);
                                }
                            }
                    }
                }
            }

            allEntity.TryAdd(entity.id, entity);
            //这个实体的添加事件

            addSystems?.Send(entity);

            //检测到监听系统存在，则说明这是个监听器
            if (AddListenerTypes != null)
                if (AddListenerTypes.Contains(typeKey))
                {
                    EntityAddListeners.GetOrNewValue(typeKey).TryAdd(entity.id, entity);
                }
            if (RemoveListenerTypes != null)
                if (RemoveListenerTypes.Contains(typeKey))
                {
                    EntityRemoveListeners.GetOrNewValue(typeKey).TryAdd(entity.id, entity);
                }


            entity.SetActive(true);
            enableSystems?.Send(entity);//添加后调用激活事件

        }




        public void Remove(Entity entity)
        {
            Type typeKey = entity.Type;
            entity.SetActive(false);//激活标记变更
            entity.RemoveAll();//移除所有子节点和组件
            disableSystems?.Send(entity);//调用禁用事件

            //检测到监听系统存在，则说明这是个监听器

            if (AddListenerTypes != null)
                if (AddListenerTypes.Contains(typeKey))
                {
                    EntityAddListeners.GetOrNewValue(typeKey).Remove(entity.id);
                }
            if (RemoveListenerTypes != null)
                if (RemoveListenerTypes.Contains(typeKey))
                {
                    EntityRemoveListeners.GetOrNewValue(typeKey).Remove(entity.id);
                }

            //这个实体的移除事件
            removeSystems?.Send(entity);

            allEntity.Remove(entity.id);

            //广播给全部监听器
            if (entityRemoveSystems != null)
            {
                if (entityRemoveSystems.TryGetValue(entity.Type, out List<ISystem> Listeners))
                {
                    foreach (IEntityRemoveSystem listenerSystem in Listeners)
                    {
                        if (EntityRemoveListeners.TryGetValue(listenerSystem.ListenerType, out UnitDictionary<long, Entity> listeners))
                        {
                            foreach (var listener in listeners)
                            {
                                listenerSystem.Invoke(listener.Value, entity);
                            }
                        }
                    }
                }
                if (entityRemoveSystems.TryGetValue(typeof(Entity), out Listeners))
                {
                    foreach (IEntityRemoveSystem listenerSystem in Listeners)
                    {
                        //不指定实体，不指定系统      ,不指定实体，指定系统
                        if (listenerSystem.ListenerSystemType == typeof(ISystem) ? true : SystemManager.TryGetSystems(entity.Type, listenerSystem.ListenerSystemType, out _))
                            if (EntityRemoveListeners.TryGetValue(listenerSystem.ListenerType, out UnitDictionary<long, Entity> listeners))
                            {
                                foreach (var listener in listeners)
                                {
                                    listenerSystem.Invoke(listener.Value, entity);
                                }
                            }
                    }
                }
            }

        }
    }
}
