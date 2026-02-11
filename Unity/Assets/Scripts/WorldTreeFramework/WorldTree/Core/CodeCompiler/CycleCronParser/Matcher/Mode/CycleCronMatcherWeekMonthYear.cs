using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 匹配 WeekMonthYear 模式 (模式2)
		/// 格式：秒 分 时 星期 月份 年轮 轮次范围
		/// </summary>
		private static bool MatchWeekMonthYear(DateTime time, CycleCronMap map)
		{
			// 匹配月份
			if ((map.Months & (1U << time.Month)) == 0)
				return false;

			// 匹配星期（需要考虑特殊符号）
			if (!MatchWeekday(time, map))
				return false;

			// TODO: 匹配年轮和轮次
			// 需要基准时间来计算轮次
			return true;
		}

		/// <summary>
		/// 匹配星期字段（支持#/L特殊符号）
		/// </summary>
		private static bool MatchWeekday(DateTime time, CycleCronMap map)
		{
			// 如果有特殊符号
			if (map.DateOffset != 0)
			{
				return MatchWeekdaySpecial(time, map.DateOffset);
			}

			// 普通星期匹配（1=周一, 7=周日）
			int weekday = time.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)time.DayOfWeek;
			return (map.Dates & (1U << weekday)) != 0;
		}

		/// <summary>
		/// 匹配星期特殊符号
		/// </summary>
		private static bool MatchWeekdaySpecial(DateTime time, byte dateOffset)
		{
			int type = (dateOffset >> 6) & 0x03; // bit 7-6
			int nth = (dateOffset >> 3) & 0x07;   // bit 5-3
			int weekday = dateOffset & 0x07;      // bit 2-0

			switch (type)
			{
				case 1: // L - 最后一个星期N
					return MatchLastWeekdayOfMonth(time, weekday);

				case 2: // # - 第N个星期X
					return MatchNthWeekdayOfMonth(time, weekday, nth);

				default:
					return false;
			}
		}

		/// <summary>
		/// 匹配最后一个星期N
		/// </summary>
		private static bool MatchLastWeekdayOfMonth(DateTime time, int targetWeekday)
		{
			// 转换：1-7 → DayOfWeek
			DayOfWeek target = targetWeekday == 7 ? DayOfWeek.Sunday : (DayOfWeek)targetWeekday;

			// 如果今天不是目标星期，直接返回false
			if (time.DayOfWeek != target)
				return false;

			// 检查是否是本月最后一个该星期
			DateTime nextWeek = time.AddDays(7);
			return nextWeek.Month != time.Month;
		}

		/// <summary>
		/// 匹配第N个星期X
		/// </summary>
		private static bool MatchNthWeekdayOfMonth(DateTime time, int targetWeekday, int nth)
		{
			// 转换：1-7 → DayOfWeek
			DayOfWeek target = targetWeekday == 7 ? DayOfWeek.Sunday : (DayOfWeek)targetWeekday;

			// 如果今天不是目标星期，直接返回false
			if (time.DayOfWeek != target)
				return false;

			// 计算这是本月第几个该星期
			int count = 0;
			DateTime firstDay = new DateTime(time.Year, time.Month, 1);
			DateTime current = firstDay;

			while (current.Month == time.Month)
			{
				if (current.DayOfWeek == target)
				{
					count++;
					if (current.Day == time.Day)
					{
						return count == nth;
					}
				}
				current = current.AddDays(1);
			}

			return false;
		}
	}
}
