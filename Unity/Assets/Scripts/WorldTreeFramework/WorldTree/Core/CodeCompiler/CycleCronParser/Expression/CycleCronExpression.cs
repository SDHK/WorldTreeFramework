using System;

namespace WorldTree
{
	/// <summary>
	/// 周期Cron表达式解析器 
	/// </summary>
	public static partial class CycleCronExpression
	{
		/// <summary>
		/// 尝试解析周期Cron表达式，成功返回true，并输出CronBitmap；失败返回false，输出默认CronBitmap 
		/// </summary>
		public static bool TryParse(string cronExpression, CycleCronMap map)
		{
			// 如果是空字符串或仅包含空白字符，则解析失败
			if (string.IsNullOrWhiteSpace(cronExpression)) return false;
			// 分割Cron表达式为6个部分,每个部分之间用空格分割。RemoveEmptyEntries是要去掉空字符串。
			string[] parts = cronExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (map.Mode)
			{
				case CycleCronMode.DateMonthYear: return TryParseDateMonthYear(parts, map);
				case CycleCronMode.WeekMonthYear: return TryParseWeekMonthYear(parts, map);
				case CycleCronMode.Week: return TryParseWeek(parts, map);
				case CycleCronMode.Day: return TryParseDay(parts, map);
				default: return false;
			}
		}
	}
}
