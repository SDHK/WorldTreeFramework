
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
using static UnityEngine.EventSystems.EventTrigger;

namespace WorldTree
{
    //剩余
    //异常处理？
    //Address异步获取对象有问题，GameObjectPoolManager键值不能用预制体

    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : Entity
    {
        public UnitDictionary<long, Entity> allEntity = new UnitDictionary<long, Entity>();


        /// <summary>
        /// 监听器类型，监听器实体集合
        /// </summary>
        public UnitDictionary<Type, UnitDictionary<long, Entity>> DynamicListenerPool = new UnitDictionary<Type, UnitDictionary<long, Entity>>();

        /// <summary>
        /// 实体添加监听系统组
        /// </summary>
        public SystemGroup entityAddSystems;
        /// <summary>
        /// 实体移除监听系统组
        /// </summary>
        public SystemGroup entityRemoveSystems;


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

            //Root.SystemManager.TryGetListenerGroup<IListenerAddSystem>(out entityAddSystems);
            //Root.SystemManager.TryGetListenerGroup<IListenerRemoveSystem>(out entityRemoveSystems);

            //Root.SystemManager.ListenerTypes.TryGetValue(typeof(IListenerAddSystem), out AddListenerTypes);
            //Root.SystemManager.ListenerTypes.TryGetValue(typeof(IListenerRemoveSystem), out RemoveListenerTypes);
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
            //EntityListeners.Clear();
        }



        public void Add(Entity entity)
        {
            //广播给全部监听器!!!!
            //entity.GetStaticListenerSystemGlobalBroadcast<IListenerAddSystem>()?.Send();
            entity.GetDynamicListenerSystemGlobalBroadcast<IListenerAddSystem>()?.Send();

            allEntity.TryAdd(entity.id, entity);

            //这个实体的添加事件
            addSystems?.Send(entity);


            //====

            //检测添加静态监听
            //StaticListenerAdd(entity);

            //检测添加动态监听
            if (entity.ListenerSwitchesTarget(typeof(Entity), ListenerState.Entity))
            {
                if (!DynamicListenerPool.TryGetValue(entity.listenerTarget, out var listeners))
                {
                    listeners = new UnitDictionary<long, Entity>();
                }
                listeners.TryAdd(entity.id, entity);
            }



            entity.SetActive(true);
            enableSystems?.Send(entity);//添加后调用激活事件

        }




        public void Remove(Entity entity)
        {
            entity.SetActive(false);//激活标记变更
            entity.RemoveAll();//移除所有子节点和组件
            disableSystems?.Send(entity);//调用禁用事件

            ////检测移除静态监听
            //StaticListenerRemove(entity);

            ////检测移除动态监听
            //if (SystemManager.DynamicListenerTypes.Contains(Type))
            //{
            //    if (DynamicListenerPool.TryGetValue(entity.listenerTarget, out var listeners))
            //    {
            //        listeners.Remove(entity.id);
            //    }
            //}

            //这个实体的移除事件
            removeSystems?.Send(entity);

            allEntity.Remove(entity.id);

            //广播给全部监听器!!!!
            //entity.GetStaticListenerSystemGlobalBroadcast<IListenerRemoveSystem>()?.Send();
            //entity.GetDynamicListenerSystemGlobalBroadcast<IListenerRemoveSystem>()?.Send();
        }


        //静态填装


        /// <summary>
        /// 检测添加静态监听器
        /// </summary>
        private void StaticListenerAdd(Entity listener)
        {
            //判断是否为监听器
            if (Root.SystemManager.ListenerSystems.TryGetValue(Type, out var systemGroups))
            {
                foreach (var systemGroup in systemGroups)//遍历系统组集合获取系统类型
                {
                    foreach (var systems in systemGroup.Value)//遍历系统组获取目标类型
                    {
                        if (EntityPoolManager.TryGetPool(systems.Key, out EntityPool pool))//尝试获取目标对象池
                        {
                            pool.AddComponent<ListenerSystemBroadcastGroup>().GetBroadcast(systemGroup.Key).AddEntity(listener);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测移除静态监听器
        /// </summary>
        private void StaticListenerRemove(Entity listener)
        {
            //判断是否为监听器
            if (Root.SystemManager.ListenerSystems.TryGetValue(Type, out var systemGroups))
            {
                foreach (var systemGroup in systemGroups)//遍历系统组集合获取系统类型
                {
                    foreach (var systems in systemGroup.Value)//遍历系统组获取目标类型
                    {
                        if (EntityPoolManager.TryGetPool(systems.Key, out EntityPool pool))//尝试获取目标对象池
                        {
                            pool.AddComponent<ListenerSystemBroadcastGroup>().GetBroadcast(systemGroup.Key).RemoveEntity(listener);
                        }
                    }
                }
            }
        }

    }
}
