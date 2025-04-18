﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 单位栈，这个栈可由对象池管理生成和回收
* 
* 其余和普通的Stack一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单位栈:这个栈可由对象池管理生成和回收
	/// </summary>
	[TreeDataSerializable]
	public partial class UnitStack<T> : Stack<T>, IUnit
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
