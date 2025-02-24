/****************************************

* 作者：闪电黑客
* 日期：2024/10/23 18:27

* 描述：

*/


using System.Collections.Generic;

namespace WorldTree.TreeDataFormatters
{
	public static class KeyValuePairFormatterRule
	{
		class Serialize<TKey, TValue> : TreeDataSerializeRule<KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out KeyValuePair<TKey, TValue> obj)) return;
				self.WriteDynamic(2);
				self.WriteValue(obj.Key);
				self.WriteValue(obj.Value);

				//Key -853882612
				//Value -783812246
			}
		}

		class Deserialize<TKey, TValue> : TreeDataDeserializeRule<KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int fieldNameCode)
			{
				if (self.TryReadArrayHead(typeof(KeyValuePair<TKey, TValue>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new KeyValuePair<TKey, TValue>(
					self.ReadValue<TKey>(),
					self.ReadValue<TValue>()
				);
			}
		}
	}
}