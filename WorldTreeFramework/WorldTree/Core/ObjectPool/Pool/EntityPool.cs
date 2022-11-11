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

namespace WorldTree
{

    class EntityPoolAddSystem : AddSystem<EntityPool>
    {
        public override void OnAdd(EntityPool self)
        {
            //生命周期系统
            self.newSystem = self.GetSystemGroup<INewSystem>();
            self.getSystem = self.GetSystemGroup<IGetSystem>();
            self.recycleSystem = self.GetSystemGroup<IRecycleSystem>();
            self.destroySystem = self.GetSystemGroup<IDestroySystem>();
        }
    }


    /// <summary>
    /// 实体对象池
    /// </summary>
    public class EntityPool : GenericPool<Entity>
    {
        //特化监听器列表
        //管理器列表

        /// <summary>
        /// 实体添加监听器
        /// </summary>
        public Dictionary<long, Entity> AddListeners = new Dictionary<long, Entity>();
        
        /// <summary>
        /// 实体移除监听器
        /// </summary>
        public Dictionary<long, Entity> RemoveListeners = new Dictionary<long, Entity>();

        //去根节点遍历管理器

        public SystemGroup newSystem;
        public SystemGroup getSystem;
        public SystemGroup recycleSystem;
        public SystemGroup destroySystem;

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
}
