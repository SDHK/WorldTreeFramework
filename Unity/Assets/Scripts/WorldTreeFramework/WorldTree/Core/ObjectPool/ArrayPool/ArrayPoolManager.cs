
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 19:00

* 描述： 一维数组对象池管理器

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 一维数组对象池管理器
	/// </summary>
	public class ArrayPoolManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
		, AsChildBranch
		, AsAwake
	{
		/// <summary>
		/// 数组对象池组字典
		/// </summary>
		public UnitDictionary<Type, ArrayPoolGroup> PoolGroupDict;
	}


	public static partial class ArrayPoolManagerRule
	{
		class AddRule : AddRule<ArrayPoolManager>
		{
			protected override void Execute(ArrayPoolManager self)
			{
				self.Core.PoolGetUnit(out self.PoolGroupDict);
			}
		}

		class RemoveRule : RemoveRule<ArrayPoolManager>
		{
			protected override void Execute(ArrayPoolManager self)
			{
				self.PoolGroupDict.Dispose();
				self.PoolGroupDict = null;
			}
		}

		/// <summary>
		/// 获取数组
		/// </summary>
		public static T[] Get<T>(this ArrayPoolManager self, int length)
		{
			return self.GetGroup(typeof(T)).GetPool(length).Get() as T[];
		}


		/// <summary>
		/// 获取数组
		/// </summary>
		public static Array Get(this ArrayPoolManager self, Type type, int length)
		{
			return self.GetGroup(type).GetPool(length).Get();
		}

		/// <summary>
		/// 回收数组
		/// </summary>
		public static void Recycle(this ArrayPoolManager self, Array obj, bool clearArray = false)
		{
			if (clearArray) Array.Clear(obj, 0, obj.Length);
			if (self.TryGetGroup(obj.GetType().GetElementType(), out ArrayPoolGroup arrayPoolGroup))
			{
				if (arrayPoolGroup.TryGetPool(obj.Length, out ArrayPool arrayPool))
				{
					arrayPool.Recycle(obj);
				}
			}
		}

		/// <summary>
		/// 获取对象池集合
		/// </summary>
		public static ArrayPoolGroup GetGroup(this ArrayPoolManager self, Type type)
		{
			if (!self.PoolGroupDict.TryGetValue(type, out ArrayPoolGroup arrayPoolGroup))
			{
				self.AddChild(out arrayPoolGroup, type);
				self.PoolGroupDict.Add(type, arrayPoolGroup);
			}
			return arrayPoolGroup;
		}

		/// <summary>
		/// 尝试获取对象池集合
		/// </summary>
		public static bool TryGetGroup(this ArrayPoolManager self, Type type, out ArrayPoolGroup arrayPoolGroup)
		{
			return self.PoolGroupDict.TryGetValue(type, out arrayPoolGroup);
		}

		/// <summary>
		/// 释放对象池集合
		/// </summary>
		public static void DisposeGroup(this ArrayPoolManager self, Type type)
		{
			if (!self.PoolGroupDict.TryGetValue(type, out ArrayPoolGroup arrayPoolGroup))
			{
				self.PoolGroupDict.Remove(type);
				arrayPoolGroup.Dispose();
			}
		}
	}
}
