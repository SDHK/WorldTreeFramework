
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

        //有监听器系统的实体
        public UnitDictionary<long, Entity> EntityAddListeners = new UnitDictionary<long, Entity>();
        public UnitDictionary<long, Entity> EntityRemoveListeners = new UnitDictionary<long, Entity>();

        private SystemGroup entityAddSystems;
        private SystemGroup entityRemoveSystems;
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
            Components = new UnitDictionary<Type, Entity>();
            Children = new UnitDictionary<long, Entity>();

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
            entityAddSystems = Root.SystemManager.GetGroup<IEntityAddSystem>();
            entityRemoveSystems = Root.SystemManager.GetGroup<IEntityRemoveSystem>();
            addSystems = Root.SystemManager.GetGroup<IAddSystem>();
            removeSystems = Root.SystemManager.GetGroup<IRemoveSystem>();
            enableSystems = Root.SystemManager.GetGroup<IEnableSystem>();
            disableSystems = Root.SystemManager.GetGroup<IDisableSystem>();
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

            //广播给全部监听器。 改进：思考是否直接将监听器绑定到对象池
            foreach (var manager in EntityAddListeners)
            {
                entityAddSystems?.Send(manager.Value, entity);
            }
            allEntity.TryAdd(entity.id, entity);
            //这个实体的添加事件

            addSystems?.Send(entity);

            //!==

            List<ISystem> systems;


            //检测到监听系统存在，则说明这是个监听器
            if (entityAddSystems.TryGetValue(typeKey, out systems))
            {
                RegisterEntityAddSystems(entity, systems);
            }
            if (entityRemoveSystems.TryGetValue(typeKey, out systems))
            {
                RegisterEntityRemoveSystems(entity, systems);
            }

            //!==

            entity.SetActive(true);
            enableSystems?.Send(entity);//添加后调用激活事件

        }

        public void Remove(Entity entity)
        {
            Type typeKey = entity.Type;
            entity.SetActive(false);//激活标记变更
            entity.RemoveAll();//移除所有子节点和组件
            disableSystems?.Send(entity);//调用禁用事件

            //检测到系统存在，则说明这是个监听器
            if (entityAddSystems.ContainsKey(typeKey))
            {
                EntityAddListeners.Remove(entity.id);
            }
            if (entityRemoveSystems.ContainsKey(typeKey))
            {
                EntityRemoveListeners.Remove(entity.id);
            }
            //这个实体的移除事件
            removeSystems?.Send(entity);

            allEntity.Remove(entity.id);

            foreach (var manager in EntityRemoveListeners)//广播给全部监听器
            {
                entityRemoveSystems?.Send(manager.Value, entity);
            }
        }

        /// <summary>
        /// 注册实体添加系统
        /// </summary>
        private void RegisterEntityAddSystems(Entity entity, List<ISystem> systems)
        {
            EntityAddListeners.TryAdd(entity.id, entity);

            foreach (IEntityAddSystem system in systems)//遍历所有监听系统
            {
                //不指定系统
                if (system.ListenerSystemType == typeof(ISystem))
                {
                    //不指定系统，不指定实体
                    if (system.ListenerEntityType == typeof(Entity))
                    {
                        foreach (var pool in EntityPoolManager.pools)//注册到全部实体对象池
                        {
                            pool.Value.AddListeners.TryAdd(entity.id, entity);
                        }
                    }
                    //不指定系统，指定实体
                    else if (EntityPoolManager.pools.TryGetValue(system.ListenerEntityType, out EntityPool pool))
                    {
                        pool.AddListeners.TryAdd(entity.id, entity);//注册到对象池
                    }
                }
                //指定了系统
                else if (SystemManager.TryGetGroup(system.ListenerSystemType, out SystemGroup systemGroup))
                {
                    //指定系统，不指定实体
                    if (system.ListenerEntityType == typeof(Entity))//判断系统是否监听全局实体
                    {
                        foreach (var systemItem in systemGroup)
                        {
                            if (EntityPoolManager.pools.TryGetValue(systemItem.Key, out EntityPool pool))
                            {
                                pool.AddListeners.TryAdd(entity.id, entity);//注册到指定对象池
                            }
                        }
                    }
                    //指定系统，指定实体
                    else if (EntityPoolManager.pools.TryGetValue(system.ListenerEntityType, out EntityPool pool))
                    {
                        if (systemGroup.ContainsKey(system.ListenerEntityType))
                        {
                            pool.AddListeners.TryAdd(entity.id, entity);//注册到指定对象池
                        }
                    }
                }
            }
        }

        private void RegisterEntityRemoveSystems(Entity entity, List<ISystem> systems)
        {
            EntityRemoveListeners.TryAdd(entity.id, entity);

            foreach (IEntityRemoveSystem system in systems)//遍历所有监听系统
            {
                //不指定系统
                if (system.ListenerSystemType == typeof(ISystem))
                {
                    //不指定系统，不指定实体
                    if (system.ListenerEntityType == typeof(Entity))
                    {
                        foreach (var pool in EntityPoolManager.pools)//注册到全部实体对象池
                        {
                            pool.Value.RemoveListeners.TryAdd(entity.id, entity);
                        }
                    }
                    //不指定系统，指定实体
                    else if (EntityPoolManager.pools.TryGetValue(system.ListenerEntityType, out EntityPool pool))
                    {
                        pool.RemoveListeners.TryAdd(entity.id, entity);//注册到对象池
                    }
                }
                //指定了系统
                else if (SystemManager.TryGetGroup(system.ListenerSystemType, out SystemGroup systemGroup))
                {
                    //指定系统，不指定实体
                    if (system.ListenerEntityType == typeof(Entity))//判断系统是否监听全局实体
                    {
                        foreach (var systemItem in systemGroup)
                        {
                            if (EntityPoolManager.pools.TryGetValue(systemItem.Key, out EntityPool pool))
                            {
                                pool.RemoveListeners.TryAdd(entity.id, entity);//注册到指定对象池
                            }
                        }
                    }
                    //指定系统，指定实体
                    else if (EntityPoolManager.pools.TryGetValue(system.ListenerEntityType, out EntityPool pool))
                    {
                        if (systemGroup.ContainsKey(system.ListenerEntityType))
                        {
                            pool.RemoveListeners.TryAdd(entity.id, entity);//注册到指定对象池
                        }
                    }
                }
            }
        }





    }
}
