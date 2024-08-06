/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// Byte序列
	/// </summary>
	public class ByteSequence : Node
		, AsAwake
		, AsComponentBranch
		, TempOf<INode>
		, AsRule<ISerialize>
		, AsRule<IDeserialize>
	{
		/// <summary>
		/// 序列段列表
		/// </summary>
		public UnitList<ByteSequenceSegment> segmentList = null;

		/// <summary>
		/// 当前缓存
		/// </summary>
		private ByteSequenceSegment current;

		/// <summary>
		/// 自身拥有的法则列表集合
		/// </summary>
		/// <remarks> RuleTypeCode, 法则列表 </remarks>
		public Dictionary<long, RuleList> ruleDict;

		/// <summary>
		/// 数据长度
		/// </summary>
		public int Length => length;

		/// <summary>
		/// 读取片段指针位置
		/// </summary>
		public int ReadPoint => readPoint;

		/// <summary>
		/// 读取数据的剩余长度
		/// </summary>
		public int ReadRemain => length - readPoint;

		/// <summary>
		/// 数据长度
		/// </summary>
		private int length = 0;

		/// <summary>
		/// 读取片段指针
		/// </summary>
		private int readSegmentPoint = 0;

		/// <summary>
		/// 读取byte指针
		/// </summary>
		private int readBytePoint = 0;

		/// <summary>
		/// 读取总指针
		/// </summary>
		private int readPoint = 0;

		/// <summary>
		/// 租用缓冲区
		/// </summary>
		private byte[] rentBuffers;

		/// <summary>
		/// 获取写入操作跨度
		/// </summary>
		public Span<byte> GetWriteSpan(int sizeHint)
		{
			if (!current.IsNull)
			{
				// 拿到当前缓存的空白区域
				Span<byte> buffer = current.FreeSpan;
				// 如果空白区域大于等于需要的空间，那么直接返回
				if (buffer.Length > sizeHint)
				{
					current.Advance(sizeHint);
					length += sizeHint;
					return buffer;
				}
			}

			// 因为是结构体，所以需要等到缓存满了之后再添加到列表
			if (current.Length != 0) segmentList.Add(current);

			// 如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			ByteSequenceSegment next = new(sizeHint);
			current = next;
			current.Advance(sizeHint);
			length += sizeHint;
			return next.FreeSpan;
		}

		/// <summary>
		/// 添加Byte数组
		/// </summary>
		public void SetBytes(byte[] array)
		{
			if (array == null || array.Length == 0) return;
			segmentList.Add(new(array));
			length += array.Length;
		}

		/// <summary>
		/// 转换为Byte数组
		/// </summary>
		public byte[] ToBytes()
		{
			if (length == 0) return Array.Empty<byte>();
			// 是否池数组
			byte[] results = new byte[length];
			Span<byte> span = results.AsSpan();

			foreach (ByteSequenceSegment item in segmentList)
			{
				item.ByteSpan.CopyTo(span);
				span = span.Slice(item.Length);
			}
			if (!current.IsNull) current.ByteSpan.CopyTo(span);
			return results;
		}

		/// <summary>
		/// 获取读取操作跨度
		/// </summary>
		public Span<byte> GetReadSpan(int sizeHint)
		{
			if (readPoint == length) this.LogError("序列已经到达末尾");
			if (sizeHint > length - readPoint) this.LogError("超出读取长度");

			if (!current.IsNull)
			{
				segmentList.Add(current);
				current = default;
			}

			ByteSequenceSegment nowSegment = segmentList[readSegmentPoint];
			int nowRemain = nowSegment.Length - readBytePoint;
			Span<byte> span;
			if (nowRemain == sizeHint)
			{
				// 如果当前缓存的剩余空间等于需要的空间，那么直接返回
				span = nowSegment.ReadSpan(readBytePoint, sizeHint);
				readBytePoint = 0;
				readSegmentPoint++;
				readPoint += sizeHint;
				return span;
			}
			else if (nowRemain > sizeHint)
			{
				span = nowSegment.ReadSpan(readBytePoint, sizeHint);
				readBytePoint += sizeHint;
				readPoint += sizeHint;
				return span;
			}
			else //如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			{
				if (rentBuffers != null) ArrayPool<byte>.Shared.Return(rentBuffers);
				rentBuffers = ArrayPool<byte>.Shared.Rent(MathInt.GetPowerOfTwo(sizeHint));

				//先读取当前缓存的剩余空间
				span = nowSegment.ReadSpan(readBytePoint, nowRemain);
				span.CopyTo(rentBuffers);
				//计算剩余需要读取的空间
				nowRemain = sizeHint - nowRemain;
				while (nowRemain != 0)
				{
					//序列指针移动
					readSegmentPoint++;
					nowSegment = segmentList[readSegmentPoint];
					//如果当前片段的长度等于剩余需要读取的空间
					if (nowSegment.Length == nowRemain)
					{
						nowSegment.ReadSpan(0, nowRemain).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						readBytePoint = 0;
						readSegmentPoint++;
						break;
					}
					//如果当前片段的长度大于剩余需要读取的空间
					else if (nowSegment.Length > nowRemain)
					{
						nowSegment.ReadSpan(0, nowRemain).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						readBytePoint = nowRemain;
						break;
					}
					else //如果当前片段的长度小于剩余需要读取的空间
					{
						nowSegment.ReadSpan(0, nowSegment.Length).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						nowRemain -= nowSegment.Length;
					}
				}
				readPoint += sizeHint;
				return rentBuffers.AsSpan(0, sizeHint);
			}
		}

		/// <summary>
		/// 清空
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			readSegmentPoint = 0;
			readBytePoint = 0;
			readPoint = 0;
			current = default;

			if (length == 0) return;
			foreach (var item in segmentList) item.Clear();
			length = 0;
			segmentList.Clear();

			if (rentBuffers != null) ArrayPool<byte>.Shared.Return(rentBuffers);
		}

		#region 写入

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T1>(in T1 value1)
			where T1 : unmanaged
		{
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(GetWriteSpan(Unsafe.SizeOf<T1>())), value1);
		}

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T1, T2>(in T1 value1, in T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged
		{
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()));
			Unsafe.WriteUnaligned(ref spanRef, value1);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()), value2);
		}

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T1, T2, T3>(in T1 value1, in T2 value2, in T3 value3)
			where T1 : unmanaged
			where T2 : unmanaged
			where T3 : unmanaged
		{
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>()));
			Unsafe.WriteUnaligned(ref spanRef, value1);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()), value2);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()), value3);
		}

		#region Array

		/// <summary>
		/// 写入Null集合
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteNullCollectionHeader()
		=> Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(GetWriteSpan(4)), TreePackCode.NULL_COLLECTION);

		/// <summary>
		/// 写入集合头
		/// </summary>
		/// <param name="length"></param>

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteCollectionHeader(int length)
		=> Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(GetWriteSpan(4)), length);

		/// <summary>
		/// 危险写入非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DangerousWriteUnmanagedArray<T>(T[] value)
		{
			if (value == null)
			{
				WriteNullCollectionHeader();
				return;
			}
			if (value.Length == 0)
			{
				WriteCollectionHeader(0);
				return;
			}

			//获取数组数据长度
			var srcLength = Unsafe.SizeOf<T>() * value.Length;
			//包含数组数量的总长度
			var allocSize = srcLength + 4;

			//获取写入操作跨度
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(allocSize));

			//获取数组数据的指针
			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));

			//写入数组长度
			Unsafe.WriteUnaligned(ref spanRef, value.Length);
			//写入数组数据
			Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, 4), ref src, (uint)srcLength);
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
				WriteNullCollectionHeader();
				return;
			}
			WriteCollectionHeader(value.Length);

			if (ruleDict.TryGetValue(TypeInfo<Serialize<T>>.TypeCode, out RuleList ruleList))
			{
				IRuleList<Serialize<T>> ruleListT = ruleList;
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


		#endregion

		#region 读取


		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 Read<T1>()
		{
			return Unsafe.ReadUnaligned<T1>(ref MemoryMarshal.GetReference(GetReadSpan(Unsafe.SizeOf<T1>())));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 Read<T1>(out T1 value1)
		{
			return value1 = Unsafe.ReadUnaligned<T1>(ref MemoryMarshal.GetReference(GetReadSpan(Unsafe.SizeOf<T1>())));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public void Read<T1, T2>(out T1 value1, out T2 value2)
		{
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()));
			value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
			value2 = Unsafe.ReadUnaligned<T2>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public void Read<T1, T2, T3>(out T1 value1, out T2 value2, out T3 value3)
		{
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>()));
			value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
			value2 = Unsafe.ReadUnaligned<T2>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()));
			value3 = Unsafe.ReadUnaligned<T3>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()));
		}

		#region Array

		/// <summary>
		/// 尝试读取集合头
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadCollectionHeader(out int length)
		{
			length = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(GetReadSpan(4)));
			if (ReadRemain < length)
			{
				this.LogError($"数组长度超出数据长度: {length}.");
				return false;
			}
			return length != TreePackCode.NULL_COLLECTION;
		}

		/// <summary>
		/// 危险读取非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void DangerousReadUnmanagedArray<T>(ref T[] value)
		{
			if (!TryReadCollectionHeader(out var length))
			{
				value = null;
				return;
			}

			if (length == 0)
			{
				value = Array.Empty<T>();
				return;
			}

			var byteCount = length * Unsafe.SizeOf<T>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(byteCount));

			//假如数组为空或长度不一致，那么重新分配
			if (value == null || value.Length != length) value = new T[length];

			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));
			Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
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

			if (!TryReadCollectionHeader(out var length))
			{
				value = null;
				return;
			}

			if (length == 0)
			{
				value = Array.Empty<T>();
				return;
			}

			//假如数组为空或长度不一致，那么重新分配
			if (value == null || value.Length != length)
			{
				value = new T[length];
			}

			Core.RuleManager.SupportGenericRule<T>();
			if (ruleDict.TryGetValue(TypeInfo<Deserialize<T>>.TypeCode, out RuleList ruleList))
			{
				IRuleList<Deserialize<T>> ruleListT = ruleList;
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

		#endregion
	}

	public static partial class ByteSequenceRule
	{

		class Add : AddRule<ByteSequence>
		{
			protected override void Execute(ByteSequence self)
			{
				self.Core.PoolGetUnit(out self.segmentList);
				// 获取节点的法则集
				self.Core.RuleManager.NodeTypeRulesDict.TryGetValue(self.Type, out self.ruleDict);
			}
		}

		class Remove : RemoveRule<ByteSequence>
		{
			protected override void Execute(ByteSequence self)
			{
				self.Clear();
				self.segmentList.Dispose();
			}
		}

		/// <summary>
		/// 序列化写入类型
		/// </summary>
		public static void Serialize<T>(this ByteSequence self, ref T value)
		{
			self.Core.RuleManager.SupportGenericRule<T>();
			if (self.ruleDict.TryGetValue(TypeInfo<Serialize<T>>.TypeCode, out RuleList ruleList))
				((IRuleList<Serialize<T>>)ruleList).SendRef(self, ref value);
		}

		/// <summary>
		/// 反序列化读取类型
		/// </summary>
		public static void Deserialize<T>(this ByteSequence self, ref T value)
		{
			self.Core.RuleManager.SupportGenericRule<T>();
			if (self.ruleDict.TryGetValue(TypeInfo<Deserialize<T>>.TypeCode, out RuleList ruleList))
				((IRuleList<Deserialize<T>>)ruleList).SendRef(self, ref value);
		}
	}

}
