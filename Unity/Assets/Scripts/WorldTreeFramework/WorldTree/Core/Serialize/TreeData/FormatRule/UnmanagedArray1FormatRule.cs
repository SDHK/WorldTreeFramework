using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreeDataFormats
{
	//无法应对T[][]这种情况，因为无法确定数组的维度T本身可能就是T[],需要动态支持嵌套数组
	public static class UnmanagedArray1FormatRule
	{
		/// <summary>
		/// 非托管泛型一维数组序列化基类
		/// </summary>
		public class Serialize<T> : TreeDataSerializeRule<TreeDataByteSequence, T[]>
			where T : unmanaged
		{
			protected override void Execute(TreeDataByteSequence self, ref object arg1)
			{
				self.WriteType(typeof(T[]));
				T[] values = (T[])arg1;
				if (values == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				self.WriteUnmanaged(1);
				if (values.Length == 0)
				{
					self.WriteUnmanaged(0);
					return;
				}

				//获取数组数据长度
				var srcLength = Unsafe.SizeOf<T>() * values.Length;

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
		}

		/// <summary>
		/// 非托管泛型一维数组反序列化基类
		/// </summary>
		public class Deserialize<T> : TreeDataDeserializeRule<TreeDataByteSequence, T[]>
			where T : unmanaged
		{
			protected override unsafe void Execute(TreeDataByteSequence self, ref object value)
			{
				self.TryReadType(out Type type);
				if (type == typeof(T[]))
				{
					//self.DangerousReadUnmanagedArray(ref Unsafe.AsRef<int[]>(Unsafe.AsPointer(ref value)));

					if (self.ReadUnmanaged<int>() == ValueMarkCode.NULL_OBJECT)
					{
						value = null;
						return;
					}
					self.ReadUnmanaged(out int length);
					if (length == 0)
					{
						value = Array.Empty<T>();
						return;
					}

					var byteCount = length * Unsafe.SizeOf<T>();
					ref byte spanRef = ref self.GetReadRefByte(byteCount);

					//假如数组为空或长度不一致，那么重新分配
					if (value == null || ((T[])value).Length != length) value = new T[length];

					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(((T[])value).AsSpan()));
					Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
				}
				else
				{
					//读取指针回退，类型码
					self.ReadBack(8);
					//跳跃数据
					self.SkipData();
				}
			}
		}
	}
}
