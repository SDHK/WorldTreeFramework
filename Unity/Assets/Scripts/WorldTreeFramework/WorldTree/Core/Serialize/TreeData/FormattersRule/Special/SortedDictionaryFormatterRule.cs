/****************************************

* 作者： 闪电黑客
* 日期： 2025/2/12 11:11

* 描述： 

*/
using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class SortedDictionaryFormatterRule
	{
		[TreeDataSpecial(1)]
		private class Serialize<TKey, TValue> : IEnumerableSpecialFormatterRule.SerializeBase<SortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
		{
			public override void ForeachWrite(TreeDataByteSequence self, SortedDictionary<TKey, TValue> obj)
			{
				self.WriteDynamic(obj.Count);
				foreach (var item in obj) self.WriteValue(item);
			}
		}

		private class Deserialize<TKey, TValue> : IEnumerableSpecialFormatterRule.DeserializeBase<SortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
		{
			public override void ForeachRead(TreeDataByteSequence self, SortedDictionary<TKey, TValue> obj)
			{
				if (obj.Count != 0) obj.Clear();
				self.ReadDynamic(out int length);
				KeyValuePair<TKey, TValue> keyValuePair;
				for (int j = 0; j < length; j++)
				{
					keyValuePair = self.ReadValue<KeyValuePair<TKey, TValue>>();
					obj.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}