
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/11 20:16

* 描述： 数组对象池
* 

*/

using System;

namespace WorldTree
{

	/// <summary>
	/// 数组对象池
	/// </summary>
	public class ArrayPool : GenericPool<Array>, ChildOf<ArrayPoolGroup>
		, AsAwake<Type, int>
	{
		/// <summary>
		/// 数组长度
		/// </summary>
		public int Length;

		public override string ToString()
		{
			return $"[ArrayPool<{ObjectType}>] [{Length}] : {Count} ";
		}
	}

	public static partial class ArrayPoolRule
	{
		class AwakeRule : AwakeRule<ArrayPool, Type, int>
		{
			protected override void Execute(ArrayPool self, Type type, int length)
			{
				self.ObjectType = type;
				self.Length = length;
				self.NewObject = self.ObjectNew;
				self.objectOnRecycle = self.ObjectOnRecycle;
			}
		}

		/// <summary>
		/// 创建一个数组的处理
		/// </summary>
		/// <returns></returns>
		private static Array ObjectNew(this ArrayPool self, IPool pool)
		{
			return Array.CreateInstance(self.ObjectType, self.Length);
		}

		/// <summary>
		/// 回收一个数组的处理
		/// </summary>
		private static void ObjectOnRecycle(this ArrayPool self, Array obj)
		{
			Array.Clear(obj, 0, obj.Length);
		}
	}
}
