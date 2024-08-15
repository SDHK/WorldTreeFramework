using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree.TreePack.Formatters
{
	public static class KeyValuePairFormatterRule
	{
		class Serialize<TKey, TValue> : TreePackSerializeRule<TreePackByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref KeyValuePair<TKey, TValue> value)
			{
				if (!RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
				{
					self.DangerousWriteUnmanaged(value);
					return;
				}
				self.WriteValue(value.Key);
				self.WriteValue(value.Value);
			}
		}
		class Deserialize<TKey, TValue> : TreePackDeserializeRule<TreePackByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreePackByteSequence self, ref KeyValuePair<TKey, TValue> value)
			{
				if (!RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
				{
					self.DangerousReadUnmanaged(out value);
					return;
				}
				value = new KeyValuePair<TKey, TValue>(
				   self.ReadValue<TKey>(),
				   self.ReadValue<TValue>()
				);
			}
		}
	}
}
