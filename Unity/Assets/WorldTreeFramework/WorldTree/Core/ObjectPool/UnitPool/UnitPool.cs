
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 10:55

* 描述： 单位对象池
* 
* 管理类型：继承 IUnitPoolEventItem 的对象
* 

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 单位对象池
    /// </summary>
    public class UnitPool : GenericPool<IUnitPoolEventItem>
        , ChildOf<PoolManagerBase<UnitPool>>
    {
        public UnitPool() : base()
        {
            NewObject = ObjectNew;
            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;
        }

        public override string ToString()
        {
            return $"[UnitPool<{ObjectType}>] : {Count} ";
        }

        public override void Recycle(object obj) => Recycle(obj as IUnitPoolEventItem);

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(IUnitPoolEventItem obj)
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

        public IUnitPoolEventItem ObjectNew(IPool pool)
        {
            IUnitPoolEventItem obj = Activator.CreateInstance(ObjectType, true) as IUnitPoolEventItem;
            obj.IsFromPool = true;
            obj.Core = Core;
            return obj;
        }

        public void ObjectOnNew(IUnitPoolEventItem obj)
        {
            obj.OnNew();
        }
        public void ObjectOnGet(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = false;
            obj.OnGet();
        }

        public void ObjectOnRecycle(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = true;
            obj.OnRecycle();
        }

        public void ObjectOnDestroy(IUnitPoolEventItem obj)
        {
            obj.IsDisposed = true;
            obj.OnDispose();
        }
    }
}
