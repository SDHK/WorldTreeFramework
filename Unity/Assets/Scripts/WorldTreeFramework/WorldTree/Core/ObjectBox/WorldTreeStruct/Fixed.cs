/****************************************

* 作者：闪电黑客
* 日期：2024/12/4 10:55

* 描述：

*/
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 定点数
	/// </summary>
	public struct Fixed
	{
		/// <summary>
		/// 值
		/// </summary>
		public long Value;

		/// <summary>
		/// 小数位数
		/// </summary>
		public const int FRACTIONAL_PLACES = 32;

		/// <summary>
		/// 最大值
		/// </summary>
		public const long MAX_VALUE = long.MaxValue;

		/// <summary>
		/// 最小值
		/// </summary>
		public const long MIN_VALUE = long.MinValue;

		/// <summary>
		/// 最大值
		/// </summary>
		public static readonly Fixed MaxValue = new Fixed(MAX_VALUE - 1);
		/// <summary>
		/// 最小值
		/// </summary>
		public static readonly Fixed MinValue = new Fixed(MIN_VALUE + 2);

		/// <summary>
		/// 值位数
		/// </summary>
		public const int NUM_BITS = 64;

		/// <summary>
		/// 1
		/// </summary>
		public const long ONE = 1L << FRACTIONAL_PLACES;

		/// <summary>
		/// 低位掩码
		/// </summary>
		public const long LOW_MASK = ONE - 1;

		public Fixed(long value) => Value = value;

		#region 转换

		public static implicit operator Fixed(byte value) => new Fixed(value << FRACTIONAL_PLACES);
		public static implicit operator byte(Fixed value) => (byte)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(short value) => new Fixed(value << FRACTIONAL_PLACES);
		public static implicit operator short(Fixed value) => (short)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(int value) => new Fixed(value << FRACTIONAL_PLACES);
		public static implicit operator int(Fixed value) => (int)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(long value) => new Fixed(value << FRACTIONAL_PLACES);
		public static implicit operator long(Fixed value) => value.Value >> FRACTIONAL_PLACES;

		public static implicit operator Fixed(float value) => new Fixed((long)Math.Round(value * ONE));
		public static implicit operator float(Fixed value) => (float)value.Value / ONE;

		public static implicit operator Fixed(double value) => new Fixed((long)Math.Round(value * ONE));
		public static implicit operator double(Fixed value) => (double)value.Value / ONE;

		public static implicit operator Fixed(decimal value) => new Fixed((long)Math.Round(value * ONE));
		public static implicit operator decimal(Fixed value) => (decimal)value.Value / ONE;

		#endregion

		#region 计算

		public static Fixed operator +(Fixed a, Fixed b) => new Fixed(a.Value + b.Value);
		public static Fixed operator -(Fixed a, Fixed b) => new Fixed(a.Value - b.Value);
		public static Fixed operator *(Fixed a, Fixed b)
		{
			// 分成高位和低位部分
			long aHigh = a.Value >> FRACTIONAL_PLACES;
			long aLow = a.Value & LOW_MASK;
			long bHigh = b.Value >> FRACTIONAL_PLACES;
			long bLow = b.Value & LOW_MASK;

			// 计算高位和低位的乘积
			long highProduct = aHigh * bHigh;
			long crossProduct1 = aHigh * bLow;
			long crossProduct2 = aLow * bHigh;
			long lowProduct = aLow * bLow;

			// 将结果合并
			long resultHigh = highProduct << FRACTIONAL_PLACES;
			long resultCross = crossProduct1 + crossProduct2;
			long resultLow = lowProduct >> FRACTIONAL_PLACES;
			long result = resultHigh + resultCross + resultLow;
			return new Fixed(result);
		}

		public static Fixed operator /(Fixed a, Fixed b)
		{
			var dividend = a.Value;
			var divisor = b.Value;

			//除0异常
			if (divisor == 0) throw new DivideByZeroException("除数不能为零");
			//除1优化
			if (divisor == ONE) return a;
			//除-1优化
			if (divisor == -ONE) { a.Value = -a.Value; return a; }

			// 取绝对值并转换为无符号长整型
			var absDividend = (ulong)(dividend >= 0 ? dividend : -dividend);
			// 取绝对值并转换为无符号长整型
			var absDivisor = (ulong)(divisor >= 0 ? divisor : -divisor);


			// 商
			var quotient = 0UL;
			// 位移量，初始化为32+1
			var offset = NUM_BITS / 2 + 1;

			// 如果除数可以被2^n整除，进行优化
			// 如果absDivisor的最低4位都是0，可以被16整除，将absDivisor右移4位，位移量减4，减少除法运算的次数
			while ((absDivisor & 0xF) == 0 && offset >= 4)
			{
				absDivisor >>= 4; // 右移4位
				offset -= 4; // 处理位置移量减4
			}

			// 主循环，执行除法运算，直到被除数为0或者位移量小于0
			while (absDividend != 0 && offset >= 0)
			{
				// 计算除数前导零的数量
				int shift = BitOperations.LeadingZeroCount(absDividend);

				// 如果前导零数量大于位移量，前导零数设置为位移量
				if (shift > offset) shift = offset;

				absDividend <<= shift; // 被除数左移
				offset -= shift; // 位移量减去移位数

				// 计算当前商
				ulong currentQuotient = absDividend / absDivisor;
				// 计算余数，作为下一轮的被除数
				absDividend = absDividend % absDivisor;
				// 将当前商左移并加到商变量中，位移offset是对齐当前商的最高位
				quotient += currentQuotient << offset;

				// 检测溢出，溢出则返回最大值或最小值
				// 计算掩码，偏移到当前商的最高位，右移offset位后取反
				ulong mask = ~(0xFFFFFFFFFFFFFFFF >> offset);
				// 将掩码应用到当前商，作用是将低位的1全部置为0
				ulong maskedQuotient = currentQuotient & mask;
				//(dividend ^ divisor) 是异或运算，如果符号不同，结果为1，否则为0
				// & MIN_VALUE 是借用-号为1 的 与运算，如果结果为0，说明符号相同，返回最大值，否则返回最小值
				if (maskedQuotient != 0) return ((dividend ^ divisor) & MIN_VALUE) == 0 ? MaxValue : MinValue;


				absDividend <<= 1; // 余数左移1位，相当于乘2
				--offset; // 位移量减1
			}

			// 四舍五入，从而得到更精确的结果。
			// 通过这种方式，如果 quotient 的最低位是 1（即小数部分大于等于 0.5），
			// 加 1 后右移1会使结果向上舍入；如果最低位是 0（即小数部分小于 0.5），
			// 加 1 后右移1不会改变结果的整数部分，从而实现四舍五入。
			// 加 1 操作不会导致最终结果溢出到 long 类型之外，因为在右移一位后，ulong 的值仍然在 long 的范围内。
			// 最后右移1位得到最终结果
			++quotient;
			var resultValue = (long)(quotient >> 1);

			// 如果被除数和除数的符号不同，结果取反
			if (((dividend ^ divisor) & MIN_VALUE) != 0) resultValue = -resultValue;

			Fixed resultFixed;
			resultFixed.Value = resultValue; // 将结果赋值给Fixed对象
			return resultFixed; // 返回结果
		}



		/// <summary>
		/// 计算无符号长整型数值的前导零的数量,更快的方法 BitOperations.LeadingZeroCount
		/// </summary>
		/// <param name="x">无符号长整型数值</param>
		/// <returns>前导零的数量</returns>
		[MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
		public static int CountLeadingZeroes(ulong x)
		{
			int result = 0;
			// 快速统计连续4位都是0的情况
			// 每次检查最高的4位，如果它们都是0，则将结果增加4，并将x左移4位
			while ((x & 0xF000000000000000) == 0)
			{
				result += 4;
				x <<= 4;
			}
			// 统计剩余的位数
			// 每次检查最高的一位，如果它是0，则将结果增加1，并将x左移1位
			while ((x & 0x8000000000000000) == 0)
			{
				result += 1;
				x <<= 1;
			}
			return result;
		}


		public static Fixed operator %(Fixed a, Fixed b) => new Fixed(a.Value % b.Value);
		public static Fixed operator ++(Fixed a)
		{
			a.Value += ONE;
			return a;
		}

		public static Fixed operator --(Fixed a)
		{
			a.Value -= ONE;
			return a;
		}


		#endregion

	}
}
