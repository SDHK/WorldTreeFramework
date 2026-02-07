using System;

namespace WorldTree
{

	/// <summary>
	/// 周期Cron表达式解析器 
	/// </summary>
	public class CycleCronParser
	{
		/// <summary>
		/// 尝试解析周期Cron表达式，成功返回true，并输出CronBitmap；失败返回false，输出默认CronBitmap 
		/// </summary>
		public static bool TryParse(string cronExpression, CycleCronMap map, CycleCronMode mode = CycleCronMode.DateMonthYear)
		{
			// 如果是空字符串或仅包含空白字符，则解析失败
			if (string.IsNullOrWhiteSpace(cronExpression)) return false;
			// 分割Cron表达式为6个部分,每个部分之间用空格分割。RemoveEmptyEntries是要去掉空字符串。
			string[] parts = cronExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 3) return false;

			// 解析秒 (0-59)
			if (!ParseField(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!ParseField(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!ParseField(parts[2], 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			bool bit = false;
			switch (mode)
			{
				case CycleCronMode.DateMonthYear:
					bit = TryParseDateMonthYear(parts, map);
					break;
				case CycleCronMode.WeekMonthYear:
					break;
				case CycleCronMode.Week:
					break;
				case CycleCronMode.Day:
					break;
				default:
					break;
			}
			return bit;
		}

		/// <summary>
		/// 尝试解析DateMonthYear模式 (模式1)
		/// 格式：秒 分 时 日期 月份 年轮 轮次范围 (7个字段)
		/// </summary>
		public static bool TryParseDateMonthYear(string[] parts, CycleCronMap map)
		{
			// 检查字段数量
			if (parts.Length != 7) return false;

			// 解析秒 (0-59)
			if (!ParseField(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!ParseField(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!ParseField(parts[2], 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析日期字段 (1-31, 支持L/W/LW特殊符号)
			if (!ParseFieldDay(parts[3], true, out map.Days, out map.DayOffset)) return false;

			// 解析月 (1-12)
			if (!ParseField(parts[4], 1, 12, out ulong months)) return false;
			map.Months = (ushort)months;

			// 解析年轮位图 (1-56)
			if (!ParseField(parts[5], 1, 56, out ulong cyclesBitmap)) return false;

			// 解析周期范围 (1-56, * = 56)
			byte cycleRange;
			if (parts[6] == "*")
			{
				cycleRange = 56; // * 表示全范围56
			}
			else
			{
				if (!byte.TryParse(parts[6], out cycleRange)) return false;
				if (cycleRange < 1 || cycleRange > 56) return false;
			}

			// 组装Cycles字段：
			// 位63-56: 周期范围 (8位)
			// 位55-0:  轮次位图 (56位)
			map.Cycles = ((ulong)cycleRange << 56) | cyclesBitmap;

			return true;
		}

		/// <summary>
		/// 解析日期/星期字段（支持L/W/#特殊符号）
		/// 特殊符号只能出现一次，可以和普通值组合
		/// 例如："1,15,L" ✅  "L,15,W" ❌（两个特殊符号）
		/// </summary>
		/// <param name="field">字段字符串（如 "15W", "1,15,L", "5#3"）</param>
		/// <param name="isDate">true=日期模式(1-31), false=星期模式(1-7)</param>
		/// <param name="daysBitmap">输出：日期/星期位图</param>
		/// <param name="dayOffset">输出：特殊符号编码</param>
		/// <returns>解析是否成功</returns>
		private static bool ParseFieldDay(string field, bool isDate, out uint daysBitmap, out byte dayOffset)
		{
			daysBitmap = 0;
			dayOffset = 0;
			if (string.IsNullOrWhiteSpace(field)) return false;

			int min = 1;
			int max = isDate ? 31 : 7;
			bool hasSpecial = false; // 标记是否已经处理过特殊符号
			ulong bitmap = 0;

			// 处理逗号分隔的多个值
			string[] segments = field.Split(',');
			foreach (string segment in segments)
			{
				if (string.IsNullOrWhiteSpace(segment)) continue;

				// 检查当前segment是否包含特殊符号
				bool isSpecial = segment.Contains("L") || segment.Contains("W") || segment.Contains("#");

				if (isSpecial)
				{
					// 如果已经有特殊符号了，返回false（只允许一个特殊符号）
					if (hasSpecial) return false;

					// 解析特殊符号（只设置 dayOffset，不设置位图）
					byte specialOffset;

					bool success = isDate
						? ParseDateSpecial(segment, out specialOffset)
						: ParseWeekSpecial(segment, out specialOffset);

					if (!success) return false;

					dayOffset = specialOffset;
					hasSpecial = true;
				}
				else
				{
					// 普通值，复用 ParseFieldSegment
					if (!ParseFieldSegment(segment, min, max, ref bitmap)) return false;
				}
			}

			daysBitmap = (uint)bitmap;
			return true;
		}


		/// <summary>
		/// 解析日期特殊符号（L/W/LW）
		/// 注意：特殊符号不设置位图，只设置 dayOffset 编码
		/// </summary>
		private static bool ParseDateSpecial(string field, out byte dayOffset)
		{
			dayOffset = 0;

			// 处理 "LW" - 月末最后一个工作日
			if (field == "LW" || field == "WL")
			{
				dayOffset = 0x60; // bit7-5 = 011 (LW)
				return true;
			}

			// 处理 "L" 或 "L-N" - 月末倒数
			if (field.StartsWith("L"))
			{
				int offset = 0;
				if (field.Length > 1)
				{
					// "L-1", "L-2" 格式
					if (field[1] == '-')
					{
						if (!int.TryParse(field.Substring(2), out offset)) return false;
						if (offset < 0 || offset > 31) return false;
					}
					else
					{
						return false; // 格式错误
					}
				}
				dayOffset = (byte)(0x20 | offset); // bit7-5 = 001 (L), bit4-0 = offset
				return true;
			}

			// 处理 "NW" - N号最近的工作日
			if (field.EndsWith("W"))
			{
				string dayStr = field.Substring(0, field.Length - 1);
				if (!int.TryParse(dayStr, out int day)) return false;
				if (day < 1 || day > 31) return false;
				dayOffset = (byte)(0x40 | day); // bit7-5 = 010 (W), bit4-0 = day
				return true;
			}

			return false;
		}

		/// <summary>
		/// 解析星期特殊符号（L/#）
		/// 注意：特殊符号不设置位图，只设置 dayOffset 编码
		/// </summary>
		private static bool ParseWeekSpecial(string field, out byte dayOffset)
		{
			dayOffset = 0;

			// 处理 "N#M" - 第M个星期N
			if (field.Contains("#"))
			{
				string[] parts = field.Split('#');
				if (parts.Length != 2) return false;
				if (!int.TryParse(parts[0], out int weekday)) return false;
				if (!int.TryParse(parts[1], out int nth)) return false;
				if (weekday < 1 || weekday > 7) return false;
				// nth必须是1-5，表示第1-5个星期N（第5个表示最后一个星期N）
				if (nth < 1 || nth > 5) return false;

				// bit7=0, bit6=1(#), bit5-3=weekday, bit2-0=nth
				dayOffset = (byte)(0x40 | (weekday << 3) | nth);
				return true;
			}

			// 处理 "NL" - 最后一个星期N
			if (field.EndsWith("L"))
			{
				string weekdayStr = field.Substring(0, field.Length - 1);
				if (!int.TryParse(weekdayStr, out int weekday)) return false;
				if (weekday < 1 || weekday > 7) return false;

				// bit7=1(L), bit6=0, bit5-3=weekday, bit2-0=0
				dayOffset = (byte)(0x80 | (weekday << 3));
				return true;
			}

			return false;
		}

		/// <summary>
		/// 解析单个字段为位图
		/// </summary>
		private static bool ParseField(string field, int min, int max, out ulong bitmap)
		{
			bitmap = 0;

			if (string.IsNullOrWhiteSpace(field)) return false;

			// 处理 * 通配符，直接设置所有位
			if (field == "*")
			{
				bitmap = ~0UL;
				return true;
			}

			// 处理逗号分隔的多个值，复用 ParseFieldSegment
			string[] segments = field.Split(',');
			foreach (string segment in segments)
			{
				if (string.IsNullOrWhiteSpace(segment)) continue;
				if (!ParseFieldSegment(segment, min, max, ref bitmap)) return false;
			}
			return bitmap != 0;
		}


		/// <summary>
		/// 解析单个segment为位图（处理范围、步长、单值）
		/// 这是核心的位图解析逻辑，被 ParseField 和 ParseFieldDay 复用
		/// </summary>
		/// <param name="segment">单个segment字符串（不含逗号）</param>
		/// <param name="min">最小值</param>
		/// <param name="max">最大值</param>
		/// <param name="bitmap">位图引用（会累加结果）</param>
		/// <returns>解析是否成功</returns>
		private static bool ParseFieldSegment(string segment, int min, int max, ref ulong bitmap)
		{
			// 处理范围 (例如: 1-5)
			if (segment.Contains("-"))
			{
				string[] ranges = segment.Split('-');
				// 范围必须是两个值,否则返回false
				if (ranges.Length != 2) return false;
				// 解析范围的起始和结束值
				if (!int.TryParse(ranges[0], out int start) || !int.TryParse(ranges[1], out int end))
					return false;
				// 检查范围是否合法
				if (start < min || end > max || start > end) return false;
				// 设置范围内的位
				for (int i = start; i <= end; i++) bitmap |= 1UL << i;
			}
			// 处理步长 (例如: */5 或 0-30/5)
			else if (segment.Contains("/"))
			{
				string[] steps = segment.Split('/');
				// 步长必须是两个值,否则返回false
				if (steps.Length != 2) return false;
				// 拿到步长值
				if (!int.TryParse(steps[1], out int stepValue)) return false;

				//先将范围设置为全部，也就是 *
				int start = min;
				int end = max;

				// 如果不是*，需要解析范围
				if (steps[0] != "*")
				{
					// 解析范围
					if (steps[0].Contains("-"))
					{
						// 解析范围的起始和结束值
						string[] ranges = steps[0].Split('-');
						// 范围必须是两个值,否则返回false
						if (ranges.Length != 2) return false;
						// 重新设置start和end。 例：10-30/5 表示从10到30每5个取一次
						if (!int.TryParse(ranges[0], out start) || !int.TryParse(ranges[1], out end)) return false;
					}
					else
					{
						// 单个起始值，重新设置start。例：10/5 表示从10开始每5个取一次，其实就是10-Max/5
						if (!int.TryParse(steps[0], out start)) return false;
					}
				}

				// 从min到max，检查范围是否合法
				if (start < min || end > max || start > end || stepValue <= 0) return false;
				// 设置步长位
				for (int i = start; i <= end; i += stepValue) bitmap |= 1UL << i;
			}
			else // 处理单个值,例如: 5
			{
				if (!int.TryParse(segment, out int value)) return false;
				if (value < min || value > max) return false;
				bitmap |= 1UL << value; // 直接移动到指定位置
			}
			return true;
		}
	}
}
