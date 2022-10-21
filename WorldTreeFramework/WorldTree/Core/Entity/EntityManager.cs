
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

namespace WorldTree
{
    //剩余
    //异常处理？


    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : Entity
    {
        public UnitDictionary<long, Entity> allEntity = new UnitDictionary<long, Entity>();

        //有监听器系统的实体
        public UnitDictionary<long, Entity> listeners = new UnitDictionary<long, Entity>();

        private SystemGroup entityAddSystems;
        private SystemGroup entityRemoveSystems;
        //private SystemGroup singletonEagerSystems;


        private SystemGroup addSystems;
        private SystemGroup removeSystems;
        private SystemGroup enableSystems;
        private SystemGroup disableSystems;


        public IdManager IdManager;
        public SystemManager SystemManager;
        public ObjectPoolManager ObjectPoolManager;


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
            AddComponent(ObjectPoolManager);

            //饿汉单例启动
            //singletonEagerSystems?.Send(this);

         
        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            RemoveAll();
            listeners.Clear();
        }


        public void Add(Entity entity)
        {
            Type typeKey = entity.Type;

            //广播给全部监听器
            foreach (var manager in listeners)
            {
                entityAddSystems?.Send(manager.Value, entity);
            }
            allEntity.TryAdd(entity.id, entity);
            //这个实体的添加事件

            addSystems?.Send(entity);

            //检测到系统存在，则说明这是个监听器
            if (entityAddSystems.ContainsKey(typeKey) || entityRemoveSystems.ContainsKey(typeKey))
            {
                listeners.TryAdd(entity.id, entity);
            }
            entity.SetActive(true);
            enableSystems?.Send(entity);//添加后调用激活事件

        }

        public void Remove(Entity entity)
        {
            Type typeKey = entity.Type;
            entity.SetActive(false);
            entity.RemoveAll();
            disableSystems?.Send(entity);//移除前调用禁用事件

            //检测到系统存在，则说明这是个监听器
            if (entityAddSystems.ContainsKey(typeKey) || entityRemoveSystems.ContainsKey(typeKey))
            {
                listeners.Remove(entity.id);
            }

            //这个实体的移除事件
            removeSystems?.Send(entity);
           
            allEntity.Remove(entity.id);

            foreach (var manager in listeners)//广播给全部监听器
            {
                entityRemoveSystems?.Send(manager.Value, entity);
            }
        }
    }
}
