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
			protected override void Execute(TreeDataByteSequence self, ref object obj)
			{
				self.WriteType(typeof(T[,]));
				T[,] values = (T[,])obj;
				if (values == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				//写入数组维度数量
				self.WriteUnmanaged(~2);

				// 写入数组维度长度
				int dim1 = values.GetLength(0);
				int dim2 = values.GetLength(1);
				self.WriteUnmanaged(dim1);
				self.WriteUnmanaged(dim2);

				//判断是否为基础类型
				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim2 * size;
					long totalSize = (long)dim1 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性写入整个数组数据
						ref byte spanRef = ref self.GetWriteRefByte((int)totalSize);
						ref byte src = ref Unsafe.As<T, byte>(ref values[0, 0]);
						Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)totalSize);
					}
					else
					{
						// 分批写入数组数据
						for (int i = 0; i < dim1; i++)
						{
							ref byte spanRef = ref self.GetWriteRefByte(elementSize);
							ref byte src = ref Unsafe.As<T, byte>(ref values[i, 0]);
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
							self.WriteValue(values[i, j]);
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
			protected override void Execute(TreeDataByteSequence self, ref object obj)
			{
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(T[,])))
				{
					//读取指针回退，类型码
					self.ReadBack(8);
					//跳跃数据
					self.SkipData();
					return;
				}

				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					obj = null;
					return;
				}
				count = ~count;
				if (count != 2)
				{
					//读取指针回退
					self.ReadBack(4);
					self.SkipData(dataType);
					return;
				}

				int dim1 = self.ReadUnmanaged<int>();
				int dim2 = self.ReadUnmanaged<int>();

				//假如数组为空或长度不一致，那么重新分配
				if (obj == null || ((T[,])obj).GetLength(0) != dim1 || ((T[,])obj).GetLength(1) != dim2)
				{
					obj = new T[dim1, dim2];
				}

				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim2 * size;
					long totalSize = (long)dim1 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性读取整个数组数据
						ref byte spanRef = ref self.GetReadRefByte((int)totalSize);
						ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,])obj)[0, 0]);
						Unsafe.CopyBlockUnaligned(ref dst, ref spanRef, (uint)totalSize);
					}
					else
					{
						// 分批读取数组数据
						for (int i = 0; i < dim1; i++)
						{
							ref byte spanRef = ref self.GetReadRefByte(elementSize);
							ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,])obj)[i, 0]);
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
							self.ReadValue(ref ((T[,])obj)[i, j]);
						}
					}
				}
			}
		}
	}
}
