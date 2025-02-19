/****************************************

* 作者：闪电黑客
* 日期：2024/8/15 14:17

* 描述：

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreePackFormats
{
	public static class Array1FormatterRule
	{
		/// <summary>
		/// 泛型一维数组序列化
		/// </summary>
		private class Serialize<T> : TreePackSerializeRule<T[]>
		{
			protected override void Execute(TreePackByteSequence self, ref T[] value)
			{
				if (value == null)
				{
					self.WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
					return;
				}
				if (value.Length == 0)
				{
					self.WriteUnmanaged(0);
					return;
				}

				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					self.WriteUnmanaged(value.Length);
					foreach (T item in value) self.WriteValue(item);
				}
				else
				{
					//获取数组数据长度
					var srcLength = Unsafe.SizeOf<T>() * value.Length;
					//包含数组数量的总长度
					var allocSize = srcLength + Unsafe.SizeOf<int>();
					//获取写入操作跨度
					ref byte spanRef = ref self.GetWriteRefByte(allocSize);
					//获取数组数据的指针
					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));
					//写入数组长度
					Unsafe.WriteUnaligned(ref spanRef, value.Length);
					//写入数组数据
					Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<int>()), ref src, (uint)srcLength);
				}
			}
		}

		/// <summary>
		/// 泛型一维数组反序列化
		/// </summary>
		private class Deserialize<T> : TreePackDeserializeRule<T[]>
		{
			protected override void Execute(TreePackByteSequence self, ref T[] value)
			{
				if (self.ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				else if (length == 0)
				{
					value = Array.Empty<T>();
					return;
				}

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || value.Length != length)
				{
					value = new T[length];
				}

				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					for (int i = 0; i < value.Length; i++) self.ReadValue(ref value[i]);
				}
				else
				{
					var byteCount = length * Unsafe.SizeOf<T>();
					ref byte spanRef = ref self.GetReadRefByte(byteCount);
					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));
					Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
				}
			}
		}
	}
}
