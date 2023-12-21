/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:45

* 描述： 

*/

using System;

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this INode self, out T unit) 
			where T : class, IUnitPoolEventItem
		=> unit = self.PoolGetUnit<T>();

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this INode self)
		where T : class, IUnitPoolEventItem
		{
			if (self.Core.IsActive)
			{
				if (self.Core.UnitPoolManager.TryGet(TypeInfo<T>.TypeCode, out IUnitPoolEventItem unit))
				{
					return unit as T;
				}
			}
			T unitObj = Activator.CreateInstance(TypeInfo<T>.Type, true) as T;
			unitObj.Type = TypeInfo<T>.TypeCode;
			unitObj.Core = self.Core;
			unitObj.OnNew();
			unitObj.OnGet();
			return unitObj;
		}

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static IUnitPoolEventItem PoolGetUnit(this INode self, long type)
		{
			if (self.Core.IsActive)
			{
				if (self.Core.UnitPoolManager.TryGet(type, out IUnitPoolEventItem unit))
				{
					return unit;
				}
			}
			IUnitPoolEventItem unitObj = Activator.CreateInstance(type.CoreToType(), true) as IUnitPoolEventItem;
			unitObj.Type = type;
			unitObj.Core = self.Core;
			unitObj.OnNew();
			unitObj.OnGet();
			return unitObj;
		}

		/// <summary>
		/// 回收单位
		/// </summary>
		public static void PoolRecycle(this INode self, IUnitPoolEventItem obj)
		{
			if (self.Core.IsActive && obj.IsFromPool)
			{
				if (self.Core.UnitPoolManager.TryRecycle(obj)) return;
			}
			obj.IsRecycle = true;
			obj.OnRecycle();
			obj.IsDisposed = true;
			obj.OnDispose();
		}
	}
}
