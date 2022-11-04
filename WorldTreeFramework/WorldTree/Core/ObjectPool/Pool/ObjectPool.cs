/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 混合对象池
*
* 管理类型：
* 
* 1.Object
*   可对 unity 或 C# 的原生对象进行缓存入池管理，释放时会尝试调用 IDisposable 方法
* 
* 2.IUnitPoolItem 
*   对继承该接口的类 管理 回收标记 和 释放标记，以及给该对象挂上池的引用用于自我回收
*   
* 3.IUnitPoolEventItem
*   在 IUnitPoolItem 的基础上， 多了生命周期事件方法，在 生成， 获取， 回收， 销毁， 时进行调用
*   
* 4.Entity
*   在 IUnitPoolItem 的基础上， 调用 ECS 生命周期系统，
*   生成， 获取， 回收， 销毁，
*   
*   同时对 实体 赋予 根节点 和 Id 分发。
*   
* 
*/
using System;

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


    /// <summary>
    /// 混合类型对象池
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

            if (obj is IUnitPoolItem) (obj as IUnitPoolItem).thisPool = this;

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
                        //假如是池管理的单位对象则可以判断回收标记，节省时间
                        if (obj is IUnitPoolItem)
                        {
                            if ((obj as IUnitPoolItem).IsRecycle)
                            {
                                return;
                            }
                        }
                        else
                        //对象没有回收的标记，所以只能由池自己判断，比较耗时
                        if (objetPool.Contains(obj))
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
            (obj as IUnitPoolEventItem)?.OnNew();
            if (obj is Entity)
            {
                newSystem?.Send(obj as Entity);
            }
        }

        private void ObjectOnGet(object obj)
        {
            if (obj is IUnitPoolItem) (obj as IUnitPoolItem).IsRecycle = false;
            (obj as IUnitPoolEventItem)?.OnGet();

            if (obj is Entity)
            {
                getSystem?.Send(obj as Entity);
            }

        }

        private void ObjectOnRecycle(object obj)
        {
            if (obj is IUnitPoolItem) (obj as IUnitPoolItem).IsRecycle = true;
            (obj as IUnitPoolEventItem)?.OnRecycle();

            if (obj is Entity)
            {
                recycleSystem?.Send(obj as Entity);
            }
        }

        private void ObjectOnDestroy(object obj)
        {
            if (obj is IUnitPoolItem) (obj as IUnitPoolItem).IsDisposed = true;
            (obj as IUnitPoolItem)?.OnDispose();

            if (obj is Entity)
            {
                destroySystem?.Send(obj as Entity);
            }
        }

    }
}
