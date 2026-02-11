using System;
using static Codice.CM.Common.CmCallContext;

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
		public static bool IsMatch(DateTime time, CycleCronMap map)
		{
			// 匹配秒、分、时（所有模式都需要）
			if (!MatchTime(time, map)) return false;

			// 根据模式匹配其他字段
			switch (map.Mode)
			{
				case CycleCronMode.DateMonthYear:
					return MatchDateMonthYear(time, map);
				case CycleCronMode.WeekMonthYear:
					return MatchWeekMonthYear(time, map);
				case CycleCronMode.Week:
					return MatchWeek(time, map);
				case CycleCronMode.Day:
					return MatchDay(time, map);
				default:
					return false;
			}
		}

		/// <summary>
		/// 匹配秒、分、时（通用）
		/// </summary>
		private static bool MatchTime(DateTime time, CycleCronMap map)
		{
			// 位运算匹配，O(1)复杂度
			return (map.Seconds & (1UL << time.Second)) != 0
				&& (map.Minutes & (1UL << time.Minute)) != 0
				&& (map.Hours & (1U << time.Hour)) != 0;
		}

		

		/// <summary>
		/// 匹配日期、月份、年份 
		/// </summary>
		private static bool MatchDateMonthYear1(int currentTime,ulong map, int max,int nextTime)
		{
			ulong timeMask = ~0UL << currentTime;
			ulong maskedTime = map & timeMask;
			nextTime = MathBit.GetLowestBitIndex(maskedTime);
			if (nextTime != -1) return false;
			// 如果没有找到更大的时间点，说明需要回退到下一个周期的起点
			nextTime = MathBit.GetLowestBitIndex(map);
			return true;
		}

		/// <summary>
		/// 获取下一个触发时间（完全无循环，纯位运算版本）
		/// 层级式回退：秒 → 分 → 时 → 日 → 月 → 年
		/// 注意：暂不处理特殊符号（L/W/#等），DateOffset != 0 时返回 null
		/// </summary>
		/// <param name="from">起始时间（不包含）</param>
		/// <param name="map">Cron位图</param>
		/// <param name="baseTime">基准时间（用于计算轮次，如开服时间）</param>
		/// <returns>下一个触发时间，如果没有则返回null</returns>
		public static DateTime? GetNext(DateTime from, CycleCronMap map, DateTime? baseTime = null)
		{
			// 暂不支持特殊符号
			if (map.DateOffset != 0)
				return null;

			// 从下一秒开始计算
			DateTime current = from.AddSeconds(1);

		// === 1. 尝试在当前分钟内找下一秒 ===
		ulong secondMask = ~0UL << current.Second; // 屏蔽当前秒之前的位
		ulong maskedSeconds = map.Seconds & secondMask;
		
		if (maskedSeconds != 0)
		{
			int nextSecond = MathBit.GetLowestBitIndex(maskedSeconds);
			DateTime candidate = new DateTime(current.Year, current.Month, current.Day, 
			                                 current.Hour, current.Minute, nextSecond);
			// 验证日期和月份是否匹配
			if (IsDateMonthMatch(candidate, map)) return candidate;
		}
		
		// === 2. 在当前小时内找下一分钟 ===
		ulong minuteMask = ~0UL << current.Minute;
		ulong maskedMinutes = map.Minutes & minuteMask;
		
		if (maskedMinutes != 0)
		{
			int nextMinute = MathBit.GetLowestBitIndex(maskedMinutes);
			int firstSecond = MathBit.GetLowestBitIndex(map.Seconds);
			if (firstSecond < 0)
			{
				//异常情况：没有任何秒数匹配，无法找到下一个时间点
			}
			
			DateTime candidate = new DateTime(current.Year, current.Month, current.Day,
			                                 current.Hour, nextMinute, firstSecond);
			if (IsDateMonthMatch(candidate, map))
				return candidate;
		}
		
		// === 3. 在今天找下一小时 ===
		uint hourMask = ~0U << current.Hour;
		uint maskedHours = map.Hours & hourMask;

			if (maskedHours != 0)
			{
				int nextHour = MathBit.GetLowestBitIndex(maskedHours);
				int firstMinute = MathBit.GetLowestBitIndex(map.Minutes);
				int firstSecond = MathBit.GetLowestBitIndex(map.Seconds);
				if (firstMinute < 0 || firstSecond < 0) return null;

				DateTime candidate = new DateTime(current.Year, current.Month, current.Day,
												 nextHour, firstMinute, firstSecond);
				if (IsDateMonthMatch(candidate, map))
					return candidate;
			}

		// === 4. 在当前月找下一日期 ===
		uint dateMask = ~0U << current.Day;
		uint maskedDates = map.Dates & dateMask;
		
		if (maskedDates != 0)
		{
			int nextDate = MathBit.GetLowestBitIndex(maskedDates);
			// 验证日期有效性（不同月份天数不同）
			int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);
			if (nextDate <= daysInMonth)
			{
				int firstHour = MathBit.GetLowestBitIndex(map.Hours);
				int firstMinute = MathBit.GetLowestBitIndex(map.Minutes);
				int firstSecond = MathBit.GetLowestBitIndex(map.Seconds);
				if (firstHour < 0 || firstMinute < 0 || firstSecond < 0) return null;
				
				DateTime candidate = new DateTime(current.Year, current.Month, nextDate,
				                                 firstHour, firstMinute, firstSecond);
				if (IsMonthMatch(candidate.Month, map))
					return candidate;
			}
		}
		
		// === 5. 在当前年找下一月份 ===
		uint monthMask = ~0U << current.Month;
		uint maskedMonths = map.Months & monthMask;

			if (maskedMonths != 0)
			{
				int nextMonth = MathBit.GetLowestBitIndex(maskedMonths);
				// 找该月第一个有效日期
				int firstDate = MathBit.GetLowestBitIndex(map.Dates);
				if (firstDate < 0) return null;

				// 验证日期有效性
				int daysInMonth = DateTime.DaysInMonth(current.Year, nextMonth);
				if (firstDate > daysInMonth)
				{
					// 该月没有这个日期，尝试下一个月
					// 注意：这里需要递归或循环，先简化处理返回null
					return null; // TODO: 可以继续查找下一个月
				}

				int firstHour = MathBit.GetLowestBitIndex(map.Hours);
				int firstMinute = MathBit.GetLowestBitIndex(map.Minutes);
				int firstSecond = MathBit.GetLowestBitIndex(map.Seconds);
				if (firstHour < 0 || firstMinute < 0 || firstSecond < 0) return null;

				return new DateTime(current.Year, nextMonth, firstDate,
								   firstHour, firstMinute, firstSecond);
			}

			// === 6. 进入下一年 ===
			int nextYear = current.Year + 1;
			if (nextYear > 9999) return null; // DateTime 最大年份

			int firstMonth = MathBit.GetLowestBitIndex(map.Months);
			int yearFirstDate = MathBit.GetLowestBitIndex(map.Dates);
			int yearFirstHour = MathBit.GetLowestBitIndex(map.Hours);
			int yearFirstMinute = MathBit.GetLowestBitIndex(map.Minutes);
			int yearFirstSecond = MathBit.GetLowestBitIndex(map.Seconds);

			if (firstMonth < 0 || yearFirstDate < 0 || yearFirstHour < 0 ||
				yearFirstMinute < 0 || yearFirstSecond < 0)
				return null;

			// 验证日期有效性
			int daysInFirstMonth = DateTime.DaysInMonth(nextYear, firstMonth);
			if (yearFirstDate > daysInFirstMonth)
				return null; // TODO: 可以继续查找该年的下一个月

			return new DateTime(nextYear, firstMonth, yearFirstDate,
							   yearFirstHour, yearFirstMinute, yearFirstSecond);
		}

		//需要一个当前日期和位图匹配相等结果，拿到有差异的时间位。

		/// <summary>
		/// 获取当前时间与Cron位图的差异位（优先级：秒 > 分 > 时） 
		/// </summary>
		private static int GetTimeDifferenceBit(DateTime time, CycleCronMap map)
		{
			int secondDiff = (map.Seconds & (1UL << time.Second)) != 0 ? -1 : time.Second;
			int minuteDiff = (map.Minutes & (1UL << time.Minute)) != 0 ? -1 : time.Minute;
			int hourDiff = (map.Hours & (1U << time.Hour)) != 0 ? -1 : time.Hour;
			// 返回优先级最高的差异位（秒 > 分 > 时）
			if (secondDiff != -1) return secondDiff;
			if (minuteDiff != -1) return minuteDiff;
			return hourDiff; // 可能是-1，表示完全匹配
		}

		/// <summary>
		/// 检查日期和月份是否匹配
		/// </summary>
		private static bool IsDateMonthMatch(DateTime time, CycleCronMap map)
		{
			switch (map.Mode)
			{
				case CycleCronMode.DateMonthYear:
					return (map.Dates & (1U << time.Day)) != 0
						&& (map.Months & (1U << time.Month)) != 0;

				case CycleCronMode.WeekMonthYear:
					int weekday = time.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)time.DayOfWeek;
					return (map.Dates & (1U << weekday)) != 0
						&& (map.Months & (1U << time.Month)) != 0;

				case CycleCronMode.Week:
					weekday = time.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)time.DayOfWeek;
					return (map.Dates & (1U << weekday)) != 0;

				case CycleCronMode.Day:
					return true; // Day模式不检查日期和月份

				default:
					return false;
			}
		}

		/// <summary>
		/// 检查月份是否匹配
		/// </summary>
		private static bool IsMonthMatch(int month, CycleCronMap map)
		{
			return (map.Months & (1U << month)) != 0;
		}

	}
}
