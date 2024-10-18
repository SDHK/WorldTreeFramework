using System;
using System.Runtime.CompilerServices;

namespace WorldTree.TreeDataFormats
{
	public static class Array3FormatRule
	{
		/// <summary>
		/// 泛型三维数组序列化
		/// </summary>
		public class Serialize<T> : TreeDataSerializeRule<TreeDataByteSequence, T[,,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(T[,,]));
				T[,,] values = (T[,,])value;
				if (values == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				//写入数组维度数量
				self.WriteUnmanaged(~3);

				// 写入数组维度
				int dim1 = values.GetLength(0);
				int dim2 = values.GetLength(1);
				int dim3 = values.GetLength(2);
				self.WriteUnmanaged(dim1);
				self.WriteUnmanaged(dim2);
				self.WriteUnmanaged(dim3);

				//判断是否为基础类型
				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
				{
					int elementSize = dim3 * size;
					long totalSize = (long)dim1 * dim2 * elementSize;

					// 检查总大小是否超过 int.MaxValue
					if (totalSize <= int.MaxValue)
					{
						// 一次性写入整个数组数据
						ref byte spanRef = ref self.GetWriteRefByte((int)totalSize);
						ref byte src = ref Unsafe.As<T, byte>(ref values[0, 0, 0]);
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
								ref byte src = ref Unsafe.As<T, byte>(ref values[i, j, 0]);
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
								self.WriteValue(values[i, j, k]);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 泛型三维数组反序列化
		/// </summary>
		public class Deserialize<T> : TreeDataDeserializeRule<TreeDataByteSequence, T[,,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!(self.TryReadType(out Type type) && type == typeof(T[,,])))
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
					value = null;
					return;
				}
				count = ~count;
				if (count != 3)
				{
					//读取指针回退
					self.ReadBack(4);
					self.SkipData(type);
					return;
				}

				int dim1 = self.ReadUnmanaged<int>();
				int dim2 = self.ReadUnmanaged<int>();
				int dim3 = self.ReadUnmanaged<int>();

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[,,])value).GetLength(0) != dim1 || ((T[,,])value).GetLength(1) != dim2 || ((T[,,])value).GetLength(2) != dim3)
				{
					value = new T[dim1, dim2, dim3];
				}

				if (TreeDataType.TypeDict.TryGetValue(typeof(T), out int size))
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
