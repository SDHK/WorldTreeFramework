
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Numerics;
#endif

namespace WorldTree
{
	/// <summary>
	/// 数学位操作工具类 
	/// </summary>
	public static class MathBit
	{
		/// <summary>
		/// 获取最高位1的位置（-1~63），全0的话返回-1
		/// </summary>
		public static int GetHighestBitIndex(long value)
		{
			if (value == 0) return -1;
			return 63 - LeadingZeroCount((ulong)value);
		}

		/// <summary>
		/// 获取最低位1的位置（-1~63），全0的话返回-1
		/// </summary>
		public static int GetLowestBitIndex(long value)
		{
			if (value == 0) return -1;
			return TrailingZeroCount((ulong)value);
		}

		/// <summary>
		/// 计算前导零的个数（0~64）
		/// </summary>
		public static int LeadingZeroCount(ulong value)
		{
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            return BitOperations.LeadingZeroCount(value);
#else
			if (value == 0) return 64;

			int count = 0;
			if ((value & 0xFFFFFFFF00000000UL) == 0) { count += 32; value <<= 32; }
			if ((value & 0xFFFF000000000000UL) == 0) { count += 16; value <<= 16; }
			if ((value & 0xFF00000000000000UL) == 0) { count += 8; value <<= 8; }
			if ((value & 0xF000000000000000UL) == 0) { count += 4; value <<= 4; }
			if ((value & 0xC000000000000000UL) == 0) { count += 2; value <<= 2; }
			if ((value & 0x8000000000000000UL) == 0) { count += 1; }
			return count;
#endif
		}

		/// <summary>
		/// 计算尾部零的个数（0~64）
		/// </summary>
		public static int TrailingZeroCount(ulong value)
		{
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            return BitOperations.TrailingZeroCount(value);
#else
			if (value == 0) return 64;

			int count = 0;
			if ((value & 0x00000000FFFFFFFFUL) == 0) { count += 32; value >>= 32; }
			if ((value & 0x000000000000FFFFUL) == 0) { count += 16; value >>= 16; }
			if ((value & 0x00000000000000FFUL) == 0) { count += 8; value >>= 8; }
			if ((value & 0x000000000000000FUL) == 0) { count += 4; value >>= 4; }
			if ((value & 0x0000000000000003UL) == 0) { count += 2; value >>= 2; }
			if ((value & 0x0000000000000001UL) == 0) { count += 1; }
			return count;
#endif
		}

	}
}
