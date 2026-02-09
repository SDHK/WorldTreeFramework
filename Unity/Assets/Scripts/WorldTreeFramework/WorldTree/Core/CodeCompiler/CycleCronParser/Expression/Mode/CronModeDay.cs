namespace WorldTree
{
	public static partial class CycleCronExpression
	{
		/// <summary>
		/// 尝试解析Day模式 (模式4)
		/// 格式：秒 分 时 天数轮 轮次范围 (5个字段)
		/// </summary>
		private static bool TryParseDay(string[] parts, CycleCronMap map)
		{
			// 检查字段数量
			if (parts.Length != 5) return false;

			// 解析秒 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!CycleCronExpressionPart.Parse(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!CycleCronExpressionPart.Parse(parts[2], 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析天数轮位图 (1-56)
			if (!CycleCronExpressionPart.Parse(parts[3], 1, 56, out ulong cyclesBitmap)) return false;

			// 解析周期范围 (1-56, * = 56)
			byte cycleRange;
			if (parts[4] == "*")
			{
				cycleRange = 56; // * 表示全范围56
			}
			else
			{
				if (!byte.TryParse(parts[4], out cycleRange)) return false;
				if (cycleRange < 1 || cycleRange > 56) return false;
			}

			// 组装Cycles字段：
			// 位63-56: 周期范围 (8位)
			// 位55-0:  轮次位图 (56位)
			map.Cycles = ((ulong)cycleRange << 56) | cyclesBitmap;

			// Day模式下不使用日期、星期、月份字段，设置为全部
			map.Days = 0xFFFFFFFF;
			map.Months = 0xFFFF;
			map.DayOffset = 0;

			return true;
		}

	}
}
