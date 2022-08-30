
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
  

    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : Entity
    {
        public UnitDictionary<long, Entity> allEntities = new UnitDictionary<long, Entity>();

        //有监听器的实体
        public UnitDictionary<Type, Entity> listeners = new UnitDictionary<Type, Entity>();

        private SystemGroup entitySystems;
        private SystemGroup singletonEagerSystems;

        private SystemGroup addSystems;
        private SystemGroup removeSystems;
        private SystemGroup enableSystems;
        private SystemGroup disableSystems;


        public IdManager IdManager;
        public SystemManager SystemManager;
        public ObjectPoolManager ObjectPoolManager;
        public EventManager EventManager;




        public EntityManager() : base()
        {
            //此时没有对象池，直接新建容器
            Components = new UnitDictionary<Type, Entity>();
            Children = new UnitDictionary<long, Entity>();

            //框架运转的核心组件
            IdManager = new IdManager();
            SystemManager = new SystemManager();
            ObjectPoolManager = new ObjectPoolManager();

            //赋予根节点
            Root = this;
            IdManager.Root = this;
            SystemManager.Root = this;
            ObjectPoolManager.Root = this;

            //域节点指向自己
            Domain = this;

            //赋予id
            Root.id = IdManager.GetId();
            IdManager.id = IdManager.GetId();
            SystemManager.id = IdManager.GetId();
            ObjectPoolManager.id = IdManager.GetId();

            //实体管理器系统事件获取
            entitySystems = Root.SystemManager.GetSystemGroup<IEntitySystem>();
            addSystems = Root.SystemManager.GetSystemGroup<IAddSystem>();
            removeSystems = Root.SystemManager.GetSystemGroup<IRemoveSystem>();
            enableSystems = Root.SystemManager.GetSystemGroup<IEnableSystem>();
            disableSystems = Root.SystemManager.GetSystemGroup<IDisableSystem>();
            singletonEagerSystems = SystemManager.GetSystemGroup<ISingletonEagerSystem>();

            //核心组件添加
            AddComponent(IdManager);
            AddComponent(SystemManager);
            AddComponent(ObjectPoolManager);
            EventManager = AddComponent<EventManager>();


            //饿汉单例启动
            foreach (UnitList<ISystem> singletonEagers in singletonEagerSystems.Values)
            {
                foreach (ISingletonEagerSystem singletonEager in singletonEagers)
                {
                    singletonEager.Singleton(this);
                }
            }
        }

        public override void OnDispose()
        {
            RemoveAll();
            listeners.Clear();
        }



        public void Add(Entity entity)
        {
            Type typeKey = entity.Type;

            foreach (var manager in listeners)//广播给全部监听器
            {
                if (entitySystems.TryGetValue(manager.Key, out List<ISystem> systems))
                {
                    foreach (IEntitySystem system in systems)
                    {
                        system.AddEntity(manager.Value, entity);
                    }
                }
            }
            allEntities.TryAdd(entity.id, entity);
            //这个实体的添加事件
            if (addSystems.TryGetValue(entity.Type, out List<ISystem> addsystem))
            {
                foreach (IAddSystem system in addsystem)
                {
                    system.Add(entity);
                }
            }

            if (entitySystems.ContainsKey(typeKey))//检测到系统存在，则说明这是个监听器
            {
                listeners.TryAdd(typeKey, entity);
            }
            entity.SetActive(true);
        }

        public void Remove(Entity entity)
        {
            Type typeKey = entity.Type;

            entity.SetActive(false);
            if (entitySystems.ContainsKey(typeKey))//检测到系统存在，则说明这是个监听器
            {
                listeners.Remove(typeKey);
            }

            //这个实体的移除事件
            if (removeSystems.TryGetValue(entity.Type, out List<ISystem> removesystem))
            {
                foreach (IRemoveSystem system in removesystem)
                {
                    system.Remove(entity);
                }
            }
            allEntities.Remove(entity.id);

            foreach (var manager in listeners)//广播给全部监听器
            {
                if (entitySystems.TryGetValue(manager.Key, out List<ISystem> systems))
                {
                    foreach (IEntitySystem system in systems)
                    {
                        system.RemoveEntity(manager.Value, entity);
                    }
                }
            }
        }

        public void Enable(Entity entity)
        {
            if (enableSystems.TryGetValue(entity.Type, out List<ISystem> enableSystem))
            {
                foreach (IEnableSystem system in enableSystem)
                {
                    system.Enable(entity);
                }
            }
        }

        public void Disable(Entity entity)
        {
            if (disableSystems.TryGetValue(entity.Type, out List<ISystem> disableSystem))
            {
                foreach (IDisableSystem system in disableSystem)
                {
                    system.Disable(entity);
                }
            }
        }

    }
}
