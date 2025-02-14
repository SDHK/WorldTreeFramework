/****************************************

* 作者：闪电黑客
* 日期：2024/10/15 21:07

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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


		#region 写入

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

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteString(string value)
		{
			if (Utf8)
				WriteUtf8(value);
			else
				WriteUtf16(value);
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf16(string value)
		{
			if (value == null)
			{
				WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			//获取字符串长度,因为 UTF-16 编码的每个字符占用 2 个字节，checked 防止溢出int值
			var copyByteCount = checked(value.Length * 2);
			//这行代码获取一个引用，指向一个足够大的缓冲区，以容纳字符串的字节数和额外的 4 个字节。
			ref byte dest = ref GetWriteRefByte(copyByteCount + 4);
			//这行代码将字符串的长度（以字节为单位）写入缓冲区的前 4 个字节
			Unsafe.WriteUnaligned(ref dest, value.Length * 2);
			//这行代码将字符串的实际字节数据复制到缓冲区中，跳过前 4 个字节
			MemoryMarshal.AsBytes(value.AsSpan()).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dest, 4), copyByteCount));
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf8(string value)
		{
			if (value == null)
			{
				WriteUnmanaged(ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			// (int utf16-length, int utf8-byte-count, utf8-bytes)
			ReadOnlySpan<char> source = value.AsSpan();

			// 由于不知道空间大小，所以字符数*3只是获取一个可能的最大空间，字符最小可能是只占1个字节
			int maxByteCount = (source.Length + 1) * 3;

			//申请总空间，包含utf8长度和数据

			// 头部需要写入byte真实长度，int长度偏移+4
			ref byte destPointer = ref GetWriteRefByte(maxByteCount + 4);

			//申请数据空间，byte长度int要写到头部，所以要偏移4
			Span<byte> dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 4), maxByteCount);

			// 数据写入到dest，此时拿到了byte的真实长度
			int bytesWritten = Encoding.UTF8.GetBytes(value, dest);

			//~0 的结果是 -1，但前面if挡住了，所以不会出现-1，所以可以用来区分8位和16位
			Unsafe.WriteUnaligned(ref destPointer, bytesWritten);

			// 重新定位指针，裁剪空间
			WriteBack(maxByteCount - bytesWritten);
		}

		/// <summary>
		/// 危险写入非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DangerousWriteUnmanagedArray<T>(T[] value)
		{
			if (value == null)
			{
				WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			//获取数组数据长度
			var srcLength = Unsafe.SizeOf<T>() * value.Length;
			//包含数组数量的总长度
			var allocSize = srcLength + Unsafe.SizeOf<int>();

			//获取写入操作跨度
			ref byte spanRef = ref GetWriteRefByte(allocSize);

			//获取数组数据的指针
			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));

			//写入数组长度
			Unsafe.WriteUnaligned(ref spanRef, value.Length);
			//写入数组数据
			Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<int>()), ref src, (uint)srcLength);
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


		/// <summary>
		/// 读取字符串
		/// </summary>
		public string ReadString()
		{
			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT) return null;
			else if (length == 0) return string.Empty;
			if (Utf8)
				return ReadUtf8(length);
			else
				return ReadUtf16(length);
		}

		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadUtf16(int length)
		{
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}
			ref byte src = ref GetReadRefByte(length);
			return new string(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, char>(ref src), (int)(length * 0.5f)));
		}


		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public string ReadUtf8(int length)
		{
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}
			ref var spanRef = ref GetReadRefByte(length);
			return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref spanRef, length));
		}

		/// <summary>
		/// 危险读取非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void DangerousReadUnmanagedArray<T>(ref T[] value)
		{
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

			var byteCount = length * Unsafe.SizeOf<T>();
			ref byte spanRef = ref GetReadRefByte(byteCount);

			//假如数组为空或长度不一致，那么重新分配
			if (value == null || value.Length != length) value = new T[length];

			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));
			Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
		}

		#endregion
	}

}
