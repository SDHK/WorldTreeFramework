using System.Collections.Generic;
using System;
using System.Collections;

namespace WorldTree.TreeDataFormatters
{
	public static class DictionaryFormatterRule
	{
		private class Serialize<TKey, TValue> : TreeDataSerializeRule<Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				Dictionary<TKey, TValue> obj = (Dictionary<TKey, TValue>)value;

				self.WriteType(typeof(Dictionary<TKey, TValue>));
				if (obj == null)
				{
					self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);
					return;
				}
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

		private class Deserialize<TKey, TValue> : TreeDataDeserializeRule<Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				var targetType = typeof(Dictionary<TKey, TValue>);
				if (!(self.TryReadType(out Type dataType) && dataType == typeof(Dictionary<TKey, TValue>)))
				{
					//跳跃数据,子类读取数据
					self.SubTypeReadValue(dataType, targetType, ref value);
					return;
				}
				//读取数组维度数量
				self.ReadUnmanaged(out int count);
				if (count == ValueMarkCode.NULL_OBJECT)
				{
					value = null;
					return;
				}
				count = ~count;
				if (count != 1)
				{
					//读取指针回退
					self.ReadBack(4);
					self.SkipData(dataType);
					return;
				}

				self.ReadUnmanaged(out int length);

				//假如数组为空或长度不一致
				Dictionary<TKey, TValue> obj = value as Dictionary<TKey, TValue>;
				if (obj == null)
				{
					value = obj = new();
				}
				else if (obj.Count != 0)
				{
					obj.Clear();
				}

				//数据长度为0，直接返回
				if (length == 0) return;

				KeyValuePair<TKey, TValue> keyValuePair;
				//读取数组数据
				for (int i = 0; i < length; i++)
				{
					keyValuePair = self.ReadValue<KeyValuePair<TKey, TValue>>();
					obj.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}