using System;

namespace WorldTree.TreeDataFormats
{
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
				if (!(self.TryReadType(out Type type) && type == typeof(T)))
				{
					self.ReadBack(8);
					self.SkipData();
					return;
				}
				self.ReadUnmanaged(out T data);
				value = data;
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
