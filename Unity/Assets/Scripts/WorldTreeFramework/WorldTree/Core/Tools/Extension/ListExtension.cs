/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 列表扩展方法 
	/// </summary>
	public static class ListExtension
	{
		/// <summary>
		/// 与最后一个元素交换位置并移除 
		/// </summary>
		public static void RemoveAtLast<T>(this IList<T> list, int index)
		{
			int lastIndex = list.Count - 1;
			if (index < 0 || index > lastIndex) return;
			if (index != lastIndex)
			{
				list[lastIndex] = list[index];
				list[index] = list[lastIndex];
			}
			list.RemoveAt(lastIndex);
		}
	}
}
