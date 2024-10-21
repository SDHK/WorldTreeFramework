using System;
using System.Collections.Generic;

namespace WorldTree.TreeDataFormats
{
	public static class BoolFormatRule
	{
		/// <summary>
		/// Bool序列化基类
		/// </summary>
		abstract class Serialize : TreeDataSerializeRule<TreeDataByteSequence, bool>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(bool));
				self.WriteUnmanaged((bool)value);
			}
		}

		/// <summary>
		/// Bool反序列化基类
		/// </summary>
		abstract class Deserialize : TreeDataDeserializeRule<TreeDataByteSequence, bool>
		{
			/// <summary>
			/// 转换表
			/// </summary>
			public Dictionary<Type, Func<TreeDataByteSequence, bool>> TypeDict = new()
			{
				[typeof(bool)] = (self) => self.ReadUnmanaged(out bool data),
				[typeof(byte)] = (self) => self.ReadUnmanaged(out byte data) != 0,
				[typeof(sbyte)] = (self) => self.ReadUnmanaged(out sbyte data) != 0,
				[typeof(short)] = (self) => self.ReadUnmanaged(out short data) != 0,
				[typeof(ushort)] = (self) => self.ReadUnmanaged(out ushort data) != 0,
				[typeof(int)] = (self) => self.ReadUnmanaged(out int data) != 0,
				[typeof(uint)] = (self) => self.ReadUnmanaged(out uint data) != 0,
				[typeof(long)] = (self) => self.ReadUnmanaged(out long data) != 0,
				[typeof(ulong)] = (self) => self.ReadUnmanaged(out ulong data) != 0,
				[typeof(float)] = (self) => self.ReadUnmanaged(out float data) != 0,
				[typeof(double)] = (self) => self.ReadUnmanaged(out double data) != 0,
				[typeof(char)] = (self) => self.ReadUnmanaged(out char data) != '0',
				[typeof(string)] = (self) => bool.TryParse(self.ReadString(), out bool result) && result,
			};

			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				if (self.TryReadType(out Type type) && TypeDict.TryGetValue(type, out var func))
				{
					value = func(self);
				}
				else
				{
					self.ReadBack(8);
					self.SkipData();
					return;
				}
			}
		}
	}


	public static class UnmanagedFormatRule
	{
		/// <summary>
		/// 非托管序列化基类
		/// </summary>
		abstract class UnmanagedSerialize<T> : TreeDataSerializeRule<TreeDataByteSequence, T>
			where T : unmanaged
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(T));
				self.WriteUnmanaged((T)value);
			}
		}

		/// <summary>
		/// 非托管反序列化基类
		/// </summary>
		abstract class UnmanagedDeserialize<T> : TreeDataDeserializeRule<TreeDataByteSequence, T>
			where T : unmanaged
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				if (self.TryReadType(out Type type) && type == typeof(T))
				{
					value = self.ReadUnmanaged<T>();
				}
				else
				{
					self.ReadBack(8);
					self.SkipData();
				}
			}
		}

		class BoolSerialize : UnmanagedSerialize<bool> { }
		class BoolDeserialize : UnmanagedDeserialize<bool> { }

		class ByteSerialize : UnmanagedSerialize<byte> { }
		class ByteDeserialize : UnmanagedDeserialize<byte> { }

		class SByteSerialize : UnmanagedSerialize<sbyte> { }
		class SByteDeserialize : UnmanagedDeserialize<sbyte> { }

		class ShortSerialize : UnmanagedSerialize<short> { }
		class ShortDeserialize : UnmanagedDeserialize<short> { }

		class UShortSerialize : UnmanagedSerialize<ushort> { }
		class UShortDeserialize : UnmanagedDeserialize<ushort> { }

		class IntSerialize : UnmanagedSerialize<int> { }
		class IntDeserialize : UnmanagedDeserialize<int> { }

		class UIntSerialize : UnmanagedSerialize<uint> { }
		class UIntDeserialize : UnmanagedDeserialize<uint> { }

		class LongSerialize : UnmanagedSerialize<long> { }
		class LongDeserialize : UnmanagedDeserialize<long> { }

		class ULongSerialize : UnmanagedSerialize<ulong> { }
		class ULongDeserialize : UnmanagedDeserialize<ulong> { }

		class FloatSerialize : UnmanagedSerialize<float> { }
		class FloatDeserialize : UnmanagedDeserialize<float> { }

		class DoubleSerialize : UnmanagedSerialize<double> { }
		class DoubleDeserialize : UnmanagedDeserialize<double> { }

		class CharSerialize : UnmanagedSerialize<char> { }
		class CharDeserialize : UnmanagedDeserialize<char> { }

		class DecimalSerialize : UnmanagedSerialize<decimal> { }
		class DecimalDeserialize : UnmanagedDeserialize<decimal> { }
	}
}
