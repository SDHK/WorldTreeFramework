
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
	public class UnitPool : GenericPool<IUnit>
		, ChildOf<PoolManagerBase<UnitPool>>
	{
		public UnitPool() : base()
		{
			NewObject = ObjectNew;
			objectOnGet = ObjectOnGet;
			objectOnRecycle = ObjectOnRecycle;
		}

		public override string ToString()
		{
			return $"[UnitPool<{ObjectType}>] : {Count} ";
		}

		public override void Recycle(object obj) => Recycle(obj as IUnit);

		/// <summary>
		/// 回收对象
		/// </summary>
		/// <param name="obj"></param>
		public void Recycle(IUnit obj)
		{
			if (obj != null)
			{
				if (maxLimit == -1 || objectPoolQueue.Count < maxLimit)
				{
					if (obj.IsDisposed) return;
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

		/// <summary>
		/// 对象新建
		/// </summary>
		/// <param name="pool"></param>
		/// <returns></returns>
		public IUnit ObjectNew(IPool pool)
		{
			IUnit obj = Activator.CreateInstance(ObjectType, true) as IUnit;
			obj.Type = ObjectTypeCode;
			obj.IsFromPool = true;
			obj.Core = Core;
			return obj;
		}

		/// <summary>
		/// 对象获取处理
		/// </summary>
		public void ObjectOnGet(IUnit obj)
		{
			obj.IsDisposed = false;
			obj.OnCreate();
		}
		/// <summary>
		/// 对象回收处理
		/// </summary>
		public void ObjectOnRecycle(IUnit obj)
		{
			obj.IsDisposed = true;
			obj.OnDispose();
		}
	}
}
