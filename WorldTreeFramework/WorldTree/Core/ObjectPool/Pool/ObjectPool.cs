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
            //注册生命周期系统
            self.newSystem = self.RootGetSystems<INewSystem>(self.ObjectType);
            self.getSystem = self.RootGetSystems<IGetSystem>(self.ObjectType);
            self.recycleSystem = self.RootGetSystems<IRecycleSystem>(self.ObjectType);
            self.destroySystem = self.RootGetSystems<IDestroySystem>(self.ObjectType);
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
        public List<ISystem> newSystem;
        public List<ISystem> getSystem;
        public List<ISystem> recycleSystem;
        public List<ISystem> destroySystem;

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

        private void ObjectDestroy(object obj)
        {
            (obj as IDisposable)?.Dispose();
        }

        private void ObjectOnNew(object obj)
        {
            (obj as IUnitPoolItemEvent)?.OnNew();
            if (newSystem != null)
            {
                foreach (INewSystem item in newSystem)
                {
                    item.New(obj);
                }
            }
        }

        private void ObjectOnGet(object obj)
        {
            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).IsRecycle = false;
            }
            (obj as IUnitPoolItemEvent)?.OnGet();
            (obj as Entity)?.SetActive(true);



            if (getSystem != null)
            {
                foreach (IGetSystem item in getSystem)
                {
                    item.Get(obj);
                }
            }

        }

        private void ObjectOnRecycle(object obj)
        {
            if (obj is IUnitPoolItem)
            {
                (obj as IUnitPoolItem).IsRecycle = true;
            }
            (obj as IUnitPoolItemEvent)?.OnRecycle();

            if (recycleSystem != null)
            {
                foreach (IRecycleSystem item in recycleSystem)
                {
                    item.Recycle(obj);
                }
            }
        }

        private void ObjectOnDestroy(object obj)
        {
            if (destroySystem != null)
            {
                foreach (IDestroySystem item in destroySystem)
                {
                    item.Destroy(obj);
                }
            }
        }

    }
}
