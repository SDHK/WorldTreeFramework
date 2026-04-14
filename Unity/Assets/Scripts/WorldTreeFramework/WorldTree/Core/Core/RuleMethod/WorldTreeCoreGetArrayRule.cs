
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:46

* 描述： 

*/

using System.Buffers;

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 获取数组对象
		/// </summary>
		public static T[] PoolGetArray<T>(this WorldLine self, out T[] outT, int length)
			 => outT = ArrayPool<T>.Shared.Rent(length);

		/// <summary>
		/// 获取数组对象
		/// </summary>
		public static T[] PoolGetArray<T>(this WorldLine self, int length)
		 => ArrayPool<T>.Shared.Rent(length);

		/// <summary>
		/// 回收数组
		/// </summary>
		public static void PoolRecycle<T>(this WorldLine self, T[] array, bool clearArray = false)
		 => ArrayPool<T>.Shared.Return(array, clearArray);
	}
}
