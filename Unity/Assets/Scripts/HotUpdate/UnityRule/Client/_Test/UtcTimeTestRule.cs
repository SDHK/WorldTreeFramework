﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/13 20:30

* 描述：

*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{


	public static partial class UtcTimeTestRule
	{
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
