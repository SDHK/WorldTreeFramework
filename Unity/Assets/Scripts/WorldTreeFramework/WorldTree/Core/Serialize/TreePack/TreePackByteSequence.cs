/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	public static partial class TreePackByteSequenceRule
	{
		/// <summary>
		/// 序列化写入类型
		/// </summary>
		public static void Serialize<T>(this TreePackByteSequence self, in T value)
		{
			self.WriteValue(value);
		}

		/// <summary>
		/// 反序列化读取类型
		/// </summary>
		public static void Deserialize<T>(this TreePackByteSequence self, ref T value)
		{
			self.ReadValue(ref value);
		}

		class AddRule : AddRule<TreePackByteSequence>
		{
			protected override void Execute(TreePackByteSequence self)
			{
				self.GetBaseRule<TreePackByteSequence, ByteSequence, Add>().Send(self);

				// 获取节点的法则集
				self.Core.RuleManager.NodeTypeRulesDict.TryGetValue(self.Type, out self.unmanagedRuleDict);
				self.Core.RuleManager.TryGetRuleGroup<ITreePackSerialize>(out self.serializeRuleDict);
				self.Core.RuleManager.TryGetRuleGroup<ITreePackDeserialize>(out self.deserializeRuleDict);
			}
		}
	}

	/// <summary>
	/// 树包字节序列
	/// </summary>
	public class TreePackByteSequence : ByteSequence
		, AsRule<ITreePackSerialize>
		, AsRule<ITreePackDeserialize>
	{
		/// <summary>
		/// 自身拥有的非委托类型法则列表集合
		/// </summary>
		/// <remarks> RuleTypeCode, 法则列表 </remarks>
		public Dictionary<long, RuleList> unmanagedRuleDict;

		/// <summary>
		/// 不同类型序列化法则列表集合
		/// </summary>
		public RuleGroup serializeRuleDict;

		/// <summary>
		/// 不同类型反序列化法则列表集合
		/// </summary>
		public RuleGroup deserializeRuleDict;


		#region 读取

		/// <summary>
		/// 写入值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value)
		{
			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);
			if (serializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				foreach (IRule rule in ruleList)
				{
					Unsafe.As<TreePackSerializeRule<TreePackByteSequence, T>>(rule).Invoke(this, ref Unsafe.AsRef(value));
				}
				return;
			}

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackSerializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackSerializeUnmanaged<T>>(), out ruleList))
				((IRuleList<TreePackSerializeUnmanaged<T>>)ruleList).SendRef(this, ref Unsafe.AsRef(value));
		}


		/// <summary>
		/// 写入数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteArray<T>(T[] value)
		{
			if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				DangerousWriteUnmanagedArray(value);
				return;
			}
			if (value == null)
			{
				WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
				return;
			}

			WriteUnmanaged(value.Length);

			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);
			if (serializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				for (int i = 0; i < value.Length; i++)
				{
					foreach (IRule rule in ruleList)
					{
						Unsafe.As<TreePackSerializeRule<TreePackByteSequence, T>>(rule).Invoke(this, ref value[i]);
					}
				}
				return;
			}

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackSerializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackSerializeUnmanaged<T>>(), out ruleList))
			{
				IRuleList<TreePackSerializeUnmanaged<T>> ruleListT = ruleList;
				for (int i = 0; i < value.Length; i++) ruleListT.SendRef(this, ref value[i]);
			}
		}

		/// <summary>
		/// 写入非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUnmanagedArray<T>(T[] value)
			where T : unmanaged
		{
			DangerousWriteUnmanagedArray(value);
		}
		#endregion


		#region 读取

		/// <summary>
		/// 读取值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(ref T value)
		{
			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);
			if (deserializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				foreach (IRule rule in ruleList)
				{
					Unsafe.As<TreePackDeserializeRule<TreePackByteSequence, T>>(rule).Invoke(this, ref value);
				}
				return;
			}

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackDeserializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackDeserializeUnmanaged<T>>(), out ruleList))
				((IRuleList<TreePackDeserializeUnmanaged<T>>)ruleList).SendRef(this, ref value);
		}

		/// <summary>
		/// 读取值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ReadValue<T>()
		{
			T value = default;
			ReadValue(ref value);
			return value;
		}


		/// <summary>
		/// 读取数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadArray<T>(ref T[] value)
		{
			if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				DangerousReadUnmanagedArray(ref value);
				return;
			}

			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
			{
				value = null;
				return;
			}
			else if (length == 0)
			{
				value = Array.Empty<T>();
				return;
			}
			else if (ReadRemain < length)
			{
				this.LogError($"数组长度超出数据长度: {length}.");
				value = null;
				return;
			}

			//假如数组为空或长度不一致，那么重新分配
			if (value == null || value.Length != length)
			{
				value = new T[length];
			}
			long typeCode = Core.TypeToCode<T>();
			Core.RuleManager.SupportNodeRule(typeCode);
			if (deserializeRuleDict.TryGetValue(typeCode, out RuleList ruleList))
			{
				for (int i = 0; i < value.Length; i++)
				{
					foreach (IRule rule in ruleList)
					{
						Unsafe.As<TreePackDeserializeRule<TreePackByteSequence, T>>(rule).Invoke(this, ref value[i]);
					}
				}
				return;
			}

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackDeserializeUnmanaged<>));
			if (unmanagedRuleDict.TryGetValue(Core.TypeToCode<TreePackDeserializeUnmanaged<T>>(), out ruleList))
			{
				IRuleList<TreePackDeserializeUnmanaged<T>> ruleListT = ruleList;
				for (int i = 0; i < length; i++) ruleListT.SendRef(this, ref value[i]);
			}
		}

		/// <summary>
		/// 读取非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadUnmanagedArray<T>(ref T[] value)
			where T : unmanaged
		{
			DangerousReadUnmanagedArray(ref value);
		}

		#endregion
	}

}
