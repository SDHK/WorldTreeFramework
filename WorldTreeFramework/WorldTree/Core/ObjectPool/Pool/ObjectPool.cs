/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池。
* 掌管实体的生命周期
*

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{

    class ObjectPoolAddSystem : AddSystem<ObjectPool>
    {
        public override void OnAdd(ObjectPool self)
        {
            //生命周期系统
            self.newSystem = self.GetSystemGroup<INewSystem>();
            self.getSystem = self.GetSystemGroup<IGetSystem>();
            self.recycleSystem = self.GetSystemGroup<IRecycleSystem>();
            self.destroySystem = self.GetSystemGroup<IDestroySystem>();
        }
    }

    class ObjectPoolRemoveSystem : RemoveSystem<ObjectPool>
    {
        public override void OnRemove(ObjectPool self)
        {
            self.Dispose();//全部释放
        }
    }

    /// <summary>
    /// 实体类型对象池
    /// </summary>
    public class ObjectPool : GenericPool<object>
    {
        public SystemGroup newSystem;
        public SystemGroup getSystem;
        public SystemGroup recycleSystem;
        public SystemGroup destroySystem;

        public ObjectPool(Type type) : base()
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
            return $"[ObjectPool<{ObjectType}>] : {Count} ";
        }

        private object ObjectNew(IPool pool)
        {
            object obj = Activator.CreateInstance(ObjectType, true);

            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).thisPool = this;
            }

            if (obj is Entity)
            {
                Entity entity = obj as Entity;
                entity.id = Root.IdManager.GetId();
                entity.Root = Root;
            }
            return obj;
        }
        public override void Recycle(object obj)
        {
            lock (objetPool)
            {
                if (obj != null)
                {
                    if (maxLimit == -1 || objetPool.Count < maxLimit)
                    {
                        //假如是池管理的单位对象则可以判断是否回收了，节省时间
                        if (obj is IUnitPoolItem)
                        {
                            if ((obj as IUnitPoolItem).IsRecycle)
                            {
                                return;
                            }
                        }
                        else if (objetPool.Contains(obj)) //对象没有回收的标记，所以只能由池自己判断，比较耗时
                        {
                            return;
                        }

                        objectOnRecycle?.Invoke(obj);
                        objetPool.Enqueue(obj);
                    }
                    else
                    {
                        objectOnRecycle?.Invoke(obj);
                        objectOnDestroy?.Invoke(obj);
                        DestroyObject?.Invoke(obj);
                    }
                }
            }


        }
        private void ObjectDestroy(object obj)
        {
            (obj as IDisposable)?.Dispose();
        }

        private void ObjectOnNew(object obj)
        {
            (obj as IUnitPoolItemEvent)?.OnNew();
            if (newSystem != null && obj is Entity)
            {
                newSystem.Send(obj as Entity);
            }
        }

        private void ObjectOnGet(object obj)
        {
            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).IsRecycle = false;
            }
            (obj as IUnitPoolItemEvent)?.OnGet();



            if (getSystem != null && obj is Entity)
            {
                getSystem.Send(obj as Entity);
            }

        }

        private void ObjectOnRecycle(object obj)
        {
            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).IsRecycle = true;
            }
            (obj as IUnitPoolItemEvent)?.OnRecycle();

            if (recycleSystem != null && obj is Entity)
            {
                recycleSystem.Send(obj as Entity);
            }
        }

        private void ObjectOnDestroy(object obj)
        {
            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).IsDisposed = true;
            }
            (obj as IUnitPoolItem)?.OnDispose();

            if (destroySystem != null && obj is Entity)
            {
                destroySystem.Send(obj as Entity);
            }
        }

    }
}
