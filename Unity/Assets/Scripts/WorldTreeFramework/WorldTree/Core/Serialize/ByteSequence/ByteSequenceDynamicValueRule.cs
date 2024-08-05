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
		public static void WriteDynamic(this ByteSequence self, byte x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.Write((sbyte)x);
			}
			else
			{
				self.Write(ValueTypeCode.BYTE, x);
			}
		}

		/// <summary>
		/// 写入sbyte，有符号字节
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, sbyte x)
		{
			if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
			{
				//大于等于最小单字节值-120，直接写入
				self.Write(x);
			}
			else
			{
				self.Write(ValueTypeCode.SBYTE, x);
			}
		}

		/// <summary>
		/// 写入ushort，无符号短整数
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, ushort x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.Write((sbyte)x);
			}
			else
			{
				self.Write(ValueTypeCode.UINT16, (UInt16)x);
			}
		}

		/// <summary>
		/// 写入short，有符号短整数
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, short x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.Write((sbyte)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT16, (Int16)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.Write((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.Write(ValueTypeCode.SBYTE, (SByte)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT16, (Int16)x);
				}
			}
		}

		/// <summary>
		/// 写入uint
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, uint x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.Write((sbyte)x);
			}
			else if (x <= ushort.MaxValue)
			{
				self.Write(ValueTypeCode.UINT16, (UInt16)x);
			}
			else
			{
				self.Write(ValueTypeCode.UINT32, (UInt32)x);
			}
		}

		/// <summary>
		/// 写入int
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, int x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.Write((sbyte)x);
				}
				else if (x <= short.MaxValue)
				{
					self.Write(ValueTypeCode.INT16, (Int16)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT32, (Int32)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.Write((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.Write(ValueTypeCode.SBYTE, (SByte)x);
				}
				else if (short.MinValue <= x)
				{
					self.Write(ValueTypeCode.INT64, (Int16)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT32, (Int32)x);
				}
			}
		}

		/// <summary>
		/// 写入ulong
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, ulong x)
		{
			if (x <= ValueTypeCode.MAX_SINGLE_VALUE)
			{
				//小于等于最大单字节值127，直接写入
				self.Write((sbyte)x);
			}
			else if (x <= ushort.MaxValue)
			{
				self.Write(ValueTypeCode.UINT16, (UInt16)x);
			}
			else if (x <= uint.MaxValue)
			{
				self.Write(ValueTypeCode.UINT32, (UInt32)x);
			}
			else
			{
				self.Write(ValueTypeCode.UINT64, (UInt64)x);
			}
		}

		/// <summary>
		/// 写入long
		/// </summary>
		public static void WriteDynamic(this ByteSequence self, long x)
		{
			if (0 <= x)
			{
				if (x <= ValueTypeCode.MAX_SINGLE_VALUE) // same as sbyte.MaxValue
				{
					//小于等于最大单字节值127，直接写入
					self.Write((sbyte)x);
				}
				else if (x <= short.MaxValue)
				{
					self.Write(ValueTypeCode.INT16, (Int16)x);
				}
				else if (x <= int.MaxValue)
				{
					self.Write(ValueTypeCode.INT32, (Int32)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT64, (Int64)x);
				}
			}
			else
			{
				if (ValueTypeCode.MIN_SINGLE_VALUE <= x)
				{
					self.Write((sbyte)x);
				}
				else if (sbyte.MinValue <= x)
				{
					self.Write(ValueTypeCode.SBYTE, (SByte)x);
				}
				else if (short.MinValue <= x)
				{
					self.Write(ValueTypeCode.INT64, (Int16)x);
				}
				else if (int.MinValue <= x)
				{
					self.Write(ValueTypeCode.INT32, (Int32)x);
				}
				else
				{
					self.Write(ValueTypeCode.INT64, (Int64)x);
				}
			}
		}

		#endregion

		#region 读取

		/// <summary>
		/// 读取byte，无符号字节
		/// </summary>
		public static byte ReadDynamic(this ByteSequence self, out byte value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((byte)self.Read<sbyte>());
				case ValueTypeCode.UINT16:
					return value = checked((byte)self.Read<byte>());
				case ValueTypeCode.INT16:
					return value = checked((byte)self.Read<short>());
				case ValueTypeCode.UINT32:
					return value = checked((byte)self.Read<uint>());
				case ValueTypeCode.INT32:
					return value = checked((byte)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = checked((byte)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((byte)self.Read<long>());
				default:
					return value = checked((byte)typeCode);
			}
		}

		/// <summary>
		/// 读取sbyte，有符号字节
		/// </summary>
		public static sbyte ReadDynamic(this ByteSequence self, out sbyte value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = checked((sbyte)self.Read<byte>());
				case ValueTypeCode.SBYTE:
					return value = self.Read<sbyte>();
				case ValueTypeCode.UINT16:
					return value = checked((sbyte)self.Read<byte>());
				case ValueTypeCode.INT16:
					return value = checked((sbyte)self.Read<short>());
				case ValueTypeCode.UINT32:
					return value = checked((sbyte)self.Read<uint>());
				case ValueTypeCode.INT32:
					return value = checked((sbyte)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = checked((sbyte)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((sbyte)self.Read<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取ushort，无符号短整数
		/// </summary>
		public static ushort ReadDynamic(this ByteSequence self, out ushort value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((ushort)self.Read<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.Read<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((ushort)self.Read<short>());
				case ValueTypeCode.UINT32:
					return value = checked((ushort)self.Read<uint>());
				case ValueTypeCode.INT32:
					return value = checked((ushort)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = checked((ushort)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((ushort)self.Read<long>());
				default:
					return value = checked((ushort)typeCode);
			}
		}

		/// <summary>
		/// 读取short，有符号短整数
		/// </summary>
		public static short ReadDynamic(this ByteSequence self, out short value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.Read<sbyte>();
				case ValueTypeCode.UINT16:
					return value = checked((short)self.Read<ushort>());
				case ValueTypeCode.INT16:
					return value = self.Read<short>();
				case ValueTypeCode.UINT32:
					return value = checked((short)self.Read<uint>());
				case ValueTypeCode.INT32:
					return value = checked((short)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = checked((short)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((short)self.Read<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取uint，无符号整数
		/// </summary>
		public static uint ReadDynamic(this ByteSequence self, out uint value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((uint)self.Read<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.Read<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((uint)self.Read<short>());
				case ValueTypeCode.UINT32:
					return value = self.Read<uint>();
				case ValueTypeCode.INT32:
					return value = checked((uint)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = checked((uint)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((uint)self.Read<long>());
				default:
					return value = checked((uint)typeCode);
			}
		}

		/// <summary>
		/// 读取int，有符号整数
		/// </summary>
		public static int ReadDynamic(this ByteSequence self, out int value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.Read<sbyte>();
				case ValueTypeCode.UINT16:
					return value = self.Read<ushort>();
				case ValueTypeCode.INT16:
					return value = self.Read<short>();
				case ValueTypeCode.UINT32:
					return value = checked((int)self.Read<uint>());
				case ValueTypeCode.INT32:
					return value = self.Read<int>();
				case ValueTypeCode.UINT64:
					return value = checked((int)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = checked((int)self.Read<long>());
				default:
					return value = typeCode;
			}
		}

		/// <summary>
		/// 读取ulong，无符号长整数
		/// </summary>
		public static ulong ReadDynamic(this ByteSequence self, out ulong value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = checked((ulong)self.Read<sbyte>());
				case ValueTypeCode.UINT16:
					return value = self.Read<ushort>();
				case ValueTypeCode.INT16:
					return value = checked((ulong)self.Read<short>());
				case ValueTypeCode.UINT32:
					return value = self.Read<uint>();
				case ValueTypeCode.INT32:
					return value = checked((ulong)self.Read<int>());
				case ValueTypeCode.UINT64:
					return value = self.Read<ulong>();
				case ValueTypeCode.INT64:
					return value = checked((ulong)self.Read<long>());
				default:
					return value = checked((ulong)typeCode);
			}
		}

		/// <summary>
		/// 读取long，有符号长整数
		/// </summary>
		public static long ReadDynamic(this ByteSequence self, out long value)
		{
			self.Read(out sbyte typeCode);
			switch (typeCode)
			{
				case ValueTypeCode.BYTE:
					return value = self.Read<byte>();
				case ValueTypeCode.SBYTE:
					return value = self.Read<sbyte>();
				case ValueTypeCode.UINT16:
					return value = self.Read<ushort>();
				case ValueTypeCode.INT16:
					return value = self.Read<short>();
				case ValueTypeCode.UINT32:
					return value = self.Read<uint>();
				case ValueTypeCode.INT32:
					return value = self.Read<int>();
				case ValueTypeCode.UINT64:
					return value = checked((long)self.Read<ulong>());
				case ValueTypeCode.INT64:
					return value = self.Read<long>();
				default:
					return value = typeCode;
			}
		}

		#endregion
	}
}
