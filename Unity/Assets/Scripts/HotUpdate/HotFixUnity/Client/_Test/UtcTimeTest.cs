/****************************************

* 作者：闪电黑客
* 日期：2024/6/13 20:30

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// Utc时间测试
	/// </summary>
	public partial class UtcTimeTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<Awake>
		, AsRule<GuiUpdateTime>
		, AsRule<GuiUpdate>
	{
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime StartTime;

		/// <summary>
		/// 每分钟漂移更新一次
		/// </summary>
		public TimeSpan offset;

		/// <summary>
		/// 每分钟时间戳
		/// </summary>
		public DateTime OneTime;

		/// <summary>
		/// 偏移后的累计时间
		/// </summary>
		public TimeSpan difference;

		[NodeRule(nameof(AddRule<UtcTimeTest>))]
		private static void OnAddRule(UtcTimeTest self)
		{
			self.StartTime = DateTime.UtcNow;
			self.OneTime = DateTime.UtcNow;
		}

		[NodeRule(nameof(GuiUpdateRule<UtcTimeTest>))]
		private static void OnGuiUpdateRule(UtcTimeTest self)
		{

			// 需要确保 RealTimeManager 提供了 UtcNow 属性
			DateTime realTimeManagerUtcNow = self.World.Line.Core.RealTimeManager.UtcNow; // 假设这是从 RealTimeManager 获取的 UTC 时间
			DateTime systemUtcNow = DateTime.UtcNow;

			TimeSpan difference = systemUtcNow - realTimeManagerUtcNow;

			TimeSpan differenceOne = systemUtcNow - realTimeManagerUtcNow.Add(self.offset);

			GUILayout.Label($@" 

    累加时间：{realTimeManagerUtcNow}

    本机时间：{systemUtcNow} 

	启动时间：{self.StartTime} 

	上一分钟相差时间：{self.difference}  

	当前分钟相差时间：{differenceOne}  	

    当前总相差时间：{difference}    


", new GUIStyle() { fontSize = 60 });


			//当前偏移时间
			if (TimeHelper.GetTimeSpanMinutes(self.OneTime, DateTime.UtcNow) >= 1)
			{
				self.OneTime = DateTime.UtcNow;
				self.offset = difference;
				self.difference = differenceOne;
			}

		}
	}
}
