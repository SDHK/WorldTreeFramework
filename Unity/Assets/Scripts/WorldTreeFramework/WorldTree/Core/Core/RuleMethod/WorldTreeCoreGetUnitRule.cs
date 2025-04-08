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
		/// 新建单位对象
		/// </summary>
		public static T NewUnit<T>(this WorldLine self, out T unit) where T : class, IUnit
			=> unit = self.NewUnit(typeof(T), out _) as T;

		/// <summary>
		/// 新建单位对象
		/// </summary>
		public static IUnit NewUnit(this WorldLine self, Type type, out IUnit unit)
		{
			unit = Activator.CreateInstance(type, true) as IUnit;
			unit.Core = self;
			unit.Type = unit.TypeToCode(type);
			unit.OnCreate();
			return unit;
		}


		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this WorldLine self, out T unit) where T : class, IUnit => unit = self.PoolGetUnit<T>();

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this WorldLine self)
		where T : class, IUnit
		{
			if (self != null && self.IsCoreActive)
			{
				lock (self.UnitPoolManager)
				{
					if (self.UnitPoolManager.TryGet(self.TypeToCode<T>(), out IUnit unit))
					{
						return unit as T;
					}
				}
			}
			return self.NewUnit<T>(out _);
		}

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static IUnit PoolGetUnit(this WorldLine self, long type)
		{
			if (self != null && self.IsCoreActive)
			{
				lock (self.UnitPoolManager)
				{
					if (self.UnitPoolManager.TryGet(type, out IUnit unit))
					{
						return unit;
					}
				}
			}
			return self.NewUnit(self.CodeToType(type), out _);
		}

		/// <summary>
		/// 回收单位
		/// </summary>
		public static void PoolRecycle(this WorldLine self, IUnit obj)
		{
			if (self != null && self.IsCoreActive && obj.IsFromPool)
			{
				lock (self.UnitPoolManager)
				{
					if (self.UnitPoolManager.TryRecycle(obj)) return;
				}
			}
			obj.IsDisposed = true;
			obj.OnDispose();
		}
	}
}
