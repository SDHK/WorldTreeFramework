/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池
*
* 管理类型： Node
*   
*   调用 ECS 生命周期系统， 生成， 获取， 回收， 销毁，
*   
*   同时对 实体 赋予 根节点 和 currentId 分发。
*   
*/
using System;
using System.Collections.Generic;

namespace WorldTree
{

    class EntityPoolAddSystem : AddSystem<EntityPool>
    {
        public override void OnEvent(EntityPool self)
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
    public class EntityPool : GenericPool<Node>
    {
        public List<IRule> newSystem;
        public List<IRule> getSystem;
        public List<IRule> recycleSystem;
        public List<IRule> destroySystem;

        /// <summary>
        /// 引用池
        /// </summary>
        public Dictionary<long, Node> Entitys;

        public EntityPool(Type type) : base()
        {

            ObjectType = type;

            Entitys = new Dictionary<long, Node>();

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

        private Node ObjectNew(IPool pool)
        {
            Node obj = Activator.CreateInstance(ObjectType, true) as Node;
            obj.thisPool = this;
            obj.id = Root.IdManager.GetId();
            obj.Root = Root;
            return obj;
        }
        public override void Recycle(object obj) => Recycle(obj as Node);
        public void Recycle(Node obj)
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
        private void ObjectDestroy(Node obj)
        {
            Root.IdManager.Recycle(obj.id);
        }

        private void ObjectOnNew(Node obj)
        {
            newSystem?.Send(obj);
        }

        private void ObjectOnGet(Node obj)
        {
            obj.IsRecycle = false;
            Entitys.TryAdd(obj.id, obj);
            getSystem?.Send(obj);
        }

        private void ObjectOnRecycle(Node obj)
        {
            obj.IsRecycle = true;
            recycleSystem?.Send(obj);
            Entitys.Remove(obj.id);
        }

        private void ObjectOnDestroy(Node obj)
        {
            obj.IsDisposed = true;
            destroySystem?.Send(obj);
        }

    }
}
