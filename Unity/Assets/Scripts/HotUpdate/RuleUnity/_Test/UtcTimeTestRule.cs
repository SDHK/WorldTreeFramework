using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	///// <summary>
	///// 测试
	///// </summary>
	//public interface IT0 { }

	//public class T1 { }

	//public struct T2 { }

	//public enum T3 { }

	public static partial class UtcTimeTestRule
	{
		//public static int T5;

		//public const int T6 = 0;

		//protected const int T7 = 0;

		private static int T8 = 0;

		private static OnAdd<UtcTimeTest> OnAdd = (self) =>
		{
			self.StartTime = DateTime.UtcNow;
			self.OneTime = DateTime.UtcNow;
		};

		private static OnGuiUpdateTime<UtcTimeTest> OnGui = (self, time) =>
		{

			// 需要确保 RealTimeManager 提供了 UtcNow 属性
			DateTime realTimeManagerUtcNow = self.Core.RealTimeManager.UtcNow; // 假设这是从 RealTimeManager 获取的 UTC 时间
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

		};




	}
}
