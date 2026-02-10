namespace WorldTree
{
	public static partial class CycleCronExpression
	{
		/// <summary>
		/// 尝试解析WeekMonthYear模式 (模式2)
		/// 格式：秒 分 时 星期 月份 年轮 轮次范围 (7个字段)
		/// </summary>
		private static bool TryParseWeekMonthYear(string[] parts, CycleCronMap map)
		{
			// 解析秒 (0-59)
			if (!CycleCronExpressionPart.Parse(parts, 0, 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!CycleCronExpressionPart.Parse(parts, 1, 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!CycleCronExpressionPart.Parse(parts, 2, 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析星期字段 (1-7, 支持#/L特殊符号)
			if (!CycleCronExpressionPart.ParseDate(parts, 3, false, out map.Dates, out map.DateOffset)) return false;

			// 解析月 (1-12)
			if (!CycleCronExpressionPart.Parse(parts, 4, 1, 12, out ulong months)) return false;
			map.Months = (ushort)months;

			// 解析年轮位图 (1-56)
			if (!CycleCronExpressionPart.ParseCycle(parts,5, out map.Cycles)) return false;
			return true;
		}

	}
}
