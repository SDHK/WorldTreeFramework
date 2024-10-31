


using System.Collections.Generic;
using System;

namespace WorldTree.TreeDataFormatters
{

	/// <summary>
	/// 枚举类型特殊格式化器
	/// </summary>
	public static class IEnumerableSpecialFormatterRule
	{
		/// <summary>
		/// 枚举类型特殊序列化法则基类
		/// </summary>
		public abstract class SerializeBase<T, ItemT> : TreeDataSerializeRule<T>
			where T : class, IEnumerable<ItemT>, new()
		{
			/// <summary>
			/// 遍历写入方法
			/// </summary>
			public abstract void ForeachWrite(TreeDataByteSequence self, T obj);

			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				T obj = (T)value;
				if (nameCode == -1)
				{
					self.WriteType(typeof(T));
					if (obj == null)
					{
						self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);
						return;
					}
					//写入字段数量
					self.WriteUnmanaged(1);
				}

				//假设字典有一个数组字段
				if (!self.WriteCheckNameCode(1683726967)) self.AddNameCode(1683726967, nameof(self));

				//序列化数组字段
				self.WriteType(typeof(ItemT[]));
				//写入数组维度数量
				self.WriteUnmanaged(~1);
				ForeachWrite(self, obj);
			}
		}

		/// <summary>
		/// 枚举类型特殊反序列化法则基类
		/// </summary>
		public abstract class DeserializeBase<T, ItemT> : TreeDataDeserializeRule<T>
			where T : class, IEnumerable<ItemT>, new()
		{
			/// <summary>
			/// 遍历读取方法
			/// </summary>
			public abstract void ForeachRead(TreeDataByteSequence self, T obj);

			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (nameCode != -1)
				{
					SwitchRead(self, ref value, nameCode);
					return;
				}
				var targetType = typeof(T);
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(T)))
				{
					//跳跃数据,子类读取数据
					self.SubTypeReadValue(dataType, targetType, ref value);
					return;
				}
				//读取字段数量
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				if (count < 0)
				{
					self.ReadBack(4);
					self.SkipData(dataType);
					return;
				}
				if (value is not T) value = new T();
				for (int i = 0; i < count; i++)
				{
					self.ReadUnmanaged(out nameCode);
					SwitchRead(self, ref value, nameCode);
				}
			}

			/// <summary>
			/// 字段读取
			/// </summary>
			private void SwitchRead(TreeDataByteSequence self, ref object value, int nameCode)
			{
				if (value is not T obj) return;
				switch (nameCode)
				{
					case 1683726967:
						{
							//反序列化数组
							if (!(self.TryReadType(out Type dataType) && dataType == typeof(ItemT[])))
							{
								//跳跃数据
								self.SkipData(dataType);
								return;
							}
							//读取数组维度数量,如果是null则会在前面挡掉,所以不用判断
							self.ReadUnmanaged(out int _);
							ForeachRead(self, obj);
						}
						break;
					default: self.SkipData(); break;
				}
			}
		}
	}
}