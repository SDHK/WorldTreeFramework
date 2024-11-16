/****************************************

* 作者：闪电黑客
* 日期：2024/10/8 19:59

* 描述：

*/
using System;
using System.Runtime.CompilerServices;

namespace WorldTree.TreeDataFormatters
{
	public static class Array2FormatterRule
	{
		/// <summary>
		/// 泛型二维数组序列化
		/// </summary>
		private class Serialize<T> : TreeDataSerializeRule<T[,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryWriteDataHead(value, nameCode, ~2, out T[,] obj)) return;

				// 写入数组维度长度
				int dim1 = obj.GetLength(0);
				int dim2 = obj.GetLength(1);
				self.WriteDynamic(dim1);
				self.WriteDynamic(dim2);

				//判断是否为基础类型
				if (TreeDataType.TypeSizeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim2 * size;
					long totalSize = (long)dim1 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性写入整个数组数据
						ref byte spanRef = ref self.GetWriteRefByte((int)totalSize);
						ref byte src = ref Unsafe.As<T, byte>(ref obj[0, 0]);
						Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)totalSize);
					}
					else
					{
						// 分批写入数组数据
						for (int i = 0; i < dim1; i++)
						{
							ref byte spanRef = ref self.GetWriteRefByte(elementSize);
							ref byte src = ref Unsafe.As<T, byte>(ref obj[i, 0]);
							Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)elementSize);
						}
					}
				}
				else //当成托管类型处理
				{
					for (int i = 0; i < dim1; i++)
					{
						for (int j = 0; j < dim2; j++)
						{
							self.WriteValue(obj[i, j]);
						}
					}
				}
			}
		}


		/// <summary>
		/// 泛型二维数组反序列化
		/// </summary>
		private class Deserialize<T> : TreeDataDeserializeRule<T[,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadArrayHead(typeof(T[,]), ref value, 2)) return;

				self.ReadDynamic(out int dim1);
				self.ReadDynamic(out int dim2);

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[,])value).GetLength(0) != dim1 || ((T[,])value).GetLength(1) != dim2)
				{
					value = new T[dim1, dim2];
				}

				if (TreeDataType.TypeSizeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim2 * size;
					long totalSize = (long)dim1 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性读取整个数组数据
						ref byte spanRef = ref self.GetReadRefByte((int)totalSize);
						ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,])value)[0, 0]);
						Unsafe.CopyBlockUnaligned(ref dst, ref spanRef, (uint)totalSize);
					}
					else
					{
						// 分批读取数组数据
						for (int i = 0; i < dim1; i++)
						{
							ref byte spanRef = ref self.GetReadRefByte(elementSize);
							ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,])value)[i, 0]);
							Unsafe.CopyBlockUnaligned(ref dst, ref spanRef, (uint)elementSize);
						}
					}
				}
				else //当成托管类型处理
				{
					for (int i = 0; i < dim1; i++)
					{
						for (int j = 0; j < dim2; j++)
						{
							self.ReadValue(ref ((T[,])value)[i, j]);
						}
					}
				}
			}
		}
	}
}
