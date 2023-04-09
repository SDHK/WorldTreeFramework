
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
    public class UnitPool : GenericPool<IUnitPoolEventItem>, IAwake<Type>, ChildOf<UnitPoolManager>
    {
        public UnitPool() : base() { }
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

        public IUnitPoolEventItem ObjectNew(IPool pool)
        {
            IUnitPoolEventItem obj = Activator.CreateInstance(ObjectType, true) as IUnitPoolEventItem;
            obj.Core = Core;
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

        public void ObjectDestroy(IUnitPoolEventItem obj)
        {
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


    class UnitPoolAddRule : AwakeRule<UnitPool, Type>
    {
        public override void OnEvent(UnitPool self, Type type)
        {
            self.ObjectType = type;

            self.NewObject = self.ObjectNew;
            self.DestroyObject = self.ObjectDestroy;

            self.objectOnNew = self.ObjectOnNew;
            self.objectOnGet = self.ObjectOnGet;
            self.objectOnRecycle = self.ObjectOnRecycle;
            self.objectOnDestroy = self.ObjectOnDestroy;
        }
    }
}
