using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// Cron时间Tick解析器
	/// </summary>
	public static class CronTickResolver
	{
		/// <summary>
		/// 根据Cron位图计算下一次执行时间的Tick值
		/// </summary>
		/// <param name="bitmap">Cron位图</param>
		/// <param name="fromTick">起始时间Tick (DateTime.Ticks)</param>
		/// <returns>下一次执行时间的Tick值，如果无法计算则返回-1</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetNextTick(ref CronBitmap bitmap, long fromTick)
		{
			DateTime fromTime = new DateTime(fromTick);
			DateTime? nextTime = GetNextDateTime(ref bitmap, fromTime);
			return nextTime?.Ticks ?? -1;
		}

		/// <summary>
		/// 根据Cron位图计算下一次执行时间
		/// </summary>
		/// <param name="bitmap">Cron位图</param>
		/// <param name="fromTime">起始时间</param>
		/// <returns>下一次执行时间，如果无法计算则返回null</returns>
		public static DateTime? GetNextDateTime(ref CronBitmap bitmap, DateTime fromTime)
		{
			// 从下一秒开始计算
			DateTime current = fromTime.AddSeconds(1);
			// 重置为整秒
			current = new DateTime(current.Year, current.Month, current.Day,
				current.Hour, current.Minute, current.Second, DateTimeKind.Unspecified);

			// 最多尝试4年（避免无限循环）
			DateTime maxTime = current.AddYears(4);

			while (current < maxTime)
			{
				// 检查月份
				if (!CronBitmapHelper.IsBitSet(bitmap.Months, current.Month))
				{
					// 跳到下个月的1号0时0分0秒
					current = GetNextMonth(ref bitmap, current);
					if (current >= maxTime) break;
					continue;
				}

				// 检查日期和星期（任一匹配即可）
				bool dayMatch = CronBitmapHelper.IsBitSet(bitmap.Days, current.Day);
				bool weekdayMatch = CronBitmapHelper.IsBitSet(bitmap.Weekdays, (int)current.DayOfWeek);

				if (!dayMatch && !weekdayMatch)
				{
					// 跳到下一天的0时0分0秒
					current = current.AddDays(1);
					current = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0);
					continue;
				}

				// 检查小时
				if (!CronBitmapHelper.IsBitSet(bitmap.Hours, current.Hour))
				{
					int nextHour = CronBitmapHelper.GetNextBitIndex(bitmap.Hours, current.Hour, 0, 23);
					if (nextHour > current.Hour)
					{
						current = new DateTime(current.Year, current.Month, current.Day, nextHour, 0, 0);
					}
					else
					{
						// 跳到下一天
						current = current.AddDays(1);
						nextHour = CronBitmapHelper.GetFirstBitIndex(bitmap.Hours, 0, 23);
						if (nextHour == -1) return null;
						current = new DateTime(current.Year, current.Month, current.Day, nextHour, 0, 0);
					}
					continue;
				}

				// 检查分钟
				if (!CronBitmapHelper.IsBitSet(bitmap.Minutes, current.Minute))
				{
					int nextMinute = CronBitmapHelper.GetNextBitIndex(bitmap.Minutes, current.Minute, 0, 59);
					if (nextMinute > current.Minute)
					{
						current = new DateTime(current.Year, current.Month, current.Day,
							current.Hour, nextMinute, 0);
					}
					else
					{
						// 跳到下一小时
						current = current.AddHours(1);
						current = new DateTime(current.Year, current.Month, current.Day,
							current.Hour, 0, 0);
					}
					continue;
				}

				// 检查秒
				if (!CronBitmapHelper.IsBitSet(bitmap.Seconds, current.Second))
				{
					int nextSecond = CronBitmapHelper.GetNextBitIndex(bitmap.Seconds, current.Second, 0, 59);
					if (nextSecond > current.Second)
					{
						current = new DateTime(current.Year, current.Month, current.Day,
							current.Hour, current.Minute, nextSecond);
					}
					else
					{
						// 跳到下一分钟
						current = current.AddMinutes(1);
						current = new DateTime(current.Year, current.Month, current.Day,
							current.Hour, current.Minute, 0);
					}
					continue;
				}

				// 所有条件都满足
				return current;
			}

			return null;
		}

		/// <summary>
		/// 获取下一个匹配的月份时间
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateTime GetNextMonth(ref CronBitmap bitmap, DateTime current)
		{
			int nextMonth = CronBitmapHelper.GetNextBitIndex(bitmap.Months, current.Month, 1, 12);

			if (nextMonth > current.Month)
			{
				// 还在当年
				int day = CronBitmapHelper.GetFirstBitIndex(bitmap.Days, 1, 31);
				if (day == -1) day = 1;
				return new DateTime(current.Year, nextMonth, Math.Min(day, DateTime.DaysInMonth(current.Year, nextMonth)), 0, 0, 0);
			}
			else if (nextMonth != -1)
			{
				// 跳到下一年
				int day = CronBitmapHelper.GetFirstBitIndex(bitmap.Days, 1, 31);
				if (day == -1) day = 1;
				return new DateTime(current.Year + 1, nextMonth, Math.Min(day, DateTime.DaysInMonth(current.Year + 1, nextMonth)), 0, 0, 0);
			}

			// 没有找到匹配的月份
			return current.AddYears(4);
		}

		/// <summary>
		/// 获取多个下一次执行时间
		/// </summary>
		/// <param name="bitmap">Cron位图</param>
		/// <param name="fromTime">起始时间</param>
		/// <param name="count">获取数量</param>
		/// <returns>执行时间数组</returns>
		public static DateTime[] GetNextDateTimes(ref CronBitmap bitmap, DateTime fromTime, int count)
		{
			if (count <= 0) return Array.Empty<DateTime>();

			DateTime[] results = new DateTime[count];
			DateTime current = fromTime;

			for (int i = 0; i < count; i++)
			{
				DateTime? next = GetNextDateTime(ref bitmap, current);
				if (!next.HasValue)
					break;

				results[i] = next.Value;
				current = next.Value;
			}

			return results;
		}

		/// <summary>
		/// 获取多个下一次执行时间的Tick值
		/// </summary>
		/// <param name="bitmap">Cron位图</param>
		/// <param name="fromTick">起始时间Tick</param>
		/// <param name="count">获取数量</param>
		/// <returns>执行时间Tick数组</returns>
		public static long[] GetNextTicks(ref CronBitmap bitmap, long fromTick, int count)
		{
			DateTime fromTime = new DateTime(fromTick);
			DateTime[] dateTimes = GetNextDateTimes(ref bitmap, fromTime, count);

			long[] ticks = new long[dateTimes.Length];
			for (int i = 0; i < dateTimes.Length; i++)
			{
				ticks[i] = dateTimes[i].Ticks;
			}

			return ticks;
		}

		/// <summary>
		/// 判断指定时间是否匹配Cron表达式
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMatch(ref CronBitmap bitmap, DateTime time)
		{
			return CronBitmapHelper.IsBitSet(bitmap.Seconds, time.Second)
				&& CronBitmapHelper.IsBitSet(bitmap.Minutes, time.Minute)
				&& CronBitmapHelper.IsBitSet(bitmap.Hours, time.Hour)
				&& (CronBitmapHelper.IsBitSet(bitmap.Days, time.Day) || CronBitmapHelper.IsBitSet(bitmap.Weekdays, (int)time.DayOfWeek))
				&& CronBitmapHelper.IsBitSet(bitmap.Months, time.Month);
		}

		/// <summary>
		/// 判断指定时间Tick是否匹配Cron表达式
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMatch(ref CronBitmap bitmap, long tick)
		{
			DateTime time = new DateTime(tick);
			return IsMatch(ref bitmap, time);
		}
	}
}
