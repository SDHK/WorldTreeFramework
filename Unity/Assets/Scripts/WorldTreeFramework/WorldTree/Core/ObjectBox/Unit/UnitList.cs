﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/12 16:23

* 描述： 单位列表，这个列表可由对象池管理生成和回收
* 
* 其余和普通的List一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单位列表：可由对象池管理回收
	/// </summary>
	[TreeDataSerializable()]
	public partial class UnitList<T> : List<T>, IUnit
	{
		[TreeDataIgnore]
		public WorldLine Core { get; set; }
		[TreeDataIgnore]
		public long Type { get; set; }
		[TreeDataIgnore]
		public bool IsFromPool { get; set; }
		[TreeDataIgnore]
		public bool IsDisposed { get; set; }

		public void Dispose()
		{
			Core.PoolRecycle(this);
		}
		public void OnCreate() { }

		public virtual void OnDispose() => Clear();

	}
}
