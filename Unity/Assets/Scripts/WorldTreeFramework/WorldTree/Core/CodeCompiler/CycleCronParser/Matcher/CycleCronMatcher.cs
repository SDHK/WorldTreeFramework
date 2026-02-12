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
		public static int IsMatch(DateTime time, CycleCronMap map)
		{
			switch (map.Mode)
			{
				case CycleCronMode.DateMonthYear:
					return MatchDateMonthYear(time, time, map);
				case CycleCronMode.WeekMonthYear:
					return MatchWeekMonthYear(time, time, map);
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

	

	

	}
}
