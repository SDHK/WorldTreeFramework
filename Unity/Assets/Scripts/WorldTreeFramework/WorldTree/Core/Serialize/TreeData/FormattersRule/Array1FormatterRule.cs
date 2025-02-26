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
				Type type = typeof(T);
				T[] obj;
				//判断是否是枚举类型
				if (type.IsEnum)
				{
					type = Enum.GetUnderlyingType(type);
					if (self.TryWriteDataHead(value, SerializedTypeMode.DataType, ~1, out obj, true, true, type.MakeArrayType())) return;
				}
				//判断是否为基础类型，基础类型需要写入完整数组类型
				else if (typeMode == SerializedTypeMode.ObjectType && TreeDataTypeHelper.TypeSizeDict.ContainsKey(type))
				{
					if (self.TryWriteDataHead(value, SerializedTypeMode.DataType, ~1, out obj, true, true)) return;
				}
				else
				{
					//写入为数组类型
					if (self.TryWriteDataHead(value, typeMode, ~1, out obj, false, true)) return;
				}

				//写入数组数据长度
				self.WriteDynamic(obj.Length);
				if (obj.Length == 0) return;

				//判断是否为基础类型
				if (TreeDataTypeHelper.TypeSizeDict.TryGetValue(type, out int size))
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
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(T[]), ref value, 1, out int objId, out int jumpReadPoint)) return;

				self.ReadDynamic(out int length);
				if (length == 0)
				{
					value = Array.Empty<T>();
					if (jumpReadPoint != TreeDataCode.NULL_OBJECT) self.ReadJump(jumpReadPoint);
					return;
				}

				//假如数组为空或长度不一致，那么重新分配
				if (value == null || ((T[])value).Length != length) value = new T[length];
				if (objId != TreeDataCode.NULL_OBJECT) self.IdToObjectDict.Add(objId, value);

				Type type = typeof(T);
				if (type.IsEnum) type = Enum.GetUnderlyingType(type);
				if (TreeDataTypeHelper.TypeSizeDict.TryGetValue(type, out int size))
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
				if (jumpReadPoint != TreeDataCode.NULL_OBJECT) self.ReadJump(jumpReadPoint);
			}
		}
	}
}
