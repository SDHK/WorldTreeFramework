/****************************************

* 作者：闪电黑客
* 日期：2024/10/9 14:52

* 描述：

*/
using System.Runtime.CompilerServices;

namespace WorldTree.TreeDataFormatters
{
	public static class Array3FormatterRule
	{
		/// <summary>
		/// 泛型三维数组序列化
		/// </summary>
		private class Serialize<T> : TreeDataSerializeRule<T[,,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryWriteDataHead(value, nameCode, ~3, out T[,,] obj)) return;

				// 写入数组维度
				int dim1 = obj.GetLength(0);
				int dim2 = obj.GetLength(1);
				int dim3 = obj.GetLength(2);
				self.WriteUnmanaged(dim1);
				self.WriteUnmanaged(dim2);
				self.WriteUnmanaged(dim3);

				//判断是否为基础类型
				if (TreeDataType.TypeSizeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim3 * size;
					long totalSize = (long)dim1 * dim2 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性写入整个数组数据
						ref byte spanRef = ref self.GetWriteRefByte((int)totalSize);
						ref byte src = ref Unsafe.As<T, byte>(ref obj[0, 0, 0]);
						Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)totalSize);
					}
					else
					{
						// 分批写入数组数据
						for (int i = 0; i < dim1; i++)
						{
							for (int j = 0; j < dim2; j++)
							{
								ref byte spanRef = ref self.GetWriteRefByte(elementSize);
								ref byte src = ref Unsafe.As<T, byte>(ref obj[i, j, 0]);
								Unsafe.CopyBlockUnaligned(ref spanRef, ref src, (uint)elementSize);
							}
						}
					}
				}
				else //当成托管类型处理
				{
					for (int i = 0; i < dim1; i++)
					{
						for (int j = 0; j < dim2; j++)
						{
							for (int k = 0; k < dim3; k++)
							{
								self.WriteValue(obj[i, j, k]);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 泛型三维数组反序列化
		/// </summary>
		private class Deserialize<T> : TreeDataDeserializeRule<T[,,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadArrayHead(typeof(T[,,]), ref value, 3)) return;

				self.ReadDynamic(out int dim1);
				self.ReadDynamic(out int dim2);
				self.ReadDynamic(out int dim3);

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[,,])value).GetLength(0) != dim1 || ((T[,,])value).GetLength(1) != dim2 || ((T[,,])value).GetLength(2) != dim3)
				{
					value = new T[dim1, dim2, dim3];
				}

				if (TreeDataType.TypeSizeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim3 * size;
					long totalSize = (long)dim1 * dim2 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性读取整个数组数据
						ref byte spanRef = ref self.GetReadRefByte((int)totalSize);
						ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,,])value)[0, 0, 0]);
						Unsafe.CopyBlockUnaligned(ref dst, ref spanRef, (uint)totalSize);
					}
					else
					{
						// 分批读取数组数据
						for (int i = 0; i < dim1; i++)
						{
							for (int j = 0; j < dim2; j++)
							{
								ref byte spanRef = ref self.GetReadRefByte(elementSize);
								ref byte dst = ref Unsafe.As<T, byte>(ref ((T[,,])value)[i, j, 0]);
								Unsafe.CopyBlockUnaligned(ref dst, ref spanRef, (uint)elementSize);
							}
						}
					}
				}
				else //当成托管类型处理
				{
					for (int i = 0; i < dim1; i++)
					{
						for (int j = 0; j < dim2; j++)
						{
							for (int k = 0; k < dim3; k++)
							{
								self.ReadValue(ref ((T[,,])value)[i, j, k]);
							}
						}
					}
				}
			}
		}
	}
}
