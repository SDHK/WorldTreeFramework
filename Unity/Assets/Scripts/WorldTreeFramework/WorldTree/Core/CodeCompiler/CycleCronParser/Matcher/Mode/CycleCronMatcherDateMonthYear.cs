using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 匹配 DateMonthYear 模式 (模式1)
		/// 格式：秒 分 时 日期 月份 年轮 轮次范围
		/// </summary>
		private static bool MatchDateMonthYear(DateTime time, CycleCronMap map)
		{
			// 匹配月份
			if ((map.Months & (1U << time.Month)) == 0)
				return false;

			// 匹配日期（需要考虑特殊符号）
			if (!MatchDate(time, map))
				return false;

			// TODO: 匹配年轮和轮次
			// 需要基准时间来计算轮次
			// 暂时简化处理：匹配所有年份
			return true;
		}

		/// <summary>
		/// 匹配日期字段（支持L/W/LW特殊符号）
		/// </summary>
		private static bool MatchDate(DateTime time, CycleCronMap map)
		{
			// 如果有特殊符号
			if (map.DateOffset != 0)
			{
				return MatchDateSpecial(time, map.DateOffset);
			}

			// 普通日期匹配
			return (map.Dates & (1U << time.Day)) != 0;
		}

		/// <summary>
		/// 匹配日期特殊符号
		/// </summary>
		private static bool MatchDateSpecial(DateTime time, byte dateOffset)
		{
			int type = (dateOffset >> 6) & 0x03; // bit 7-6
			int value = dateOffset & 0x3F;        // bit 5-0

			switch (type)
			{
				case 1: // L - 月末倒数
					return MatchLastDayOfMonth(time, value);

				case 2: // W - 最近的工作日
					return MatchNearestWeekday(time, value);

				case 3: // LW - 月末最后一个工作日
					return MatchLastWeekdayOfMonth(time);

				default:
					return false;
			}
		}

		/// <summary>
		/// 匹配月末倒数第N天
		/// </summary>
		private static bool MatchLastDayOfMonth(DateTime time, int offset)
		{
			int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
			int targetDay = daysInMonth - offset;
			return time.Day == targetDay;
		}

		/// <summary>
		/// 匹配最近的工作日
		/// </summary>
		private static bool MatchNearestWeekday(DateTime time, int targetDay)
		{
			// 检查目标日期
			DateTime target = new DateTime(time.Year, time.Month, Math.Min(targetDay, DateTime.DaysInMonth(time.Year, time.Month)));
			DayOfWeek targetDayOfWeek = target.DayOfWeek;

			// 如果目标日期是工作日
			if (targetDayOfWeek != DayOfWeek.Saturday && targetDayOfWeek != DayOfWeek.Sunday)
			{
				return time.Day == target.Day;
			}

			// 如果是周六，取周五
			if (targetDayOfWeek == DayOfWeek.Saturday)
			{
				DateTime friday = target.AddDays(-1);
				// 如果周五在上个月，取周一
				if (friday.Month != target.Month)
				{
					return time.Day == target.AddDays(2).Day;
				}
				return time.Day == friday.Day;
			}

			// 如果是周日，取周一
			if (targetDayOfWeek == DayOfWeek.Sunday)
			{
				DateTime monday = target.AddDays(1);
				// 如果周一在下个月，取周五
				if (monday.Month != target.Month)
				{
					return time.Day == target.AddDays(-2).Day;
				}
				return time.Day == monday.Day;
			}

			return false;
		}

		/// <summary>
		/// 匹配月末最后一个工作日
		/// </summary>
		private static bool MatchLastWeekdayOfMonth(DateTime time)
		{
			int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
			DateTime lastDay = new DateTime(time.Year, time.Month, daysInMonth);

			// 从月末往前找第一个工作日
			while (lastDay.DayOfWeek == DayOfWeek.Saturday || lastDay.DayOfWeek == DayOfWeek.Sunday)
			{
				lastDay = lastDay.AddDays(-1);
			}

			return time.Day == lastDay.Day;
		}
	}
}
