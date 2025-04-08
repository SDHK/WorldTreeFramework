/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界线程
*

*/

using System;
using System.Collections.Concurrent;

namespace WorldTree
{

	/// <summary>
	/// 世界线管理器
	/// </summary>
	public class WorldLineManager
	{
		/// <summary>
		/// 世界线集合
		/// </summary>
		private ConcurrentDictionary<int, IWorldLine> lineDict;

		/// <summary>
		/// 主世界线
		/// </summary>
		private IWorldLine mainLine;

		/// <summary>
		/// 创建世界线
		/// </summary>
		public void Create(int id, Type type, int frameTime)
		{

		}
	}

}