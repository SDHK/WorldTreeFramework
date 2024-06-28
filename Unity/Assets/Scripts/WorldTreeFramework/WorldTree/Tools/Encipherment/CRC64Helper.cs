/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 16:34

* 描述： long类型CRC64，(正常的CRC64应为ulong类型)
*
*/

using System;

namespace WorldTree
{
	/// <summary>
	/// long类型CRC64
	/// </summary>
	public static class CRC64Helper
	{
		/// <summary>
		/// 多项式 :这是由国际标准化组织(ISO)在1993年指定的64位CRC校验算法。
		/// </summary>
		private const ulong POLYNOMIAL = 0x42F0E1EBA9EA3693;

		/// <summary>
		/// 查找表
		/// </summary>
		private static readonly long[] tables = GenerateCRC64Table();

		/// <summary>
		/// 生成CRC64位查找表
		/// </summary>
		private static long[] GenerateCRC64Table()
		{
			long[] tables = new long[256];
			for (int i = 0; i < 256; i++)
			{
				ulong crc = CalculateCRC64(i);
				byte[] bytes = BitConverter.GetBytes(crc);
				long value = 0;
				for (int j = 0; j < 8; j++)
				{
					value = (value << 8) | bytes[j];//这样操作可以覆盖到符号位
				}
				tables[i] = value;
			}
			return tables;
		}

		/// <summary>
		/// 计算CRC64
		/// </summary>
		private static ulong CalculateCRC64(int i)
		{
			ulong crc = (ulong)i;
			for (int j = 0; j < 8; j++)
			{
				if ((crc & 1) == 1)
				{
					crc = (crc >> 1) ^ POLYNOMIAL;
				}
				else
				{
					crc >>= 1;
				}
			}
			return crc;
		}

		/// <summary>
		/// 转换为CRC64
		/// </summary>
		public static long GetCRC64(this string str)
		{
			long crc = 0;
			foreach (char c in str)
			{
				crc = tables[(crc ^ c) & 0xFF] ^ (crc >> 8);
			}
			return crc;
		}

		/// <summary>
		/// 验证
		/// </summary>
		public static bool VerifyCRC64(string originalData, long crc) => GetCRC64(originalData) == crc;
	}
}