
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
            lock (objectPoolQueue)
            {
                if (obj != null)
                {
                    if (maxLimit == -1 || objectPoolQueue.Count < maxLimit)
                    {
                        if (obj.IsRecycle) return;
                        objectOnRecycle.Invoke(obj);
                        objectPoolQueue.Enqueue(obj);
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

		/// <summary>
		/// 对象新建
		/// </summary>
		/// <param name="pool"></param>
		/// <returns></returns>
		public IUnitPoolEventItem ObjectNew(IPool pool)
        {
            IUnitPoolEventItem obj = Activator.CreateInstance(ObjectType, true) as IUnitPoolEventItem;
            obj.Type = ObjectTypeCode;
            obj.IsFromPool = true;
            obj.Core = Core;
            return obj;
        }

        /// <summary>
        /// 对象新建处理
        /// </summary>
        public void ObjectOnNew(IUnitPoolEventItem obj)
        {
            obj.OnNew();
        }
		/// <summary>
		/// 对象获取处理
		/// </summary>
		public void ObjectOnGet(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = false;
            obj.OnGet();
        }
		/// <summary>
		/// 对象回收处理
		/// </summary>
		public void ObjectOnRecycle(IUnitPoolEventItem obj)
        {
            obj.IsRecycle = true;
            obj.OnRecycle();
        }
		/// <summary>
		/// 对象销毁处理
		/// </summary>
		public void ObjectOnDestroy(IUnitPoolEventItem obj)
        {
            obj.IsDisposed = true;
            obj.OnDispose();
        }
    }
}
