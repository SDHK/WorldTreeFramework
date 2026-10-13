/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 世界环境

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 世界环境的工作数据 
	/// </summary>
	public struct WorldContextData
	{
		/// <summary>
		/// 执行委托 
		/// </summary>
		public Delegate CallBack;

		/// <summary>
		/// 对象数据 
		/// </summary>
		public object Object;

		/// <summary>
		/// 长整型数据 
		/// </summary>
		public long Long;

		public WorldContextData(object objData = null, long longData = 0)
		{
			CallBack = null;
			Object = objData;
			Long = longData;
		}
	}
}
