/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 字典扩展方法
	/// </summary>
	public static class DictionaryExtension
	{
		/// <summary>
		/// 获取或新建值
		/// </summary>
		/// <remarks>值为对象，但未通过对象池创建</remarks>
		public static V GetOrAdd<K, V>(this Dictionary<K, V> self, K key)
			where V : class, new()
		{

			if (!self.TryGetValue(key, out V value))
			{
				value = new V();
				self.Add(key, value);
			}
			return value;
		}

		/// <summary>
		/// 获取或新建值
		/// </summary>
		/// <remarks>值为对象，但未通过对象池创建</remarks>
		public static V GetOrAdd<K, V>(this ConcurrentDictionary<K, V> self, K key)
			where V : class, new()
		{
			if (!self.TryGetValue(key, out V value))
			{
				value = new V();
				self.TryAdd(key, value);
			}
			return value;
		}
	}
}
