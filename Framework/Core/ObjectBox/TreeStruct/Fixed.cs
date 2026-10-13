/****************************************

* 作者：闪电黑客
* 日期：2024/12/4 10:55

* 描述：

*/
using System;

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
		public static readonly Fixed MaxValue = new Fixed(MAX_VALUE - 1);//为了在某些运算（如加法）中留出安全的空间，防止加法运算直接溢出到负数区域
		/// <summary>
		/// 最小值
		/// </summary>
		public static readonly Fixed MinValue = new Fixed(MIN_VALUE + 2);//为了在减法等运算中避免极端溢出，给负数运算留出一些安全边界

		/// <summary>
		/// 值位数
		/// </summary>
		public const int NUM_BITS = 64;

		/// <summary>
		/// 1
		/// </summary>
		public const long ONE = 1L << FRACTIONAL_PLACES;
		/// <summary>
		/// 10
		/// </summary>
		public const long TEN = 10L << FRACTIONAL_PLACES;

		public Fixed(long value) => Value = value;

		#region 转换

		public static implicit operator Fixed(byte value) => new(value << FRACTIONAL_PLACES);
		public static implicit operator byte(Fixed value) => (byte)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(short value) => new(value << FRACTIONAL_PLACES);
		public static implicit operator short(Fixed value) => (short)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(int value) => new(value << FRACTIONAL_PLACES);
		public static implicit operator int(Fixed value) => (int)(value.Value >> FRACTIONAL_PLACES);

		public static implicit operator Fixed(long value) => new(value << FRACTIONAL_PLACES);
		public static implicit operator long(Fixed value) => value.Value >> FRACTIONAL_PLACES;

		public static implicit operator Fixed(float value) => new((long)(value * ONE));
		public static implicit operator float(Fixed value) => (float)value.Value / ONE;

		public static implicit operator Fixed(double value) => new((long)(value * ONE));
		public static implicit operator double(Fixed value) => (double)value.Value / ONE;

		public static implicit operator Fixed(decimal value) => new((long)(value * ONE));
		public static implicit operator decimal(Fixed value) => (decimal)value.Value / ONE;

		#endregion

		#region 运算符

		public static Fixed operator +(Fixed a, Fixed b) => new Fixed(a.Value + b.Value);//3倍
		public static Fixed operator -(Fixed a, Fixed b) => new Fixed(a.Value - b.Value);//3倍
		public static Fixed operator *(Fixed a, Fixed b)//7倍
		{
			// 分成高位和低位部分
			ulong aLow = (ulong)(a.Value & 0x00000000FFFFFFFF);
			long aHigh = a.Value >> FRACTIONAL_PLACES;
			ulong bLow = (ulong)(b.Value & 0x00000000FFFFFFFF);
			long bHigh = b.Value >> FRACTIONAL_PLACES;

			// 计算高位和低位的乘积
			long highProduct = aHigh * bHigh;
			long crossProduct1 = aHigh * (long)bLow;
			long crossProduct2 = (long)aLow * bHigh;
			ulong lowProduct = aLow * bLow;

			// 将结果合并
			long resultHigh = highProduct << FRACTIONAL_PLACES;
			ulong resultLow = lowProduct >> FRACTIONAL_PLACES;
			long resultCross = crossProduct1 + crossProduct2;
			long result = resultHigh + resultCross + (long)resultLow;
			return new Fixed(result);
		}

		/// <summary>
		/// 前导零计数
		/// </summary>
		public static int CountLeadingZeroes(ulong x)
		{
			int result = 0;
			while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
			while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
			return result;
		}

		public static Fixed operator /(Fixed a, Fixed b)//20倍
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
				int shift = CountLeadingZeroes(absDividend);

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

		public static Fixed operator %(Fixed a, Fixed b)
		{
			Fixed result;
			result.Value = a.Value == MIN_VALUE & b.Value == -1 ? 0 : a.Value % b.Value;
			return result;
		}

		public static Fixed operator -(Fixed a) => a.Value == MIN_VALUE ? MaxValue : new Fixed(-a.Value);

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

		#region 比较符

		public static bool operator ==(Fixed a, Fixed b) => a.Value == b.Value;
		public static bool operator !=(Fixed a, Fixed b) => a.Value != b.Value;
		public static bool operator >(Fixed a, Fixed b) => a.Value > b.Value;
		public static bool operator <(Fixed a, Fixed b) => a.Value < b.Value;
		public static bool operator >=(Fixed a, Fixed b) => a.Value >= b.Value;
		public static bool operator <=(Fixed a, Fixed b) => a.Value <= b.Value;

		#endregion

		#region 计算方法

		/// <summary>
		/// 开平方
		/// </summary>
		public static Fixed Sqrt(Fixed value)
		{
			var numValue = value.Value;

			// 如果值为负数，抛出异常
			if (numValue < 0) throw new ArithmeticException("负数不能开平方");
			// 如果值为0，直接返回0
			if (numValue == 0) return 0;

			// 将值转换为无符号长整型
			ulong num = (ulong)numValue;
			ulong result = 0UL;
			// 初始化bit为次高位
			var bit = 1UL << (NUM_BITS - 2);

			// 找到合适的初始bit位置
			while (bit > num) bit >>= 2;

			// 主循环部分执行两次，以避免使用128位的值进行计算
			for (var i = 0; i < 2; ++i)
			{
				// 首先获取答案的高48位，逐步逼近平方根的值
				while (bit != 0)
				{
					// 如果 num 大于等于 result + bit
					if (num >= result + bit)
					{
						// 从 num 中减去 result + bit
						num -= result + bit;
						// 将 result 右移一位并加上 bit
						result = (result >> 1) + bit;
					}
					else
					{
						// 否则，仅将 result 右移一位
						result = result >> 1;
					}
					// 将 bit 右移两位
					bit >>= 2;
				}


				if (i == 0)
				{
					// 然后再次处理以获得最低的16位
					if (num > (1UL << (NUM_BITS / 2)) - 1)
					{
						// 如果余数太大，无法左移32位，则手动加1并调整余数
						num -= result;
						num = (num << (NUM_BITS / 2)) - 0x80000000UL;
						result = (result << (NUM_BITS / 2)) + 0x80000000UL;
					}
					else
					{
						// 否则，直接将 num 和 result 左移32位
						num <<= (NUM_BITS / 2);
						result <<= (NUM_BITS / 2);
					}

					// 将 bit 设置为次高位
					bit = 1UL << (NUM_BITS / 2 - 2);
				}
			}

			// 最后，如果下一个bit是1，则将结果向上舍入
			if (num > result) ++result;

			Fixed r;
			r.Value = (long)result;
			return r;
		}


		/// <summary>
		/// 返回一个绝对值。
		/// 注意：Abs(Fixed.MinValue) == Fixed.MaxValue。
		/// </summary>
		public static Fixed Abs(Fixed value)
		{
			// 如果值等于最小值，返回最大值
			if (value.Value == MIN_VALUE) return MaxValue;

			// 无分支实现，参考 http://www.strchr.com/optimized_abs_function
			// 计算掩码，右移63位，相当于取符号位
			var mask = value.Value >> 63;
			Fixed result;
			// 计算绝对值：如果是负数，(value.Value + mask) ^ mask 会将其取反；如果是正数，结果不变
			result.Value = (value.Value + mask) ^ mask;
			return result;
		}



		#endregion

	}
}
