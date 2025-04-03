
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:46

* 描述： 

*/

using System;

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 获取数组对象
		/// </summary>
		public static T[] PoolGetArray<T>(this WorldTreeCore self, out T[] outT, int length)
		{
			Type type = typeof(T);
			if (self.ArrayPoolManager != null)
			{
				lock (self.ArrayPoolManager)
				{
					outT = self.ArrayPoolManager.Get(type, length) as T[];
				}
			}
			else
			{
				outT = Array.CreateInstance(type, length) as T[];
			}
			return outT;
		}

		/// <summary>
		/// 获取数组对象
		/// </summary>
		public static T[] PoolGetArray<T>(this WorldTreeCore self, int length)
		{
			Type type = typeof(T);
			if (self.ArrayPoolManager != null)
			{
				lock (self.ArrayPoolManager)
				{
					return self.ArrayPoolManager.Get(type, length) as T[];
				}
			}
			return Array.CreateInstance(type, length) as T[];
		}

		/// <summary>
		/// 获取数组对象
		/// </summary>
		public static Array PoolGetArray(this WorldTreeCore self, Type type, int length)
		{
			if (self.ArrayPoolManager != null)
			{
				lock (self.ArrayPoolManager)
				{
					return self.ArrayPoolManager.Get(type, length);
				}
			}
			return Array.CreateInstance(type, length);
		}


		/// <summary>
		/// 回收数组
		/// </summary>
		public static void PoolRecycle(this WorldTreeCore self, Array obj, bool clearArray = false)
		{
			if (self.ArrayPoolManager != null)
			{
				lock (self.ArrayPoolManager)
				{
					self.ArrayPoolManager.Recycle(obj, clearArray);
				}
			}
			else if (clearArray)
			{
				Array.Clear(obj, 0, obj.Length);
			}
		}

	}
}
