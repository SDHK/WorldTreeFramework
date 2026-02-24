using Codice.Client.Common;
using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 获取下一个匹配的时间点（日月年模式） 
		/// </summary>
		private static DateTime GetNextDateMonthYear(DateTime from, DateTime startTime, CycleCronMap map)
		{
			// 从下一秒开始计算
			DateTime current = from.AddSeconds(1);
			int nextSecond;
			int nextMinute;
			int nextHour;
			int nextDay;
			int nextMonth;
			int nextYear;
			int mask = MaskDateMonthYear(current, startTime, map);
			switch (mask)
			{
				case 1: // 秒不匹配 → 直接跳到下一个匹配的秒
					ulong secondMask = ~0UL << current.Second; // 屏蔽当前秒之前的位，未完
					ulong maskedSeconds = map.Seconds & secondMask;
					nextSecond = MathBit.GetLowestBitIndex(maskedSeconds);
					return new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, nextSecond);
				case 3: // 分不匹配
					ulong minuteMask = ~0UL << current.Minute;
					ulong maskedMinutes = map.Minutes & minuteMask;
					nextMinute = MathBit.GetLowestBitIndex(maskedMinutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, current.Month, current.Day, current.Hour, nextMinute, nextSecond);
				case 7:// 时不匹配
					uint hourMask = ~0U << current.Hour;
					uint maskedHours = map.Hours & hourMask;
					nextHour = MathBit.GetLowestBitIndex(maskedHours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, current.Month, current.Day, nextHour, nextMinute, nextSecond);
				case 15:// 日期不匹配
					uint dayMap = ComputeMonthDateBitmap(current.Year, current.Month, map);
					uint dayMask = ~0U << current.Day;
					uint maskedDays = dayMap & dayMask;
					nextDay = MathBit.GetLowestBitIndex(maskedDays);
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, current.Month, nextDay, nextHour, nextMinute, nextSecond);
				case 31: // 月份不匹配
					ulong monthMask = ~0UL << current.Month;
					ulong maskedMonths = map.Months & monthMask;
					nextMonth = MathBit.GetLowestBitIndex(maskedMonths);
					nextDay = MathBit.GetLowestBitIndex(ComputeMonthDateBitmap(current.Year, nextMonth, map));
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, nextMonth, nextDay, nextHour, nextMinute, nextSecond);
				case 63: // 年轮不匹配
					ulong cycleMask = ~0UL << (current.Year - startTime.Year);
					ulong maskedCycles = map.Cycles & cycleMask;
					if (maskedCycles == 0) maskedCycles = map.Cycles;
					nextYear = MathBit.GetLowestBitIndex(maskedCycles) + startTime.Year;
					nextMonth = MathBit.GetLowestBitIndex(map.Months);
					nextDay = MathBit.GetLowestBitIndex(ComputeMonthDateBitmap(nextYear, nextMonth, map));
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(nextYear, nextMonth, nextDay, nextHour, nextMinute, nextSecond);
				default:
					break;
			}
			return DateTime.MinValue;
		}

		/// <summary>
		/// 获取当前时间与Cron位图的差异位
		/// </summary>
		private static int MaskDateMonthYear(DateTime time, DateTime startTime, CycleCronMap map)
		{
			int timeDiff = 0;
			int dayMap = (int)ComputeMonthDateBitmap(time.Year, time.Month, map);
			if (((map.Seconds >> time.Second) & 1) == 0) timeDiff = 1;
			if (((map.Minutes >> time.Minute) & 1) == 0 || ((map.Seconds >> time.Second + 1) == 0 && timeDiff == 1)) timeDiff = 3;
			if (((map.Hours >> time.Hour) & 1) == 0 || ((map.Minutes >> time.Minute + 1) == 0 && timeDiff == 3)) timeDiff = 7;
			if (((dayMap >> time.Day) & 1) == 0 || ((map.Hours >> time.Hour + 1) == 0 && timeDiff == 7)) timeDiff = 15;
			if (((map.Months >> time.Month) & 1) == 0 || ((dayMap >> time.Day + 1) == 0 && timeDiff == 15)) timeDiff = 31;
			if (((map.Cycles >> (time.Year - startTime.Year)) & 1) == 0 || ((map.Months >> time.Month + 1) == 0 && timeDiff == 31)) timeDiff = 63;

			return timeDiff;
		}


		#region 匹配判断
		/// <summary>
		/// 匹配日期字段（支持L/W/LW特殊符号）
		/// </summary>
		private static bool MatchDate(DateTime time, CycleCronMap map)
		{
			// 如果有特殊符号
			if (map.DateOffset != 0) return MatchDateSpecial(time, map.DateOffset);
			// 普通日期匹配
			return (map.Dates >> time.Day & 1) != 0;
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
			if (targetDay > daysInMonth) targetDay = daysInMonth;

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
		#endregion

		#region 动态计算日期

		/// <summary>
		/// 计算指定年月的实际星期日期位图
		/// </summary>
		private static uint ComputeMonthDateBitmap(int year, int month, CycleCronMap map)
		{
			// 无特殊符号
			if (map.DateOffset == 0) return map.Dates;

			int daysInMonth = DateTime.DaysInMonth(year, month);
			// 只创建1个DateTime: 计算1号是星期几
			int firstDayOfWeek = (int)new DateTime(year, month, 1).DayOfWeek;
			// 转换为1=周一,...,7=周日格式
			firstDayOfWeek = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;
			return map.Dates | ComputeSpecialDate(firstDayOfWeek, daysInMonth, map.DateOffset);
		}

		/// <summary>
		/// 根据特殊符号计算实际日期位图 
		/// </summary>
		private static uint ComputeSpecialDate(int firstDayOfWeek, int daysInMonth, byte dateOffset)
		{
			int type = (dateOffset >> 6) & 0x03; // bit 7-6
			int value = dateOffset & 0x3F;        // bit 5-0

			switch (type)
			{
				case 1: // L - 月末倒数
					return ComputeLastDayOfMonth(daysInMonth, value);
				case 2: // W - 最近的工作日
					return ComputeNearestWeekday(firstDayOfWeek, daysInMonth, value);
				case 3: // LW - 月末最后一个工作日
					return ComputeLastWeekdayOfMonth(firstDayOfWeek, daysInMonth);
			}
			return 0;
		}

		/// <summary>
		/// 匹配月末倒数第N天
		/// </summary>
		private static uint ComputeLastDayOfMonth(int daysInMonth, int offset)
		{
			return 1u << (daysInMonth - offset);
		}

		/// <summary>
		/// 匹配最近的工作日 
		/// </summary>
		private static uint ComputeNearestWeekday(int firstDayOfWeek, int daysInMonth, int targetDay)
		{
			int targetDayOfWeek = (firstDayOfWeek + targetDay - 2) % 7 + 1; // 计算目标日期的星期几
			int actualDay; // 实际触发的日期
			if (targetDayOfWeek == 7) // 周日
			{
				actualDay = (targetDay + 1 <= daysInMonth) ? targetDay + 1 : targetDay - 2;
			}
			else if (targetDayOfWeek == 6) // 周六
			{
				actualDay = (targetDay - 1 >= 1) ? targetDay - 1 : targetDay + 2;
			}
			else
			{
				actualDay = targetDay; // 工作日
			}
			return 1u << actualDay;
		}

		/// <summary>
		/// 匹配月末最后一个工作日 
		/// </summary>
		private static uint ComputeLastWeekdayOfMonth(int firstDayOfWeek, int daysInMonth)
		{
			int lastDayOfWeek = (firstDayOfWeek + daysInMonth - 2) % 7 + 1; // 计算月末日期的星期几
			int offset = lastDayOfWeek == 6 ? 1   // 周六 → 往前1天到周五
					   : lastDayOfWeek == 7 ? 2     // 周日 → 往前2天到周五
					   : 0;                         // 工作日 → 不偏移
			return 1u << (daysInMonth - offset);
		}
		#endregion

	}
}
