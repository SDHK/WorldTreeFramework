
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Numerics;
#endif

using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 数学位操作工具类 
	/// </summary>
	public static class MathBit
	{
#if !(NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
		/// <summary>
		/// De Bruijn 序列查找表 - 用于 TrailingZeroCount
		/// </summary>
		private static readonly int[] trailingDeBruijnPositions = new int[64]
		{
			0,  1,  2, 53,  3,  7, 54, 27,
			4, 38, 41,  8, 34, 55, 48, 28,
			62,  5, 39, 46, 44, 42, 22,  9,
			24, 35, 59, 56, 49, 18, 29, 11,
			63, 52,  6, 26, 37, 40, 33, 47,
			61, 45, 43, 21, 23, 58, 17, 10,
			51, 25, 36, 32, 60, 20, 57, 16,
			50, 31, 19, 15, 30, 14, 13, 12
		};

		/// <summary>
		/// De Bruijn 序列查找表 - 用于 LeadingZeroCount
		/// </summary>
		private static readonly int[] leadingDeBruijnPositions = new int[64]
		{
			63,  0, 58,  1, 59, 47, 53,  2,
			60, 39, 48, 27, 54, 33, 42,  3,
			61, 51, 37, 40, 49, 18, 28, 20,
			55, 30, 34, 11, 43, 14, 22,  4,
			62, 57, 46, 52, 38, 26, 32, 41,
			50, 36, 17, 19, 29, 10, 13, 21,
			56, 45, 25, 31, 35, 16,  9, 12,
			44, 24, 15,  8, 23,  7,  6,  5
		};
#endif



		/// <summary>
		/// 获取最高位1的位置（-1~63），全0的话返回-1
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetHighestBitIndex(long value)
		{
			if (value == 0) return -1;
			return 63 - LeadingZeroCount((ulong)value);
		}

		/// <summary>
		/// 获取最低位1的位置（-1~63），全0的话返回-1
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetLowestBitIndex(long value)
		{
			if (value == 0) return -1;
			return TrailingZeroCount((ulong)value);
		}

		/// <summary>
		/// 计算前导零的个数（0~64）
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong value)
		{
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            return BitOperations.LeadingZeroCount(value);
#else
			return LeadingZeroCountDeBruijn(value);
#endif
		}

		/// <summary>
		/// 计算尾部零的个数（0~64）
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(ulong value)
		{
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            return BitOperations.TrailingZeroCount(value);
#else
			return TrailingZeroCountDeBruijn(value);
#endif
		}

		/// <summary>
		/// 计算尾部零的个数（0~64）- 等价于 GetLowestBitIndex
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int TrailingZeroCountDeBruijn(ulong value)
		{
			if (value == 0) return 64;

			// 分离最低设置位
			ulong isolated = unchecked((ulong)((long)value & -(long)value));

			// De Bruijn 哈希
			ulong hash = isolated * 0x022fdd63cc95386dUL;

			// 提取索引并查表
			int index = (int)(hash >> 58);
			return trailingDeBruijnPositions[index];
		}


		/// <summary>
		/// 计算前导零的个数（0~64）
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LeadingZeroCountDeBruijn(ulong value)
		{
			if (value == 0) return 64;

			// 填充所有低位为1
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;

			// De Bruijn 哈希
			ulong hash = value * 0x03f6eaf2cd271461UL;

			// 提取索引并查表
			int index = (int)(hash >> 58);
			return leadingDeBruijnPositions[index];
		}


		/// <summary>
		/// 计算尾部零的个数（0~64）- 二分法 
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int TrailingZeroCountDichotomy(ulong value)
		{
			if (value == 0) return 64;

			int count = 0;
			if ((value & 0x00000000FFFFFFFFUL) == 0) { count += 32; value >>= 32; }
			if ((value & 0x000000000000FFFFUL) == 0) { count += 16; value >>= 16; }
			if ((value & 0x00000000000000FFUL) == 0) { count += 8; value >>= 8; }
			if ((value & 0x000000000000000FUL) == 0) { count += 4; value >>= 4; }
			if ((value & 0x0000000000000003UL) == 0) { count += 2; value >>= 2; }
			if ((value & 0x0000000000000001UL) == 0) { count += 1; }
			return count;
		}

		/// <summary>
		/// 计算前导零的个数（0~64）- 二分法 
		/// </summary>
		private static int LeadingZeroCountDichotomy(ulong value)
		{
			if (value == 0) return 64;
			int count = 0;
			if ((value & 0x00000000FFFFFFFFUL) == 0) { count += 32; value >>= 32; }
			if ((value & 0x000000000000FFFFUL) == 0) { count += 16; value >>= 16; }
			if ((value & 0x00000000000000FFUL) == 0) { count += 8; value >>= 8; }
			if ((value & 0x000000000000000FUL) == 0) { count += 4; value >>= 4; }
			if ((value & 0x0000000000000003UL) == 0) { count += 2; value >>= 2; }
			if ((value & 0x0000000000000001UL) == 0) { count += 1; }
			return count;
		}
	}
}
