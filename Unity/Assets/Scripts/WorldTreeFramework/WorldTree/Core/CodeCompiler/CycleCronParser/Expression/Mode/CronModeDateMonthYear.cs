namespace WorldTree
{
	public static partial class CycleCronExpression
	{
		/// <summary>
		/// 尝试解析DateMonthYear模式 (模式1)
		/// 格式：秒 分 时 日期 月份 年轮 轮次范围 (7个字段)
		/// </summary>
		private static bool TryParseDateMonthYear(string[] parts, CycleCronMap map)
		{
			// 检查字段数量
			if (parts.Length != 7) return false;

			// 解析秒 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!CycleCronExpressionPart.Parse(parts[2], 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析日期字段 (1-31, 支持L/W/LW特殊符号)
			if (!CycleCronExpressionPart.ParseDate(parts[3], true, out map.Days, out map.DayOffset)) return false;

			// 解析月 (1-12)
			if (!CycleCronExpressionPart.Parse(parts[4], 1, 12, out ulong months)) return false;
			map.Months = (ushort)months;

			// 解析年轮位图 (1-56)
			if (!CycleCronExpressionPart.Parse(parts[5], 1, 56, out ulong cyclesBitmap)) return false;
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

	}
}
