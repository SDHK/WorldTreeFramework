using System.Collections.Generic;
using System;
using System.Collections;

namespace WorldTree.TreeDataFormatters
{

	public static class DictionaryFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<TKey, TValue> : TreeDataSerializeRule<Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				Dictionary<TKey, TValue> obj = (Dictionary<TKey, TValue>)value;
				if (nameCode == -1)
				{
					self.WriteType(typeof(Dictionary<TKey, TValue>));
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
				self.WriteType(typeof(KeyValuePair<TKey, TValue>[]));
				//写入数组维度数量
				self.WriteUnmanaged(~1);
				self.WriteUnmanaged(obj.Count);
				if (obj.Count == 0) return;

				//写入数组数据
				foreach (var item in obj)
				{
					self.WriteValue(item);
				}
			}
		}

		private  class Deserialize<TKey, TValue> : TreeDataDeserializeRule<Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (nameCode != -1)
				{
					SwitchRead(self, ref value, nameCode);
					return;
				}
				var targetType = typeof(Dictionary<TKey, TValue>);
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Dictionary<TKey, TValue>)))
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
				if (value is not Dictionary<TKey, TValue>) value = new Dictionary<TKey, TValue>();
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
				if (value is not Dictionary<TKey, TValue> obj) return;
				switch (nameCode)
				{
					case 1683726967:
						{
							//反序列化数组
							if (!(self.TryReadType(out Type dataType) && dataType == typeof(KeyValuePair<TKey, TValue>[])))
							{
								//跳跃数据
								self.SkipData(dataType);
								return;
							}
							//读取数组维度数量,如果是null则会在前面挡掉,所以不用判断
							self.ReadUnmanaged(out int _);
							self.ReadUnmanaged(out int length);
							//假如有数据
							if (obj.Count != 0) obj.Clear();
							//数据长度为0，直接返回
							if (length == 0) return;
							KeyValuePair<TKey, TValue> keyValuePair;
							//读取数组数据
							for (int j = 0; j < length; j++)
							{
								keyValuePair = self.ReadValue<KeyValuePair<TKey, TValue>>();
								obj.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
						break;
					default: self.SkipData(); break;
				}
			}
		}
	}
}