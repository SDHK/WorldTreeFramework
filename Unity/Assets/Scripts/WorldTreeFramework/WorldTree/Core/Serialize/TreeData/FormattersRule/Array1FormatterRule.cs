/****************************************

* 作者：闪电黑客
* 日期：2024/10/8 15:59

* 描述：

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreeDataFormatters
{
	public static class Array1FormatterRule
	{
		/// <summary>
		/// 泛型一维数组序列化
		/// </summary>
		private class Serialize<T> : TreeDataSerializeRule<T[]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				//判断是否为基础类型，基础类型需要写入完整数组类型
				if (typeMode == SerializedTypeMode.ObjectType && TreeDataTypeHelper.TypeSizeDict.ContainsKey(typeof(T)))
					typeMode = SerializedTypeMode.DataType;

				if (self.TryWriteDataHead(value, typeMode, ~1, out T[] obj)) return;

				//写入数组数据长度
				self.WriteDynamic(obj.Length);
				if (obj.Length == 0) return;

				//type.GetEnumUnderlyingType();？？枚举类型的基础类型优化,如果有则跳跃数据也需要

				//判断是否为基础类型
				if (TreeDataTypeHelper.TypeSizeDict.TryGetValue(typeof(T), out int size))
				{
					//获取数组数据长度
					var srcLength = size * obj.Length;

					//获取写入操作跨度
					ref byte spanRef = ref self.GetWriteRefByte(srcLength);

					//获取数组数据的指针
					ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(obj.AsSpan()));

					//写入数组数据
					Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)srcLength);

				}
				else //当成托管类型处理
				{
					//写入数组数据
					for (int i = 0; i < obj.Length; i++)
					{
						T t = obj[i];
						self.WriteValue(t);
					}
				}
			}
		}

		/// <summary>
		/// 泛型一维数组反序列化
		/// </summary>
		private class Deserialize<T> : TreeDataDeserializeRule<T[]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadArrayHead(typeof(T[]), ref value, 1)) return;

				self.ReadDynamic(out int length);
				if (length == 0)
				{
					value = Array.Empty<T>();
					return;
				}

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[])value).Length != length) value = new T[length];

				if (TreeDataTypeHelper.TypeSizeDict.TryGetValue(typeof(T), out int size))
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
