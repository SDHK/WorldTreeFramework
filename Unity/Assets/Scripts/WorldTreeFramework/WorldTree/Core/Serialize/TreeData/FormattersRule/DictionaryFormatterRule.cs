using System.Collections.Generic;
using System;

namespace WorldTree.TreeDataFormatters
{
	public static class DictionaryFormatterRule
	{
		private class Serialize<TKey, TValue> : TreeDataSerializeRule<TreeDataByteSequence, Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				Dictionary<TKey, TValue> dataDict = (Dictionary<TKey, TValue>)value;
				self.WriteType(typeof(Dictionary<TKey, TValue>));
				if (dataDict == null)
				{
					self.WriteUnmanaged((long)ValueMarkCode.NULL_OBJECT);
					return;
				}
				//写入数组维度数量
				self.WriteUnmanaged(~1);
				self.WriteUnmanaged(dataDict.Count);
				if (dataDict.Count == 0) return;

				//写入数组数据
				foreach (var item in dataDict)
				{
					self.WriteValue(item);
				}
			}
		}

		private class Deserialize<TKey, TValue> : TreeDataDeserializeRule<TreeDataByteSequence, Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!(self.TryReadType(out Type type) && type == typeof(Dictionary<TKey, TValue>)))
				{
					//跳跃数据
					self.SkipData(type);
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
					self.SkipData(type);
					return;
				}

				self.ReadUnmanaged(out int length);

				//假如数组为空或长度不一致
				Dictionary<TKey, TValue> dataDict = value as Dictionary<TKey, TValue>;
				if (dataDict == null)
				{
					//改用Type反射创建
					value.GetType();

					value = dataDict = new Dictionary<TKey, TValue>(length);
				}
				else if (dataDict.Count != 0)
				{
					dataDict.Clear();
				}

				//数据长度为0，直接返回
				if (length == 0) return;

				KeyValuePair<TKey, TValue> keyValuePair;
				//读取数组数据
				for (int i = 0; i < length; i++)
				{
					keyValuePair = self.ReadValue<KeyValuePair<TKey, TValue>>();
					dataDict.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}