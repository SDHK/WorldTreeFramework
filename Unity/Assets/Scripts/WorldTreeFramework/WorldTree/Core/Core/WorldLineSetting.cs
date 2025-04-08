/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界线启动设置
*
*

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 世界线启动设置
	/// </summary>
	public class WorldLineSetting
	{
		/// <summary>
		/// 世界之心
		/// </summary>
		public Type HeartType;

		/// <summary>
		/// 世界日志
		/// </summary>
		public Type LogType;

		/// <summary>
		/// 世界线的帧时间(毫秒)
		/// </summary>
		public int FrameTime;

		public WorldLineSetting()
		{
			HeartType = typeof(WorldHeart);
			LogType = typeof(WorldLog);
			FrameTime = 1000;
		}
	}
}