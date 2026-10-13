/****************************************

* 作者：闪电黑客
* 日期：2024/6/14 17:04

* 描述：

*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 时间帮助类
	/// </summary>
	public static class TimeHelper
	{
		#region 时间常量

		/// <summary>
		/// 一毫秒，10000刻度
		/// </summary>
		public const int MILLI_TICK = 10000;

		/// <summary>
		/// 一秒，1000毫秒
		/// </summary>
		public const int SECOND = 1000;

		/// <summary>
		/// 一分钟，60000毫秒
		/// </summary>
		public const long MINUTE = 60000;

		/// <summary>
		/// 一小时，3600000毫秒
		/// </summary>
		public const long HOUR = 3600000;

		/// <summary>
		/// 一天，86400000毫秒
		/// </summary>
		public const long DAY = 86400000;

		#endregion

		#region 计算时间跨度

		/// <summary>
		/// 获取两个时间戳的跨度
		/// </summary>
		public static TimeSpan GetTimeSpan(long startTimerTicks, long endTimerTicks)
		{
			return new TimeSpan(endTimerTicks).Subtract(new TimeSpan(startTimerTicks));
		}

		/// <summary>
		/// 获取两个时间刻度跨度多少秒
		/// </summary>
		public static int GetTimeSpanSeconds(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalSeconds;
		}

		/// <summary>
		/// 获取两个时间跨多少秒
		/// </summary>
		public static int GetTimeSpanSeconds(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalSeconds;
		}

		/// <summary>
		/// 获取两个时间刻度跨多少分钟
		/// </summary>
		public static int GetTimeSpanMinutes(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalMinutes;
		}

		/// <summary>
		/// 获取两个时间跨多少分钟
		/// </summary>
		public static int GetTimeSpanMinutes(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalMinutes;
		}

		/// <summary>
		/// 获取两个时间刻度跨度多少小时
		/// </summary>
		public static int GetTimeSpanHours(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalHours;
		}

		/// <summary>
		/// 获取两个时间跨多少小时
		/// </summary>
		public static int GetTimeSpanHours(DateTime startTimerTicks, DateTime endTimerTicks)
		{
			return (int)(endTimerTicks - startTimerTicks).TotalHours;
		}

		/// <summary>
		/// 获取两个时间刻度跨多少天
		/// </summary>
		public static int GetTimeSpanDays(long startTimerTicks, long endTimerTicks)
		{
			return (int)(TimeSpan.FromTicks(endTimerTicks) - TimeSpan.FromTicks(startTimerTicks)).TotalDays;
		}

		/// <summary>
		/// 获取两个时间跨多少天
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