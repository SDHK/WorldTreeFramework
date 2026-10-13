using System;

namespace WorldTree
{
	/// <summary>
	/// 周期Cron匹配器（静态工具类）
	/// 提供高性能的时间匹配和下次触发时间计算
	/// </summary>
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 判断指定时间是否匹配Cron表达式
		/// </summary>
		/// <param name="time">要检查的时间</param>
		/// <param name="map">Cron位图</param>
		/// <returns>是否匹配</returns>
		public static int IsMatch(DateTime time, CycleCronMap map)
		{
			switch (map.Mode)
			{
				//case CycleCronMode.DateMonthYear:
				//	return MatchDateMonthYear(time, time, map);
				//case CycleCronMode.WeekMonthYear:
				//	return MaskWeekMonthYear(time, time, map);
				//case CycleCronMode.Week:
				//	return MatchWeek(time, map);
				//case CycleCronMode.Day:
				//	return MatchDay(time, map);
				default:
					return 0;
			}

		}


		/// <summary>
		/// 获取下一个触发时间（完全无循环，纯位运算版本）
		/// 层级式回退：秒 → 分 → 时 → 日 → 月 → 年
		/// </summary>
		public static DateTime GetNext(DateTime from, CycleCronMap map, DateTime startTime = default)
		{
			switch (map.Mode)
			{
				case CycleCronMode.DateMonthYear:
					return GetNextDateMonthYear(from, startTime, map);
				case CycleCronMode.WeekMonthYear:
					return GetNextWeekMonthYear(from, startTime, map);
				//case CycleCronMode.Week:
				//case CycleCronMode.Day:
			}
			return DateTime.MaxValue;
		}
	}
}
