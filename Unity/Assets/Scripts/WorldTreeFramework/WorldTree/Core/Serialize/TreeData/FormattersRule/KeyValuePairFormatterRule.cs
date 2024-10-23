

using System.Collections.Generic;
using System;

namespace WorldTree.TreeDataFormatters
{
	public static class KeyValuePairFormatterRule
	{
		class Serialize<TKey, TValue> : TreeDataSerializeRule<TreeDataByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				KeyValuePair<TKey, TValue> data = (KeyValuePair<TKey, TValue>)value;
				self.WriteType(typeof(KeyValuePair<TKey, TValue>));
				self.WriteUnmanaged(2);
				if (!self.WriteCheckNameCode(-853882612)) self.AddNameCode(-853882612, nameof(data.Key));
				self.WriteValue(data.Key);
				if (!self.WriteCheckNameCode(-783812246)) self.AddNameCode(-783812246, nameof(data.Value));
				self.WriteValue(data.Value);
			}
		}

		class Deserialize<TKey, TValue> : TreeDataDeserializeRule<TreeDataByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				var targetType = typeof(KeyValuePair<TKey, TValue>);
				if (!(self.TryReadType(out Type type) && type == targetType))
				{
					self.SkipData(type);
					return;
				}
				self.ReadUnmanaged(out int count);
				if (count < 0)
				{
					self.ReadBack(4);
					self.SkipData(type);
					return;
				}
				TKey key = default;
				TValue val = default;
				for (int i = 0; i < count; i++)
				{
					self.ReadUnmanaged(out int nameCode);

					switch (nameCode)
					{
						case -853882612: key = self.ReadValue<TKey>(); break;
						case -783812246: val = self.ReadValue<TValue>(); break;
						default: self.SkipData(); break;
					}
				}
				value = new KeyValuePair<TKey, TValue>(key, val);
			}
		}
	}
}