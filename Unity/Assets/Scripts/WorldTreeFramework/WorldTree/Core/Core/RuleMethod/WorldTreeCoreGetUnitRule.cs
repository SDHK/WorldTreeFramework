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
		public static T PoolGetUnit<T>(this WorldTreeCore self, out T unit)
			where T : class, IUnit
		=> unit = self.PoolGetUnit<T>();

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this WorldTreeCore self)
		where T : class, IUnit
		{
			if (self != null && self.IsCoreActive)
			{
				if (self.UnitPoolManager.TryGet(TypeInfo<T>.TypeCode, out IUnit unit))
				{
					return unit as T;
				}
			}
			T unitObj = Activator.CreateInstance(TypeInfo<T>.Type, true) as T;
			unitObj.Type = TypeInfo<T>.TypeCode;
			unitObj.Core = self;
			unitObj.OnCreate();
			return unitObj;
		}

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static IUnit PoolGetUnit(this WorldTreeCore self, long type)
		{
			if (self != null && self.IsCoreActive)
			{
				if (self.UnitPoolManager.TryGet(type, out IUnit unit))
				{
					return unit;
				}
			}
			IUnit unitObj = Activator.CreateInstance(type.CodeToType(), true) as IUnit;
			unitObj.Type = type;
			unitObj.Core = self;
			unitObj.OnCreate();
			return unitObj;
		}

		/// <summary>
		/// 回收单位
		/// </summary>
		public static void PoolRecycle(this WorldTreeCore self, IUnit obj)
		{
			if (self != null && self.IsCoreActive && obj.IsFromPool)
			{
				if (self.UnitPoolManager.TryRecycle(obj)) return;
			}
			obj.IsDisposed = true;
			obj.OnDispose();
		}
	}
}
