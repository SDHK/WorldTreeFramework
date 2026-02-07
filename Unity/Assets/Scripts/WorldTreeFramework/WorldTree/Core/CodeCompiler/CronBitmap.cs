using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	// Cron表达式位图数据结构
	// ,是分隔符，-是范围，/是步长，*是通配符


	/// <summary>
	/// Cron位图数据 27
	/// </summary>
	public struct CronBitmap
	{
		/// <summary>
		/// 秒位图 (0-59) - 使用64位可以表示60个值
		/// </summary>
		public long Seconds; //8

		/// <summary>
		/// 分钟位图 (0-59)
		/// </summary>
		public long Minutes; //8

		/// <summary>
		/// 小时位图 (0-23)
		/// </summary>
		public int Hours; //4

		/// <summary>
		/// 天位图 (1-31)
		/// </summary>
		public int Days; //4

		/// <summary>
		/// 月位图 (1-12)
		/// </summary>
		public short Months; //2

		/// <summary>
		/// 星期位图 (0-6, 0=星期日)
		/// </summary>
		public byte Weekdays; //1

		/// <summary>
		/// 倒数每月天数偏移
		/// </summary>
		public byte LastMonthOffset;

		/// <summary>
		/// 倒数每月星期数偏移 
		/// </summary>
		public byte LastWeekOffset;
	}

	/// <summary>
	/// Cron位图工具类
	/// </summary>
	public static class CronBitmapHelper
	{
		/// <summary>
		/// 解析Cron表达式为位图数据
		/// 格式: 秒 分 时 日 月 周
		/// 支持: * , - /
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryParse(string cronExpression, out CronBitmap map)
		{
			map = default;

			if (string.IsNullOrWhiteSpace(cronExpression))
				return false;
			// 分割Cron表达式为6个部分,每个部分之间用空格分割。RemoveEmptyEntries是要去掉空字符串。
			string[] parts = cronExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			// 如果分割后的部分不是6个，则返回false。
			if (parts.Length != 6) return false;

			// 解析秒 (0-59)
			if (!ParseField(parts[0], 0, 59, out map.Seconds)) return false;

			// 解析分 (0-59)
			if (!ParseField(parts[1], 0, 59, out map.Minutes)) return false;

			// 解析时 (0-23)
			if (!ParseField(parts[2], 0, 23, out long hours)) return false;
			map.Hours = (int)hours;

			// 解析日 (1-31)
			if (!ParseField(parts[3], 1, 31, out long days)) return false;
			map.Days = (int)days;

			// 解析月 (1-12)
			if (!ParseField(parts[4], 1, 12, out long months)) return false;
			map.Months = (short)months;

			// 解析周 (0-6)
			if (!ParseField(parts[5], 0, 6, out long weekdays)) return false;
			map.Weekdays = (byte)weekdays;

			return true;
		}

		/// <summary>
		/// 解析单个字段为位图
		/// </summary>
		private static bool ParseField(string field, int min, int max, out long bitmap)
		{
			bitmap = 0;

			if (string.IsNullOrWhiteSpace(field))
				return false;

			// 处理 * 通配符，直接设置所有位
			if (field == "*")
			{
				bitmap = ~0;
				return true;
			}

			// 处理逗号分隔的多个值
			string[] segments = field.Split(',');
			foreach (string segment in segments)
			{
				if (string.IsNullOrWhiteSpace(segment)) continue;
				// 处理范围 (例如: 1-5)
				if (segment.Contains("-"))
				{
					string[] ranges = segment.Split('-');
					// 范围必须是两个值,否则返回false
					if (ranges.Length != 2) return false;
					// 解析范围的起始和结束值
					if (!int.TryParse(ranges[0], out int start) || !int.TryParse(ranges[1], out int end)) return false;
					// 检查范围是否合法
					if (start < min || end > max || start > end) return false;
					// 设置范围内的位
					for (int i = start; i <= end; i++) bitmap |= 1L << i;
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
					for (int i = start; i <= end; i += stepValue) bitmap |= 1L << i;
				}
				else // 处理单个值,例如: 5
				{
					if (!int.TryParse(segment, out int value)) return false;
					if (value < min || value > max) return false;
					bitmap |= 1L << value; // 直接移动到指定位置
				}
			}

			return bitmap != 0;
		}

		/// <summary>
		/// 检查指定位是否被设置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBitSet(long bitmap, int index)
		{
			return (bitmap & (1L << index)) != 0;
		}

		/// <summary>
		/// 检查指定位是否被设置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBitSet(int bitmap, int index)
		{
			return (bitmap & (1 << index)) != 0;
		}

		/// <summary>
		/// 检查指定位是否被设置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBitSet(short bitmap, int index)
		{
			return (bitmap & (1 << index)) != 0;
		}

		/// <summary>
		/// 检查指定位是否被设置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBitSet(byte bitmap, int index)
		{
			return (bitmap & (1 << index)) != 0;
		}

		/// <summary>
		/// 获取位图中下一个被设置的位索引
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetNextBitIndex(long bitmap, int currentIndex, int min, int max)
		{
			// 从 currentIndex+1 开始查找
			for (int i = currentIndex + 1; i <= max; i++)
			{
				if (IsBitSet(bitmap, i))
					return i;
			}

			// 如果没找到，从最小值开始循环查找
			if (currentIndex >= min)
			{
				for (int i = min; i <= currentIndex; i++)
				{
					if (IsBitSet(bitmap, i))
						return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// 获取位图中下一个被设置的位索引 (int版本)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetNextBitIndex(int bitmap, int currentIndex, int min, int max)
		{
			for (int i = currentIndex + 1; i <= max; i++)
			{
				if (IsBitSet(bitmap, i))
					return i;
			}

			if (currentIndex >= min)
			{
				for (int i = min; i <= currentIndex; i++)
				{
					if (IsBitSet(bitmap, i))
						return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// 获取位图中第一个被设置的位索引
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetFirstBitIndex(long bitmap, int min, int max)
		{
			for (int i = min; i <= max; i++)
			{
				if (IsBitSet(bitmap, i))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// 获取位图中第一个被设置的位索引 (int版本)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetFirstBitIndex(int bitmap, int min, int max)
		{
			for (int i = min; i <= max; i++)
			{
				if (IsBitSet(bitmap, i))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// 创建一个包含所有位的位图 (用于*通配符)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long CreateFullBitmap(int min, int max)
		{
			long bitmap = 0;
			for (int i = min; i <= max; i++)
			{
				bitmap |= 1L << i;
			}
			return bitmap;
		}
	}
}
