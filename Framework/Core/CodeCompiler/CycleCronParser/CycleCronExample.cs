using System;

namespace WorldTree
{
	/// <summary>
	/// CycleCron 使用示例
	/// </summary>
	public static class CycleCronExample
	{
		/// <summary>
		/// 示例1：每天上午9点执行
		/// </summary>
		public static void Example1_DailyAt9AM()
		{
			// 创建Map并设置模式
			var map = new CycleCronMap { Mode = CycleCronMode.DateMonthYear };

			// 解析表达式（简写形式）
			if (CycleCronExpression.TryParse("0 0 9", map))
			{
				// 检查当前时间是否匹配
				DateTime now = DateTime.Now;
				//bool isMatch = CycleCronMatcher.IsMatch(now, map);
				
				//Console.WriteLine($"当前时间 {now} 是否匹配: {isMatch}");

				// 获取下一个触发时间
				DateTime? next = CycleCronMatcher.GetNext(now, map);
				if (next.HasValue)
				{
					Console.WriteLine($"下次触发时间: {next.Value}");
				}
			}
		}

		/// <summary>
		/// 示例2：每月1号和15号9点执行
		/// </summary>
		public static void Example2_TwiceMonthly()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.DateMonthYear };

			// 解析完整表达式
			if (CycleCronExpression.TryParse("0 0 9 1,15", map))
			{
				DateTime now = DateTime.Now;
				
				// 检查匹配
				//if (CycleCronMatcher.IsMatch(now, map))
				//{
				//	Console.WriteLine("现在是1号或15号的9点！");
				//}

				// 获取未来10次触发时间
				DateTime current = now;
				for (int i = 0; i < 10; i++)
				{
					DateTime? next = CycleCronMatcher.GetNext(current, map);
					if (next.HasValue)
					{
						Console.WriteLine($"第{i + 1}次触发: {next.Value:yyyy-MM-dd HH:mm:ss}");
						current = next.Value;
					}
					else
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// 示例3：每小时执行
		/// </summary>
		public static void Example3_Hourly()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.DateMonthYear };

			// 极简形式
			if (CycleCronExpression.TryParse("0 0", map))
			{
				DateTime now = DateTime.Now;
				//bool isMatch = CycleCronMatcher.IsMatch(now, map);
				
				//Console.WriteLine($"每小时触发，当前是否匹配: {isMatch}");
			}
		}

		/// <summary>
		/// 示例4：月末最后一天
		/// </summary>
		public static void Example4_LastDayOfMonth()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.DateMonthYear };

			// 使用L特殊符号
			if (CycleCronExpression.TryParse("0 0 9 L", map))
			{
				DateTime now = DateTime.Now;
				//bool isMatch = CycleCronMatcher.IsMatch(now, map);
				
				//Console.WriteLine($"月末触发，当前是否匹配: {isMatch}");
			}
		}

		/// <summary>
		/// 示例5：每周一到周五（工作日）
		/// </summary>
		public static void Example5_Weekdays()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.WeekMonthYear };

			// 星期模式：1-5表示周一到周五
			if (CycleCronExpression.TryParse("0 0 9 1-5", map))
			{
				DateTime now = DateTime.Now;
				//bool isMatch = CycleCronMatcher.IsMatch(now, map);
				
				//Console.WriteLine($"工作日9点触发，当前是否匹配: {isMatch}");
			}
		}

		/// <summary>
		/// 示例6：Day模式 - 每7天
		/// </summary>
		public static void Example6_Every7Days()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.Day };

			// Day模式：每7天12点
			if (CycleCronExpression.TryParse("0 0 12 7 28", map))
			{
				DateTime now = DateTime.Now;
				//bool isMatch = CycleCronMatcher.IsMatch(now, map);
				
				//Console.WriteLine($"每7天触发，当前是否匹配: {isMatch}");
				// 注意：完整的Day模式匹配需要基准时间（开服时间）
			}
		}

		/// <summary>
		/// 示例7：性能测试
		/// </summary>
		public static void Example7_Performance()
		{
			var map = new CycleCronMap { Mode = CycleCronMode.DateMonthYear };
			CycleCronExpression.TryParse("0 0 9 1,15", map);

			// 测试100万次匹配
			DateTime testTime = new DateTime(2024, 1, 15, 9, 0, 0);
			
			var sw = System.Diagnostics.Stopwatch.StartNew();
			int matchCount = 0;
			
			for (int i = 0; i < 1000000; i++)
			{
				//if (CycleCronMatcher.IsMatch(testTime, map))
				//{
				//	matchCount++;
				//}
			}
			
			sw.Stop();
			
			Console.WriteLine($"100万次匹配耗时: {sw.ElapsedMilliseconds}ms");
			Console.WriteLine($"平均每次: {sw.Elapsed.TotalMilliseconds / 1000000 * 1000000}ns");
			Console.WriteLine($"匹配次数: {matchCount}");
			// 预期：~20-50ms (每次20-50ns)
		}
	}
}
