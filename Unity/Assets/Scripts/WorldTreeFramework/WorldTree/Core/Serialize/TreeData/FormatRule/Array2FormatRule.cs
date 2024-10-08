using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree.TreeDataFormats
{
	public static class Array2FormatRule
	{
		/// <summary>
		/// 托管泛型二维数组序列化
		/// </summary>
		public class Serialize<T> : TreeDataSerializeRule<TreeDataByteSequence, T[,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				self.WriteType(typeof(T[,]));
				T[,] values = (T[,])value;
				if (values == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				//写入数组维度数量
				self.WriteUnmanaged(2);

				// 写入数组维度
				int dim1 = values.GetLength(0);
				int dim2 = values.GetLength(1);
				self.WriteUnmanaged(dim1);
				self.WriteUnmanaged(dim2);

				//判断是否为基础类型
				if (TreeDataType.TypeDict.ContainsKey(typeof(T)))
				{
					int elementSize = Unsafe.SizeOf<T>();

					// 写入数组数据
					for (int i = 0; i < dim1; i++)
					{
						ref byte spanRef = ref self.GetWriteRefByte(dim2 * elementSize);
						for (int j = 0; j < dim2; j++)
						{
							ref T element = ref values[i, j];
							ref byte src = ref Unsafe.As<T, byte>(ref element);
							Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, j * elementSize), ref src, (uint)elementSize);
						}
					}
				}
				else //当成非托管类型处理
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
		/// 托管泛型二维数组反序列化
		/// </summary>
		public class Deserialize<T> : TreeDataDeserializeRule<TreeDataByteSequence, T[,]>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!(self.TryReadType(out Type type) && type == typeof(T[])))
				{
					//读取指针回退，类型码
					self.ReadBack(8);
					//跳跃数据
					self.SkipData();
					return;
				}

				if (self.ReadUnmanaged<int>() == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}

				int dim1 = self.ReadUnmanaged<int>();
				int dim2 = self.ReadUnmanaged<int>();

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[,])value).GetLength(0) != dim1 || ((T[,])value).GetLength(1) != dim2)
				{
					value = new T[dim1, dim2];
				}

				if (TreeDataType.TypeDict.ContainsKey(typeof(T)))
				{
					int elementSize = Unsafe.SizeOf<T>();

					// 读取数组数据
					for (int i = 0; i < dim1; i++)
					{
						ref byte spanRef = ref self.GetReadRefByte(dim2 * elementSize);
						for (int j = 0; j < dim2; j++)
						{
							ref T element = ref ((T[,])value)[i, j];
							ref byte dst = ref Unsafe.As<T, byte>(ref element);
							Unsafe.CopyBlockUnaligned(ref dst, ref Unsafe.Add(ref spanRef, j * elementSize), (uint)elementSize);
						}
					}
				}
				else //当成非托管类型处理
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
