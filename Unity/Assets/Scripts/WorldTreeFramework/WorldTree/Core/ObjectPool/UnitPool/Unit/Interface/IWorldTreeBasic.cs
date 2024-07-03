/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 13:40:18

 * 最后日期: 2021/12/15 18:32:23

 * 最后修改: 闪电黑客

 * 描述:  
  
    框架最底层基础对象的接口
	给实现接口的类一个统一的销毁标记，和方法

******************************/

using System;

namespace WorldTree
{
	/// <summary>
	/// 框架最底层基础对象接口
	/// </summary>
	public interface IWorldTreeBasic : IDisposable
	{
		/// <summary>
		/// 类型码
		/// </summary>
		public long Type { get; set; }

		/// <summary>
		/// 释放标记
		/// </summary>
		bool IsDisposed { get; set; }

		/// <summary>
		/// 是否从池获取
		/// </summary>
		public bool IsFromPool { get; set; }

		/// <summary>
		/// 世界树核心
		/// </summary>
		/// <remarks>框架的核心</remarks>
		public WorldTreeCore Core { get; set; }

    }
}