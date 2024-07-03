/****************************************

* 作者： 闪电黑客
* 日期： 2022/5/18 15:07

* 描述： 单位对象基类
* 
* 设定为可以回收到对象池的对象，
* Unit对象为框架启动阶段，Node还无法用时使用的对象。
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 单位基类
	/// </summary>
	public abstract class Unit : IUnit
	{
		public long Type { get; set; }
		public bool IsDisposed { get; set; }


		public bool IsFromPool { get; set; }

		public WorldTreeCore Core { get; set; }


		public virtual void OnCreate() { }

		/// <summary>
		/// 释放对象
		/// </summary>
		public virtual void Dispose()
		{
			Core.PoolRecycle(this);
		}

		public virtual void OnDispose() { }
	}
}
