/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;

namespace WorldTree
{

	public static partial class ByteSequenceRule
	{
		#region 写入

		/// <summary>
		/// 写入byte，无符号字节
		/// </summary>
		public static void WriteDynamic(this IByteSequence self, byte x)
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
		public static void WriteDynamic(this IByteSequence self, sbyte x)
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
		public static void WriteDynamic(this IByteSequence self, ushort x)
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
		public static void WriteDynamic(this IByteSequence self, short x)
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
		public static void WriteDynamic(this IByteSequence self, uint x)
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
		public static void WriteDynamic(this IByteSequence self, int x)
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
		public static void WriteDynamic(this IByteSequence self, ulong x)
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
		public static void WriteDynamic(this IByteSequence self, long x)
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

		#endregion

		#region 读取

		/// <summary>
		/// 读取byte，无符号字节
		/// </summary>
		public static byte ReadDynamic(this IByteSequence self, out byte value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((byte)self.ReadUnmanaged<sbyte>());
				case ValueTypeCode.UINT16:
					return value = checked((byte)self.ReadUnmanaged<byte>());
				case ValueTypeCode.INT16:
					return value = checked((byte)self.ReadUnmanaged<short>());
				case ValueTypeCode.UINT32:
					return value = checked((byte)self.ReadUnmanaged<uint>());
				case ValueTypeCode.INT32:
					return value = checked((byte)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = checked((byte)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((byte)self.ReadUnmanaged<long>());
				default:
					return value = checked((byte)typeCode);
			}
		}

		/// <summary>
		/// 读取sbyte，有符号字节
		/// </summary>
		public static sbyte ReadDynamic(this IByteSequence self, out sbyte value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = checked((sbyte)self.ReadUnmanaged<byte>());
				case ValueTypeCode.SBYTE:
					return value = self.ReadUnmanaged<sbyte>();
				case ValueTypeCode.UINT16:
					return value = checked((sbyte)self.ReadUnmanaged<byte>());
				case ValueTypeCode.INT16:
					return value = checked((sbyte)self.ReadUnmanaged<short>());
				case ValueTypeCode.UINT32:
					return value = checked((sbyte)self.ReadUnmanaged<uint>());
				case ValueTypeCode.INT32:
					return value = checked((sbyte)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = checked((sbyte)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((sbyte)self.ReadUnmanaged<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取ushort，无符号短整数
		/// </summary>
		public static ushort ReadDynamic(this IByteSequence self, out ushort value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((ushort)self.ReadUnmanaged<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.ReadUnmanaged<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((ushort)self.ReadUnmanaged<short>());
				case ValueTypeCode.UINT32:
					return value = checked((ushort)self.ReadUnmanaged<uint>());
				case ValueTypeCode.INT32:
					return value = checked((ushort)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = checked((ushort)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((ushort)self.ReadUnmanaged<long>());
				default:
					return value = checked((ushort)typeCode);
			}
		}

		/// <summary>
		/// 读取short，有符号短整数
		/// </summary>
		public static short ReadDynamic(this IByteSequence self, out short value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.ReadUnmanaged<sbyte>();
				case ValueTypeCode.UINT16:
					return value = checked((short)self.ReadUnmanaged<ushort>());
				case ValueTypeCode.INT16:
					return value = self.ReadUnmanaged<short>();
				case ValueTypeCode.UINT32:
					return value = checked((short)self.ReadUnmanaged<uint>());
				case ValueTypeCode.INT32:
					return value = checked((short)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = checked((short)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((short)self.ReadUnmanaged<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取uint，无符号整数
		/// </summary>
		public static uint ReadDynamic(this IByteSequence self, out uint value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((uint)self.ReadUnmanaged<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.ReadUnmanaged<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((uint)self.ReadUnmanaged<short>());
				case ValueTypeCode.UINT32:
					return value = self.ReadUnmanaged<uint>();
				case ValueTypeCode.INT32:
					return value = checked((uint)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = checked((uint)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((uint)self.ReadUnmanaged<long>());
				default:
					return value = checked((uint)typeCode);
			}
		}

		/// <summary>
		/// 读取int，有符号整数
		/// </summary>
		public static int ReadDynamic(this IByteSequence self, out int value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.ReadUnmanaged<sbyte>();
				case ValueTypeCode.UINT16:
					return value = self.ReadUnmanaged<ushort>();
				case ValueTypeCode.INT16:
					return value = self.ReadUnmanaged<short>();
				case ValueTypeCode.UINT32:
					return value = checked((int)self.ReadUnmanaged<uint>());
				case ValueTypeCode.INT32:
					return value = self.ReadUnmanaged<int>();
				case ValueTypeCode.UINT64:
					return value = checked((int)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((int)self.ReadUnmanaged<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取ulong，无符号长整数
		/// </summary>
		public static ulong ReadDynamic(this IByteSequence self, out ulong value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((ulong)self.ReadUnmanaged<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.ReadUnmanaged<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((ulong)self.ReadUnmanaged<short>());
				case ValueTypeCode.UINT32:
					return value = self.ReadUnmanaged<uint>();
				case ValueTypeCode.INT32:
					return value = checked((ulong)self.ReadUnmanaged<int>());
				case ValueTypeCode.UINT64:
					return value = self.ReadUnmanaged<ulong>();
				case ValueTypeCode.INT64:
					return value = checked((ulong)self.ReadUnmanaged<long>());
				default:
					return value = checked((ulong)typeCode);
			}
		}

		/// <summary>
		/// 读取long，有符号长整数
		/// </summary>
		public static long ReadDynamic(this IByteSequence self, out long value)
		{
			self.ReadUnmanaged(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.ReadUnmanaged<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.ReadUnmanaged<sbyte>();
				case ValueTypeCode.UINT16:
					return value = self.ReadUnmanaged<ushort>();
				case ValueTypeCode.INT16:
					return value = self.ReadUnmanaged<short>();
				case ValueTypeCode.UINT32:
					return value = self.ReadUnmanaged<uint>();
				case ValueTypeCode.INT32:
					return value = self.ReadUnmanaged<int>();
				case ValueTypeCode.UINT64:
					return value = checked((long)self.ReadUnmanaged<ulong>());
				case ValueTypeCode.INT64:
					return value = self.ReadUnmanaged<long>();
				default:
					return value = typeCode;
			}
		}

		#endregion
	}
}
