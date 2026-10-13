/****************************************

* 作者： 闪电黑客
* 日期： 2026/4/15 15:04

* 描述： 
* 
    框架核心层对象抽象类
	在框架核心层，启动阶段，Node还无法用时使用的对象。
*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 框架核心层对象接口 
	/// </summary>
	public interface ICoreObject : IDisposable
	{
		/// <summary>
		/// 世界线管理器 
		/// </summary>
		public WorldTreeCore Core { get; set; }
		/// <summary>
		/// 释放标记
		/// </summary>
		public bool IsDisposed { get; set; }
		/// <summary>
		/// 对象创建时的处理
		/// </summary>
		public void OnCreate();
	}

	/// <summary>
	/// 框架核心层对象抽象类 
	/// </summary>
	public abstract class CoreObject : ICoreObject
	{
		/// <summary>
		/// 世界线管理器 
		/// </summary>
		public WorldTreeCore Core { get; set; }

		/// <summary>
		/// 释放标记
		/// </summary>
		public bool IsDisposed { get; set; }

		public void Dispose()
		{
			IsDisposed = true;
			OnDispose();
			Core = null;
		}

		public virtual void OnCreate() { }


		/// <summary>
		/// 对象释放时的处理
		/// </summary>
		public virtual void OnDispose() { }
	}
}