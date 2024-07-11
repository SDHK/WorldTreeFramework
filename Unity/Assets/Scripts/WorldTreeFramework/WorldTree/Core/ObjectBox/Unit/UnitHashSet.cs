
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/18 19:23

* 描述： 单位HashSet，可由对象池管理生成和回收
* 
* 其余和普通的HashSet一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单元哈希集合：可由对象池管理回收
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class UnitHashSet<T> : HashSet<T>, IUnit
    {
        public WorldTreeCore Core { get; set; }
        public long Type { get; set; }
        public bool IsFromPool { get; set; }
        public bool IsDisposed { get; set; }

		public void OnCreate() { }

		public void Dispose()
		{
			Core.PoolRecycle(this);
		}
		public virtual void OnDispose() => Clear();
	}
}
