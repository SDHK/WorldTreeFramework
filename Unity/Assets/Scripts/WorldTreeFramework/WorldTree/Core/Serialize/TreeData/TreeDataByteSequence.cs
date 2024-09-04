/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// data
	/// </summary>
	public class AData
	{
		/// <summary>
		/// a
		/// </summary>
		public int AInt;
	}


	public static class KeyValuePairFormatterRule
	{
		class Serialize : TreeDataSerializeRule<TreeDataByteSequence, AData>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value)
			{
				//记录字段名称，类型名称，最后是数据
				//名称应该要转为数字码进行储存

				AData data = (AData)value;
				string className = "WorldTree.AData";
				int mAIntType = 1;
				string mAInt = nameof(data.AInt);

				self.WriteUnmanaged(mAIntType);
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


		//public void

		/// <summary>
		/// 写入值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public void WriteValue<T>(in T value)
		{
			WriteValue(typeof(T), ref Unsafe.AsRef<object>(value));
		}


		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<ITreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataSerialize>)ruleList).SendRef(this, ref value);
			}
		}


		/// <summary>
		/// 读取值
		/// </summary>
		public void ReadValue<T>(ref T value)
		{
			object obj = value;
			ReadValue(typeof(T), ref obj);
			value = (T)obj;
		}

		/// <summary>
		/// 读取值
		/// </summary>
		public void ReadValue(Type type, ref object value)
		{
			long typeCode = this.Core.TypeToCode(type);
			this.Core.RuleManager.SupportNodeRule(typeCode);
			if (this.Core.RuleManager.TryGetRuleList<ITreeDataSerialize>(typeCode, out RuleList ruleList))
			{
				((IRuleList<TreeDataDeserialize>)ruleList).SendRef(this, ref value);
			}
		}
	}
}
