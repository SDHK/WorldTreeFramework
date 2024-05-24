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
		, AsAwake<Type>
		, AsChildBranch
	{
		/// <summary>
		/// 数组类型
		/// </summary>
		public Type ArrayType;
		public TreeDictionary<int, ArrayPool> Pools;
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
				self.AddChild(out self.Pools);
			}
		}

		class RemoveRule : RemoveRule<ArrayPoolGroup>
		{
			protected override void Execute(ArrayPoolGroup self)
			{
				self.Pools = null;
			}
		}


		/// <summary>
		/// 获取数组对象池
		/// </summary>
		public static ArrayPool GetPool(this ArrayPoolGroup self, int Length)
		{
			if (!self.Pools.TryGetValue(Length, out ArrayPool arrayPool))
			{
				self.AddChild(out arrayPool, self.ArrayType, Length);
				self.Pools.Add(Length, arrayPool);
			}
			return arrayPool;
		}

		/// <summary>
		/// 尝试获取数组对象池
		/// </summary>
		public static bool TryGetPool(this ArrayPoolGroup self, int Length, out ArrayPool arrayPool)
		{
			return self.Pools.TryGetValue(Length, out arrayPool);
		}

		/// <summary>
		/// 释放数组对象池
		/// </summary>
		public static void DisposePool(this ArrayPoolGroup self, int Length)
		{
			if (self.Pools.TryGetValue(Length, out ArrayPool arrayPool))
			{
				self.Pools.Remove(Length);
				arrayPool.Dispose();
			}
		}
	}
}
