
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/12 16:45

* 描述： 单位字典，这个字典可由对象池管理生成和回收
* 
* 其余和普通的字典一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单位字典：可由对象池管理回收
	/// </summary>
	[TreeDataSerializable]
	[TreeCopyable]
	public partial class UnitDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IUnit
	{
		[TreeDataIgnore]
		[TreeCopyIgnore]
		public WorldLine Core { get; set; }
		[TreeDataIgnore]
		[TreeCopyIgnore]
		public long Type { get; set; }
		[TreeDataIgnore]
		[TreeCopyIgnore]
		public bool IsFromPool { get; set; }
		[TreeDataIgnore]
		[TreeCopyIgnore]
		public bool IsDisposed { get; set; }

		public virtual void OnCreate() { }

		public void Dispose()
		{
			Core.PoolRecycle(this);
		}

		public virtual void OnDispose() => Clear();

	}
}
