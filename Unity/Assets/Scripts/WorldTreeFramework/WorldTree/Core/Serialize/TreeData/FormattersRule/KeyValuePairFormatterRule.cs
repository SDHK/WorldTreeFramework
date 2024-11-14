

using System.Collections.Generic;
using System;

namespace WorldTree.TreeDataFormatters
{
	public static class KeyValuePairFormatterRule
	{
		class Serialize<TKey, TValue> : TreeDataSerializeRule<KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryWriteDataHead(value, nameCode, 2, out KeyValuePair<TKey, TValue> obj)) return;
				self.WriteUnmanaged(-853882612);
				self.WriteValue(obj.Key);
				self.WriteUnmanaged(-783812246);
				self.WriteValue(obj.Value);
			}
		}

		class Deserialize<TKey, TValue> : TreeDataDeserializeRule<KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadDataHead(typeof(KeyValuePair<TKey, TValue>), ref value, out int count)) return;
				if (self.CheckClassCount(count)) return;

				TKey key = default;
				TValue val = default;
				for (int i = 0; i < count; i++)
				{
					self.ReadUnmanaged(out nameCode);

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