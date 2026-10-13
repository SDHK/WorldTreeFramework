/****************************************

* 作者：闪电黑客
* 日期：2024/8/15 15:17

* 描述：

*/
using System.Collections.Generic;

namespace WorldTree.TreePackFormats
{
	public static class DictionaryFormatRule
	{
		class Serialize<TKey, TValue> : TreePackSerializeRule<IDictionary<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref IDictionary<TKey, TValue> value)
			{
				if (value == null)
				{
					self.WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
					return;
				}
				else
				{
					self.WriteUnmanaged(value.Count);
				}
				foreach (KeyValuePair<TKey, TValue> item in value) self.WriteValue(item);
			}
		}
		class Deserialize<TKey, TValue> : TreePackDeserializeRule<IDictionary<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref IDictionary<TKey, TValue> value)
			{
				if (self.ReadUnmanaged(out int tagCount) == ValueMarkCode.NULL_OBJECT)
				{
					value = default;
					return;
				}
				Dictionary<TKey, TValue> valueDict = new();
				KeyValuePair<TKey, TValue> keyValuePair = default;
				for (int i = 0; i < tagCount; i++)
				{
					self.ReadValue(ref keyValuePair);
					valueDict.Add(keyValuePair.Key, keyValuePair.Value);
				}
				value = valueDict;
			}
		}
	}
}
