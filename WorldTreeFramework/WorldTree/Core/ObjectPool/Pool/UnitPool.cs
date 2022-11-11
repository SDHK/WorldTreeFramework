
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
    {
        public UnitPool(Type type) : base()
        {
            ObjectType = type;

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;
        }


        public override string ToString()
        {
            return $"[UnitPool<{ObjectType}>] : {Count} ";
        }

        private IUnitPoolEventItem ObjectNew(IPool pool)
        {
            IUnitPoolEventItem obj = Activator.CreateInstance(ObjectType, true) as IUnitPoolEventItem;
            obj.thisPool = this;
            return obj;
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

        private void ObjectDestroy(IUnitPoolEventItem obj)
        {
            (obj as IDisposable).Dispose();
        }

        private void ObjectOnNew(IUnitPoolEventItem obj)
        {
            obj.OnNew();
        }
        private void ObjectOnGet(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = false;
            obj.OnGet();
        }

        private void ObjectOnRecycle(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = true;
            obj.OnRecycle();
        }

        private void ObjectOnDestroy(IUnitPoolEventItem obj)
        {
            obj.IsDisposed = true;
            obj.OnDispose();
        }


    }
}
