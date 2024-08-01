/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree
{


	public static partial class ByteSequenceRule
	{

		class Add : AddRule<ByteSequence>
		{
			protected override void Execute(ByteSequence self)
			{

				self.Core.PoolGetUnit(out self.segmentList);

				// 序列化法则
				//if (self.Core.RuleManager.GetOrNewRuleList( NodeType,)
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
	}

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
		/// 序列化法则列表
		/// </summary>
		public RuleList serializeRuleList;

		/// <summary>
		/// 反序列化法则列表
		/// </summary>
		public RuleList deserializeRuleList;

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
			var size = Unsafe.SizeOf<T1>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(size));
			Unsafe.WriteUnaligned(ref spanRef, value1);
		}

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T1, T2>(in T1 value1, in T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged
		{
			var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(size));
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
			var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetWriteSpan(size));
			Unsafe.WriteUnaligned(ref spanRef, value1);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()), value2);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()), value3);
		}

		#endregion

		#region 读取

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 Read<T1>()
		{
			var size = Unsafe.SizeOf<T1>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(size));
			return Unsafe.ReadUnaligned<T1>(ref spanRef);
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 Read<T1>(out T1 value1)
		{
			var size = Unsafe.SizeOf<T1>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(size));
			return value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public void Read<T1, T2>(out T1 value1, out T2 value2)
		{
			var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(size));
			value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
			value2 = Unsafe.ReadUnaligned<T2>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		public void Read<T1, T2, T3>(out T1 value1, out T2 value2, out T3 value3)
		{
			var size = Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>() + Unsafe.SizeOf<T3>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetReadSpan(size));
			value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
			value2 = Unsafe.ReadUnaligned<T2>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()));
			value3 = Unsafe.ReadUnaligned<T3>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>()));
		}
		#endregion
	}
}
