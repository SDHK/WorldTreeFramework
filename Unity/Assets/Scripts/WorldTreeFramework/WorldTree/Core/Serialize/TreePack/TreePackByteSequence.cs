/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
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

	}

	/// <summary>
	/// 树包字节序列
	/// </summary>
	public class TreePackByteSequence : ByteSequence
	{
		#region 读取

		/// <summary>
		/// 写入值
		/// </summary>
		public void WriteValue<T>(in T value)
		{
			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackSerialize<>));
			if (ruleDict.TryGetValue(TypeInfo<TreePackSerialize<T>>.TypeCode, out RuleList ruleList))
				((IRuleList<TreePackSerialize<T>>)ruleList).SendRef(this, ref Unsafe.AsRef(value));
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

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackSerialize<>));
			if (ruleDict.TryGetValue(TypeInfo<TreePackSerialize<T>>.TypeCode, out RuleList ruleList))
			{
				IRuleList<TreePackSerialize<T>> ruleListT = ruleList;
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
		public void ReadValue<T>(ref T value)
		{
			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackDeserialize<>));
			if (ruleDict.TryGetValue(TypeInfo<TreePackDeserialize<T>>.TypeCode, out RuleList ruleList))
				((IRuleList<TreePackDeserialize<T>>)ruleList).SendRef(this, ref value);
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

			Core.RuleManager.SupportGenericRule<T>(typeof(TreePackDeserialize<>));
			if (ruleDict.TryGetValue(TypeInfo<TreePackDeserialize<T>>.TypeCode, out RuleList ruleList))
			{
				IRuleList<TreePackDeserialize<T>> ruleListT = ruleList;
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
