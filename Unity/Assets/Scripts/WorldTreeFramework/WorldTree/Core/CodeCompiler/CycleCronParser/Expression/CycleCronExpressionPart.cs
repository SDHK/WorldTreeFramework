namespace WorldTree
{
	/// <summary>
	/// 周期Cron表达式分段解析
	/// </summary>
	public static class CycleCronExpressionPart
	{
		/// <summary>
		/// 解析单个字段为位图
		/// </summary>
		public static bool Parse(string field, int min, int max, out ulong bitmap)
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
				if (!ParseSegment(segment, min, max, ref bitmap)) return false;
			}
			return bitmap != 0;
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
		public static bool ParseDate(string field, bool isDate, out uint daysBitmap, out byte dayOffset)
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
					if (!ParseSegment(segment, min, max, ref bitmap)) return false;
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
				dayOffset = 3 << 6; // bit7-6 = 11 (LW)
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
				// bit6=1(L), bit5-0 = offset
				dayOffset = (byte)(1 << 6 | offset);
				return true;
			}

			// 处理 "NW" - N号最近的工作日
			if (field.EndsWith("W"))
			{
				string dayStr = field.Substring(0, field.Length - 1);
				if (!int.TryParse(dayStr, out int day)) return false;
				if (day < 1 || day > 31) return false;
				// bit7=1(W), bit5-0 = day
				dayOffset = (byte)(2 << 6 | day);
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
				// bit7=1(#), bit6=0, bit5-3=nth, bit2-0=weekday
				dayOffset = (byte)(2 << 6 | (nth << 3) | weekday);
				return true;
			}

			// 处理 "NL" - 最后一个星期N
			if (field.EndsWith("L"))
			{
				string weekdayStr = field.Substring(0, field.Length - 1);
				if (!int.TryParse(weekdayStr, out int weekday)) return false;
				if (weekday < 1 || weekday > 7) return false;
				// bit7=0, bit6=1(L), bit5-3=0(未用), bit2-0=weekday
				dayOffset = (byte)(1 << 6 | (weekday));
				return true;
			}

			return false;
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
		private static bool ParseSegment(string segment, int min, int max, ref ulong bitmap)
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
