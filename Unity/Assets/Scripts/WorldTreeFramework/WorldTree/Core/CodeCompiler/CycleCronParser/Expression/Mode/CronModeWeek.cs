namespace WorldTree
{
	public static partial class CycleCronExpression
	{
		/// <summary>
		/// 尝试解析Week模式 (模式3)
		/// 格式：秒 分 时 星期 星期轮 轮次范围 (6个字段)
		/// </summary>
		private static bool TryParseWeek(string[] parts, CycleCronMap map)
		{
			// 检查字段数量
			if (parts.Length != 6) return false;

			// 解析秒 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!CycleCronExpressionPart.Parse(parts[2], 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析星期字段 (1-7, 支持#/L特殊符号)
			if (!CycleCronExpressionPart.ParseDate(parts[3], false, out map.Dates, out map.DateOffset)) return false;

			// 解析星期轮位图 (1-56)
			if (!CycleCronExpressionPart.Parse(parts[4], 1, 56, out ulong cyclesBitmap)) return false;

			// 解析周期范围 (1-56, * = 56)
			byte cycleRange;
			if (parts[5] == "*")
			{
				cycleRange = 56; // * 表示全范围56
			}
			else
			{
				if (!byte.TryParse(parts[5], out cycleRange)) return false;
				if (cycleRange < 1 || cycleRange > 56) return false;
			}

			// 组装Cycles字段：
			// 位63-56: 周期范围 (8位)
			// 位55-0:  轮次位图 (56位)
			map.Cycles = ((ulong)cycleRange << 56) | cyclesBitmap;

			// 月份字段在Week模式下不使用，设置为全部
			map.Months = 0xFFFF;

			return true;
		}

	}
}
