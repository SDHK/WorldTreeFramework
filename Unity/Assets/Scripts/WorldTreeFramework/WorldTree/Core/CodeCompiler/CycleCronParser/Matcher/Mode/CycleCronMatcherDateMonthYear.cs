using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{


		/// <summary>
		/// 获取当前时间与Cron位图的差异位
		/// </summary>
		private static int MatchDateMonthYear(DateTime time, DateTime startTime, CycleCronMap map)
		{
			int timeDiff = 0;
			if (((map.Seconds >> time.Second) & 1) == 0) timeDiff = 1;
			if (((map.Minutes >> time.Minute) & 1) == 0) timeDiff = 3;
			if (((map.Hours >> time.Hour) & 1) == 0) timeDiff = 7;
			if (!MatchDate(time, map)) timeDiff = 15;
			if (((map.Months >> time.Month) & 1) == 0) timeDiff = 31;
			if (((map.Cycles >> (time.Year - startTime.Year)) & 1) == 0) timeDiff = 63;
			return timeDiff;
		}

		/// <summary>
		/// 匹配日期字段（支持L/W/LW特殊符号）
		/// </summary>
		private static bool MatchDate(DateTime time, CycleCronMap map)
		{
			// 如果有特殊符号
			if (map.DateOffset != 0)return MatchDateSpecial(time, map.DateOffset);
			// 普通日期匹配
			return (map.Dates>> time.Day & 1) != 0;
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
			return time.Day == DateTime.DaysInMonth(time.Year, time.Month) - offset;
		}

		/// <summary>
		/// 匹配最近的工作日
		/// </summary>
		private static bool MatchNearestWeekday(DateTime time, int targetDay)
		{
			int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
			// 限制目标日期在当月范围内
			if (targetDay > daysInMonth)targetDay = daysInMonth;

			// 计算目标日期的星期几(蔡勒公式或直接用 DateTime)
			DayOfWeek targetDayOfWeek = new DateTime(time.Year, time.Month, targetDay).DayOfWeek;
			int actualDay; // 实际触发的日期
			if (targetDayOfWeek == DayOfWeek.Sunday) 
			{
				// 周日 → 优先取周一(+1),如果周一超出当月则取周五(-2)
				actualDay = (targetDay + 1 <= daysInMonth) ? targetDay + 1 : targetDay - 2;
			}
			else if (targetDayOfWeek == DayOfWeek.Saturday) 
			{
				// 周六 → 优先取周五(-1),如果周五小于1则取周一(+2)
				actualDay = (targetDay - 1 >= 1) ? targetDay - 1 : targetDay + 2;
			}
			else
			{
				// 工作日 → 不偏移
				actualDay = targetDay;
			}
			return time.Day == actualDay;
		}

		/// <summary>
		/// 匹配月末最后一个工作日
		/// </summary>
		private static bool MatchLastWeekdayOfMonth(DateTime time)
		{
			int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
			DayOfWeek lastDayOfWeek = new DateTime(time.Year, time.Month, daysInMonth).DayOfWeek;

			// 计算需要往前偏移的天数
			int offset = lastDayOfWeek == DayOfWeek.Saturday ? 1   // 周六 → 往前1天到周五
					   : lastDayOfWeek == DayOfWeek.Sunday ? 2     // 周日 → 往前2天到周五
					   : 0;                                         // 工作日 → 不偏移

			return time.Day == daysInMonth - offset;
		}
	}
}
