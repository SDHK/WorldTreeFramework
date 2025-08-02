/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 10:37

* 描述： 数组对象池集合

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 数组对象池集合
	/// </summary>
	public class ArrayPoolGroup : Node, ChildOf<ArrayPoolManager>
		, AsChildBranch
		, AsRule<Awake<Type>>
	{
		/// <summary>
		/// 数组类型
		/// </summary>
		public Type ArrayType;
		/// <summary>
		/// 数组对象池集合
		/// </summary>
		public UnitDictionary<int, ArrayPool> PoolDict;
	}

	public static partial class ArrayPoolGroupRule
	{
		class AwakeRule : AwakeRule<ArrayPoolGroup, Type>
		{
			protected override void Execute(ArrayPoolGroup self, Type type)
			{
				self.ArrayType = type;
			}
		}

		class AddRule : AddRule<ArrayPoolGroup>
		{
			protected override void Execute(ArrayPoolGroup self)
			{
				self.Core.PoolGetUnit(out self.PoolDict);
			}
		}

		class RemoveRule : RemoveRule<ArrayPoolGroup>
		{
			protected override void Execute(ArrayPoolGroup self)
			{
				self.PoolDict.Dispose();
				self.PoolDict = null;
			}
		}


		/// <summary>
		/// 获取数组对象池
		/// </summary>
		public static ArrayPool GetPool(this ArrayPoolGroup self, int length)
		{
			if (!self.PoolDict.TryGetValue(length, out ArrayPool arrayPool))
			{
				self.AddChild(out arrayPool, self.ArrayType, length);
				self.PoolDict.Add(length, arrayPool);
			}
			return arrayPool;
		}

		/// <summary>
		/// 尝试获取数组对象池
		/// </summary>
		public static bool TryGetPool(this ArrayPoolGroup self, int length, out ArrayPool arrayPool)
		{
			return self.PoolDict.TryGetValue(length, out arrayPool);
		}

		/// <summary>
		/// 释放数组对象池
		/// </summary>
		public static void DisposePool(this ArrayPoolGroup self, int length)
		{
			if (self.PoolDict.TryGetValue(length, out ArrayPool arrayPool))
			{
				self.PoolDict.Remove(length);
				arrayPool.Dispose();
			}
		}
	}
}
