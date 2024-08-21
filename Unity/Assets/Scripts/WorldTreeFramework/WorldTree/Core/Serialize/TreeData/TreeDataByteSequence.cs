/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	public static class KeyValuePairFormatterRule
	{
		class Serialize<TKey, TValue> : TreeDataSerializeRule<TreeDataByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
				{
					self.DangerousWriteUnmanaged(value);
					return;
				}
				//self.WriteValue(value.Key);
				//self.WriteValue(value.Value);
			}
		}
		class Deserialize<TKey, TValue> : TreeDataDeserializeRule<TreeDataByteSequence, KeyValuePair<TKey, TValue>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				if (!RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
				{
					self.DangerousReadUnmanaged(out value);
					return;
				}
				value = new KeyValuePair<TKey, TValue>(
				   //self.ReadValue<TKey>(),
				   //self.ReadValue<TValue>()
				);
			}
		}
	}




	public static class TreeDataByteSequenceRule
	{



		class AddRule : AddRule<TreeDataByteSequence>
		{
			protected override void Execute(TreeDataByteSequence self)
			{
				self.GetBaseRule<TreeDataByteSequence, ByteSequence, Add>().Send(self);

				self.Core.PoolGetUnit(out self.typeCodeToTypeNameDict);
				self.Core.PoolGetUnit(out self.typeNameToCodeDict);
			}
		}

	}

	/// <summary>
	/// 树数据字节序列
	/// </summary>
	public class TreeDataByteSequence : ByteSequence
		, AsRule<ITreeDataSerialize>
		, AsRule<ITreeDataDeserialize>
	{
		/// <summary>
		/// 类型码对应名称字典
		/// </summary>
		public UnitDictionary<long, string> typeCodeToTypeNameDict;
		/// <summary>
		/// 类型名称对应码字典
		/// </summary>
		public UnitDictionary<string, long> typeNameToCodeDict;


	}
}
