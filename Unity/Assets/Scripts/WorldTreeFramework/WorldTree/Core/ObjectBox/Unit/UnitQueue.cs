
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 单位队列，这个队列可由对象池管理生成和回收
* 
* 其余和普通的Queue一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单元队列：可由对象池管理回收
	/// </summary>
	public class UnitQueue<T> : Queue<T>, IUnit
	{
		public WorldLine Core { get; set; }
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

	/// <summary>
	/// 单元并发队列：可由对象池管理回收
	/// </summary>
	public class UnitConcurrentQueue<T> : Queue<T>, IUnit
	{
		public WorldLine Core { get; set; }
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
