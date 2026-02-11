using System;

namespace WorldTree
{
	public static partial class CycleCronMatcher
	{
		/// <summary>
		/// 匹配 Day 模式 (模式4)
		/// 格式：秒 分 时 天数轮 轮次范围
		/// </summary>
		private static bool MatchDay(DateTime time, CycleCronMap map)
		{
			// Day模式只需要匹配时间（秒、分、时）
			// 已经在MatchTime中完成

			// TODO: 匹配天数轮和轮次
			// 需要基准时间来计算是第几天
			return true;
		}
	}
}
