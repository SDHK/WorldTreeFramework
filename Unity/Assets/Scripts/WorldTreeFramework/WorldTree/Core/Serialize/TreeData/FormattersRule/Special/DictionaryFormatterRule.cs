using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{

	public static class DictionaryFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<TKey, TValue> : IEnumerableSpecialFormatterRule.SerializeBase<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
		{
			public override void ForeachWrite(TreeDataByteSequence self, Dictionary<TKey, TValue> obj)
			{
				self.WriteUnmanaged(obj.Count);
				if (obj.Count == 0) return;
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<TKey, TValue> : IEnumerableSpecialFormatterRule.DeserializeBase<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
		{
			public override void ForeachRead(TreeDataByteSequence self, Dictionary<TKey, TValue> obj)
			{
				//假如有数据，则清空数据
				if (obj.Count != 0) obj.Clear();

				self.ReadUnmanaged(out int length);
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
		}
	}
}