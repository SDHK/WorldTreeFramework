
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/31 11:30

* 描述： 真实时间管理器，将根据机器时间计时
* 
* 带Utc的是世界标准时间，是0时区时间，
* 无论你在哪个时区，Utc时间都是一样的。
* 
* 无Utc是时区时间，经过了时区偏差的时间。
* 

*/

using System;

namespace WorldTree
{
	public static class TimeHelper
	{
		#region 计算时间跨度

		/// <summary>
		/// 获取两个时间戳的跨度
		/// </summary>
		public static TimeSpan GetTimeSpan(long startTimerTicks, long endTimerTicks)
		{
			return new TimeSpan(endTimerTicks).Subtract(new TimeSpan(startTimerTicks));
		}

		/// <summary>
		/// 获取两个时间戳跨度多少秒
		/// </summary>
		public static int GetTimeSpanSeconds(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalSeconds;
		}

		/// <summary>
		/// 获取两个时间跨度多少秒
		/// </summary>
		public static int GetTimeSpanSeconds(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalSeconds;
		}

		/// <summary>
		/// 获取两个时间戳跨度多少分钟
		/// </summary>
		public static int GetTimeSpanMinutes(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalMinutes;
		}
		/// <summary>
		/// 获取两个时间跨度多少分钟
		/// </summary>
		public static int GetTimeSpanMinutes(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalMinutes;
		}

		/// <summary>
		/// 获取两个时间戳跨度多少小时
		/// </summary>
		public static int GetTimeSpanHours(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalHours;
		}
		/// <summary>
		/// 获取两个时间跨度多少小时
		/// </summary>
		public static int GetTimeSpanHours(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalHours;
		}

		/// <summary>
		/// 获取两个时间戳跨度多少天
		/// </summary>
		public static int GetTimeSpanDays(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalDays;
		}

		/// <summary>
		/// 获取两个时间跨度多少天
		/// </summary>
		public static int GetTimeSpanDays(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalDays;
		}

		#endregion

		#region 时间单位转换

		/// <summary>
		/// 秒转分
		/// </summary>
		public static float ConvertSecondsToMinute(int seconds) => seconds / 60;
		/// <summary>
		/// 分转秒
		/// </summary>
		public static float ConvertMinuteToSeconds(int minute) => minute * 60;

		/// <summary>
		/// 分转时
		/// </summary>
		public static float ConvertMinuteToHour(int minute) => minute / 60;
		/// <summary>
		/// 时转分
		/// </summary>
		public static float ConvertHourToMinute(int hour) => hour * 60;

		/// <summary>
		/// 时转天
		/// </summary>
		public static float ConvertHourToDay(int hour) => hour / 24;

		/// <summary>
		/// 天转时
		/// </summary>
		public static float ConvertDayToHour(int day) => day / 24;

		#endregion

	}
}
