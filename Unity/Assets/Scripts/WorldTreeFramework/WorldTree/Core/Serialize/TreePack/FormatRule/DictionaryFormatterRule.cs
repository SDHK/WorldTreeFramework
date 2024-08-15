﻿using System.Collections.Generic;

namespace WorldTree.TreePack.Formatters
{
	public static class DictionaryFormatterRule
	{

		class Serialize<TKey, TValue> : TreePackSerializeRule<TreePackByteSequence, Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref Dictionary<TKey, TValue> value)
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
		class Deserialize<TKey, TValue> : TreePackDeserializeRule<TreePackByteSequence, Dictionary<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref Dictionary<TKey, TValue> value)
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
