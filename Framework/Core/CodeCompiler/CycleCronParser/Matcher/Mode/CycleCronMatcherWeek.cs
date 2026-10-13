using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 匹配 Week 模式 (模式3)
		/// 格式：秒 分 时 星期 星期轮 轮次范围
		/// </summary>
		private static bool MatchWeek(DateTime time, CycleCronMap map)
		{
			// 匹配星期
			int weekday = time.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)time.DayOfWeek;
			if ((map.Dates & (1U << weekday)) == 0)
				return false;

			// TODO: 匹配星期轮和轮次
			// 需要基准时间来计算是第几周
			return true;
		}
	}
}
