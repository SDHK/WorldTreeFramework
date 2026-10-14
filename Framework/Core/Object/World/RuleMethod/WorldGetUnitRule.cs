/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:45

* 描述： 

*/

using System;

namespace WorldTree
{
	public static partial class WorldRule
	{

		/// <summary>
		/// 新建单位对象
		/// </summary>
		public static T NewUnit<T>(this World self, out T unit) where T : class, IUnit
			=> unit = self.NewUnit(typeof(T), out _) as T;

		/// <summary>
		/// 新建单位对象
		/// </summary>
		public static IUnit NewUnit(this World self, Type type, out IUnit unit)
		{
			unit = Activator.CreateInstance(type, true) as IUnit;
			unit.World = self;
			unit.Type = unit.TypeToCode(type);
			RefreshUnitId(self, unit);
			unit.OnCreate();
			return unit;
		}

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this World self, out T unit) where T : class, IUnit => unit = self.PoolGetUnit<T>();

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static T PoolGetUnit<T>(this World self)
		where T : class, IUnit
		{
			if (self.Line.Core.UnitPoolManager.TryGet(out T unit))
			{
				unit.World = self;
				RefreshUnitId(self, unit);
				unit.OnCreate();
				return unit;
			}
			return self.NewUnit<T>(out _);
		}

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static IUnit PoolGetUnit(this World self, Type type)
		{
			if (self.Line.Core.UnitPoolManager.TryGet(type, out object obj))
			{
				IUnit unit = obj as IUnit;
				unit.World = self;
				RefreshUnitId(self, unit);
				unit.OnCreate();
				return unit;
			}
			return self.NewUnit(type, out _);
		}

		/// <summary>
		/// 取出单位时换一次实例号。普通单位的 Id 与 InstanceId 相同。
		/// </summary>
		private static void RefreshUnitId(World self, IUnit unit)
		{
			unit.InstanceId = self.Line.Core.IdManager.GetId();
			unit.Id = unit.InstanceId;
		}

		/// <summary>
		/// 回收单位
		/// </summary>
		public static void PoolRecycle(this World self, IUnit obj)
		{
			if (obj.IsFromPool)
			{
				if (self.Line.Core.UnitPoolManager.TryRecycle(obj)) return;
			}
			obj.IsDisposed = true;
			obj.OnDispose();
		}
	}
}
