using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 匹配 WeekMonthYear 模式 (模式2)
		/// 格式：秒 分 时 星期 月份 年轮 轮次范围
		/// </summary>
		private static int MatchWeekMonthYear(DateTime time, DateTime startTime, CycleCronMap map)
		{
			int timeDiff = 0;
			if (((map.Seconds >> time.Second) & 1) == 0) timeDiff = 1;
			if (((map.Minutes >> time.Minute) & 1) == 0) timeDiff = 3;
			if (((map.Hours >> time.Hour) & 1) == 0) timeDiff = 7;
			if (MatchWeekday(time, map)) timeDiff = 15;
			if (((map.Months >> time.Month) & 1) == 0) timeDiff = 31;
			if (((map.Cycles >> (time.Year - startTime.Year)) & 1) == 0) timeDiff = 63;
			return timeDiff;
		}

		/// <summary>
		/// 获取下一个触发时间
		/// </summary>
		private static DateTime GetNext(DateTime from, DateTime startTime, CycleCronMap map)
		{

			// 从下一秒开始计算
			DateTime current = from.AddSeconds(1);
			int nextSecond;
			int nextMinute;
			int nextHour;
			int nextDay;
			int nextMonth ;
			int nextYear;
			int mod = MatchWeekMonthYear(current, startTime, map);
			switch (mod)
			{
				case 1: // 秒不匹配
					ulong secondMask = ~0UL << current.Second; // 屏蔽当前秒之前的位
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
				case 15:// 星期不匹配
					uint dayMap = ComputeMonthWeekdayBitmap(current.Year, current.Month, map);
					uint weekdayMask = ~0U << current.Day;
					uint maskedDays = dayMap & weekdayMask;
					nextDay = MathBit.GetLowestBitIndex(maskedDays);
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, current.Month, nextDay, nextHour, nextMinute, nextSecond);
				case 31: // 月份不匹配
					ulong monthMask = ~0UL << current.Month;
					ulong maskedMonths = map.Months & monthMask;
					nextMonth = MathBit.GetLowestBitIndex(maskedMonths);
					nextDay = MathBit.GetLowestBitIndex(ComputeMonthWeekdayBitmap(current.Year, nextMonth, map));
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(current.Year, nextMonth, nextDay, nextHour, nextMinute, nextSecond);
				case 63: // 年轮不匹配
					ulong cycleMask = ~0UL << (current.Year - startTime.Year);
					ulong maskedCycles = map.Cycles & cycleMask;
					nextYear = MathBit.GetLowestBitIndex(maskedCycles) + startTime.Year;
					nextMonth = MathBit.GetLowestBitIndex(map.Months);
					nextDay = MathBit.GetLowestBitIndex(ComputeMonthWeekdayBitmap(nextYear, nextMonth, map));
					nextHour = MathBit.GetLowestBitIndex(map.Hours);
					nextMinute = MathBit.GetLowestBitIndex(map.Minutes);
					nextSecond = MathBit.GetLowestBitIndex(map.Seconds);
					return new DateTime(nextYear, nextMonth, nextDay, nextHour, nextMinute, nextSecond);
				default:
					break;
			}
			return DateTime.MinValue;
		}

		#region 匹配判断

		/// <summary>
		/// 匹配星期字段（支持#/L特殊符号）
		/// </summary>
		private static bool MatchWeekday(DateTime time, CycleCronMap map)
		{
			// 如果有特殊符号
			if (map.DateOffset != 0) if (MatchWeekdaySpecial(time, map.DateOffset)) return true;
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
			if (time.DayOfWeek != target) return false;
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

			// 计算本月1号是星期几
			int firstDayOfWeek = (int)new DateTime(time.Year, time.Month, 1).DayOfWeek;

			// 计算本月第一个目标星期几是几号
			// 例: 1号是周三(3), 要找周五(5) → (5-3+7)%7 = 2 → 1+2=3号
			//     1号是周五(5), 要找周三(3) → (3-5+7)%7 = 5 → 1+5=6号
			int targetDayOfWeek = (int)target;
			int firstTargetDay = 1 + (targetDayOfWeek - firstDayOfWeek + 7) % 7;

			// 如果当前日期小于第一个目标日期,说明不匹配
			if (time.Day < firstTargetDay)
				return false;

			// 计算当前日期是本月第几个该星期几
			// 例: 第一个是3号, 当前是10号 → (10-3)/7 + 1 = 2 (第2个)
			int weekCount = (time.Day - firstTargetDay) / 7 + 1;

			return weekCount == nth;
		}
		#endregion

		#region 星期转换日期

		/// <summary>
		/// 计算指定年月的实际星期日期位图
		/// </summary>
		private static uint ComputeMonthWeekdayBitmap(int year, int month, CycleCronMap map)
		{
			int daysInMonth = DateTime.DaysInMonth(year, month);

			// 只创建1个DateTime: 计算1号是星期几
			int firstDayOfWeek = (int)new DateTime(year, month, 1).DayOfWeek;
			// 转换为1=周一,...,7=周日格式
			firstDayOfWeek = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;

			// 无特殊符号: 快速计算所有匹配的星期
			if (map.DateOffset == 0)
			{
				return ComputeNormalWeekdays(firstDayOfWeek, daysInMonth, map.Dates);
			}
			else
			{
				// 有特殊符号: 直接计算目标日期
				return ComputeNormalWeekdays(firstDayOfWeek, daysInMonth, map.Dates) |
					ComputeSpecialWeekday(firstDayOfWeek, daysInMonth, map.DateOffset);
			}
		}

		/// <summary>
		/// 计算普通星期位图
		/// </summary>
		private static uint ComputeNormalWeekdays(int firstDayOfWeek, int daysInMonth, uint weekdayMask)
		{
			uint bitmap = 0;
			// 将firstDayOfWeek转换为0-6范围（0=周一,...,6=周日），方便位移计算
			int firstWeek = firstDayOfWeek - 1;

			// 覆盖第一周，通过firstWeek 将匹配的星期设置到位图中
			bitmap |= weekdayMask >> firstWeek;

			// 计算1号到第一个匹配的星期之间的天数，
			// 假设firstWeek是3（星期4），那么结果是4，表示1号到第一个匹配的星期之间有4天
			int firstWeekDays = (7 - firstWeek);
			// 覆盖第二个星期开始的位图，注意bitmap包含了无意义的0位。所以这里firstWeekDays不用-1。
			bitmap |= weekdayMask << firstWeekDays;

			// 算出剩余天数还能覆盖多少个星期
			int remainingDays = daysInMonth - firstWeekDays - 7;
			// 每7天会重复一次星期位图，通过位移实现
			while (remainingDays >= 7)
			{
				bitmap |= bitmap << 7;
				remainingDays -= 7;
			}
			return bitmap;
		}

		/// <summary>
		/// 计算特殊符号星期位图
		/// </summary>
		private static uint ComputeSpecialWeekday(int firstDayOfWeek, int daysInMonth, byte dateOffset)
		{
			int type = (dateOffset >> 6) & 0x03;
			int nth = (dateOffset >> 3) & 0x07;
			int targetWeekday = dateOffset & 0x07;

			// 计算本月第一个目标星期几是几号
			int firstOccurrence = 1 + (targetWeekday - firstDayOfWeek + 7) % 7;
			int targetDay = -1;

			switch (type)
			{
				case 1: // L - 最后一个星期X
						// 数学计算: 找最后一个不超过月末的日期
					targetDay = firstOccurrence + ((daysInMonth - firstOccurrence) / 7) * 7;
					break;

				case 2: // # - 第N个星期X
					targetDay = firstOccurrence + (nth - 1) * 7;
					// 验证是否超出月末
					if (targetDay > daysInMonth) targetDay = -1;
					break;
			}
			return targetDay > 0 ? (1U << targetDay) : 0;
		}
		#endregion
	}
}
