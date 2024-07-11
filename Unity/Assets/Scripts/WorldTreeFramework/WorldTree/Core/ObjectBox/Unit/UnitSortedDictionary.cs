
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/10 10:33

* 描述： 单位排序字典，这个字典可由对象池管理生成和回收
* 
* 其余和普通的排序字典一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单位排序字典：可由对象池管理回收: SortedDictionary在Add的时候会产生56B的GC
	/// </summary>
	public class UnitSortedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>, IUnit
    {
        public WorldTreeCore Core { get; set; }
        public long Type { get; set; }
        public bool IsFromPool { get; set; }
        public bool IsDisposed { get; set; }

		public void Dispose()
		{
			Core.PoolRecycle(this);
		}
        public void OnCreate() { }
		public virtual void OnDispose() => Clear();
	}
}
