using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreeDataFormats
{
	public static class Array1FormatRule
	{
		/// <summary>
		/// 泛型一维数组序列化
		/// </summary>
		public class Serialize<T> : TreeDataSerializeRule<TreeDataByteSequence, T[]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(T[]));
				T[] values = (T[])value;
				if (values == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				//写入数组维度数量
				self.WriteUnmanaged(~1);
				if (values.Length == 0)
				{
					self.WriteUnmanaged(0);
					return;
				}

				//判断是否为基础类型
				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
				{
					//获取数组数据长度
					var srcLength = size * values.Length;

					//包含数组数量的总长度
					var allocSize = srcLength + Unsafe.SizeOf<int>();

					//获取写入操作跨度
					ref byte spanRef = ref self.GetWriteRefByte(allocSize);

					//获取数组数据的指针
					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values.AsSpan()));

					//写入数组长度
					Unsafe.WriteUnaligned(ref spanRef, values.Length);
					//写入数组数据
					Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<int>()), ref src, (uint)srcLength);

				}
				else //当成托管类型处理
				{
					//写入数组数据长度
					self.WriteUnmanaged(values.Length);
					//写入数组数据
					for (int i = 0; i < values.Length; i++)
					{
						T t = values[i];
						self.WriteValue(t);
					}
				}
			}
		}

		/// <summary>
		/// 泛型一维数组反序列化
		/// </summary>
		public class Deserialize<T> : TreeDataDeserializeRule<TreeDataByteSequence, T[]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!(self.TryReadType(out Type type) && type == typeof(T[])))
				{
					//跳跃数据
					self.SkipData(type);
					return;
				}
				//读取数组维度数量
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				count = ~count;
				if (count != 1)
				{
					//读取指针回退
					self.ReadBack(4);
					self.SkipData(type);
					return;
				}

				self.ReadUnmanaged(out int length);
				if (length == 0)
				{
					value = Array.Empty<T>();
					return;
				}
				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[])value).Length != length) value = new T[length];

				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
				{
					var byteCount = length * size;
					ref byte spanRef = ref self.GetReadRefByte(byteCount);

					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(((T[])value).AsSpan()));
					Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
				}
				else //当成托管类型处理
				{
					//读取数组数据
					for (int i = 0; i < length; i++)
					{
						self.ReadValue(ref ((T[])value)[i]);
					}
				}
			}
		}
	}
}
