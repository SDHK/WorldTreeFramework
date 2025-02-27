﻿/****************************************

* 作者：闪电黑客
* 日期：2024/8/15 14:17

* 描述：

*/
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree.TreePackFormats
{
	public static class KeyValuePairFormatRule
	{
		class Serialize<TKey, TValue> : TreePackSerializeRule<KeyValuePair<TKey, TValue>>
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
		class Deserialize<TKey, TValue> : TreePackDeserializeRule<KeyValuePair<TKey, TValue>>
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
