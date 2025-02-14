/****************************************

* 作者：闪电黑客
* 日期：2024/11/4 10:07

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{

	/// <summary>
	/// 迭代器类型特殊格式化器
	/// </summary>
	public static class IEnumerableSpecialFormatterRule
	{
		/// <summary>
		/// 迭代器类型特殊序列化法则基类
		/// </summary>
		public abstract class SerializeBase<T, ItemT> : TreeDataSerializeRule<T>
			where T : class, IEnumerable<ItemT>, new()
		{
			/// <summary>
			/// 遍历写入方法
			/// </summary>
			public abstract void ForeachWrite(TreeDataByteSequence self, T obj);

			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, 1, out T obj)) return;

				//迭代器类型可能有字段，所以不能当做数组类型储存，而是只能假设自己有一个数组字段进行储存
				self.WriteUnmanaged(1683726967);
				//序列化数组字段
				self.WriteType(typeof(object));
				//写入数组维度数量
				self.WriteDynamic(~1);
				ForeachWrite(self, obj);
			}
		}

		/// <summary>
		/// 迭代器类型特殊反序列化法则基类
		/// </summary>
		public abstract class DeserializeBase<T, ItemT> : TreeDataDeserializeRule<T>
			where T : class, IEnumerable<ItemT>, new()
		{
			/// <summary>
			/// 遍历读取方法
			/// </summary>
			public abstract void ForeachRead(TreeDataByteSequence self, T obj);

			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (fieldNameCode != TreeDataCode.DESERIALIZE_SELF_MODE)
				{
					SwitchRead(self, ref value, fieldNameCode);
					return;
				}
				if (self.TryReadClassHead(typeof(T), ref value, out int count)) return;

				if (value is not T) value = new T();
				for (int i = 0; i < count; i++)
				{
					self.ReadUnmanaged(out fieldNameCode);
					SwitchRead(self, ref value, fieldNameCode);
				}
			}

			/// <summary>
			/// 字段读取
			/// </summary>
			private void SwitchRead(TreeDataByteSequence self, ref object value, int fieldNameCode)
			{
				if (value is not T obj) return;
				switch (fieldNameCode)
				{
					case 1683726967:
						{
							//反序列化数组
							if (self.TryReadArrayHead(typeof(ItemT[]), ref value, 1)) return;
							ForeachRead(self, obj);
						}
						break;
					default: self.SkipData(); break;
				}
			}
		}
	}
}