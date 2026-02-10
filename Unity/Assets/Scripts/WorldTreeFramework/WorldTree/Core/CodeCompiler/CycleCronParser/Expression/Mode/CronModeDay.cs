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
			// 解析秒 (0-59)
			if (!CycleCronExpressionPart.Parse(parts, 0, 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!CycleCronExpressionPart.Parse(parts, 1, 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!CycleCronExpressionPart.Parse(parts, 2, 0, 23, out ulong hours)) return false;
			map.Hours = (uint)hours;

			// 解析天数轮位图 (1-56)
			if (!CycleCronExpressionPart.ParseCycle(parts, 3, out map.Cycles)) return false;
			return true;
		}

	}
}
