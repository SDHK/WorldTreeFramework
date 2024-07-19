/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;

namespace WorldTree
{


	public static partial class ByteSequenceWriterRule
	{
		/// <summary>
		/// 写入byte，无符号字节
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, byte x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.WriteUnmanaged((sbyte)x);
			}
			else
			{
				self.WriteUnmanaged(ValueTypeCode.BYTE, x);
			}
		}

		/// <summary>
		/// 写入sbyte，有符号字节
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, sbyte x)
		{
			if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
			{
				//大于等于最小单字节值-120，直接写入
				self.WriteUnmanaged(x);
			}
			else
			{
				self.WriteUnmanaged(ValueTypeCode.SBYTE, x);
			}
		}

		/// <summary>
		/// 写入ushort，无符号短整数
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, ushort x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.WriteUnmanaged((sbyte)x);
			}
			else
			{
				self.WriteUnmanaged(ValueTypeCode.UINT16, (UInt16)x);
			}
		}

		/// <summary>
		/// 写入short，有符号短整数
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, short x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.WriteUnmanaged((sbyte)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT16, (Int16)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.WriteUnmanaged((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.SBYTE, (SByte)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT16, (Int16)x);
				}
			}
		}

		/// <summary>
		/// 写入uint
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, uint x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.WriteUnmanaged((sbyte)x);
			}
			else if (x <= ushort.MaxValue)
			{
				self.WriteUnmanaged(ValueTypeCode.UINT16, (UInt16)x);
			}
			else
			{
				self.WriteUnmanaged(ValueTypeCode.UINT32, (UInt32)x);
			}
		}

		/// <summary>
		/// 写入int
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, int x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.WriteUnmanaged((sbyte)x);
				}
				else if (x <= short.MaxValue)
				{
					self.WriteUnmanaged(ValueTypeCode.INT16, (Int16)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT32, (Int32)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.WriteUnmanaged((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.SBYTE, (SByte)x);
				}
				else if (short.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.INT64, (Int16)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT32, (Int32)x);
				}
			}
		}

		/// <summary>
		/// 写入ulong
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, ulong x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.WriteUnmanaged((sbyte)x);
			}
			else if (x <= ushort.MaxValue)
			{
				self.WriteUnmanaged(ValueTypeCode.UINT16, (UInt16)x);
			}
			else if (x <= uint.MaxValue)
			{
				self.WriteUnmanaged(ValueTypeCode.UINT32, (UInt32)x);
			}
			else
			{
				self.WriteUnmanaged(ValueTypeCode.UINT64, (UInt64)x);
			}
		}

		/// <summary>
		/// 写入long
		/// </summary>
		public static void WriteDynamicValue(this ByteSequenceWriter self, long x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.WriteUnmanaged((sbyte)x);
				}
				else if (x <= short.MaxValue)
				{
					self.WriteUnmanaged(ValueTypeCode.INT16, (Int16)x);
				}
				else if (x <= int.MaxValue)
				{
					self.WriteUnmanaged(ValueTypeCode.INT32, (Int32)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT64, (Int64)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.WriteUnmanaged((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.SBYTE, (SByte)x);
				}
				else if (short.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.INT64, (Int16)x);
				}
				else if (int.MinValue <= x)
				{
					self.WriteUnmanaged(ValueTypeCode.INT32, (Int32)x);
				}
				else
				{
					self.WriteUnmanaged(ValueTypeCode.INT64, (Int64)x);
				}
			}
		}
	}
}
