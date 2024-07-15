/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using MemoryPack;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// 字节序列读取器
	/// </summary>
	public ref struct UnmanagedDeSerialize
	{
		/// <summary>
		/// 只读跨度器
		/// </summary>
		private ReadOnlySpan<byte> bufferReference;

		/// <summary>
		/// 只读序列
		/// </summary>
		private ReadOnlySequence<byte> bufferSource;

		/// <summary>
		/// 数据长度
		/// </summary>
		private int bufferLength;

		/// <summary>
		/// 总长度
		/// </summary>
		private long totalLength;

		/// <summary>
		/// 消耗长度
		/// </summary>
		private int consumed;

		/// <summary>
		/// 租用缓冲区
		/// </summary>
		private byte[] rentBuffers;

		/// <summary>
		/// 消耗
		/// </summary>
		public int Consumed => consumed;

		/// <summary>
		/// 剩余长度
		/// </summary>
		public long Remaining => totalLength - consumed;

		/// <summary>
		/// 高级计数
		/// </summary>
		private int advancedCount;

		/// <summary>
		/// 设置只读序列
		/// </summary>
		public UnmanagedDeSerialize(in ReadOnlySequence<byte> sequence)
		{
			this.bufferSource = sequence.IsSingleSegment ? ReadOnlySequence<byte>.Empty : sequence;
			var span = sequence.FirstSpan;
			this.bufferReference = span;
			this.bufferLength = span.Length;
			this.advancedCount = 0;
			this.consumed = 0;
			this.rentBuffers = null;
			this.totalLength = sequence.Length;
		}

		/// <summary>
		/// 设置只读跨度
		/// </summary>
		/// <param name="buffer"></param>
		public UnmanagedDeSerialize(in ReadOnlySpan<byte> buffer)
		{
			this.bufferSource = ReadOnlySequence<byte>.Empty;
			this.bufferReference = buffer;
			this.bufferLength = buffer.Length;

			this.advancedCount = 0;
			this.consumed = 0;
			this.rentBuffers = null;
			this.totalLength = buffer.Length;
		}

		/// <summary>
		/// 获取引用
		/// </summary>
		public ref byte GetSpanReference(int sizeHint)
		{
			if (sizeHint <= bufferLength)
			{
				return ref MemoryMarshal.GetReference(bufferReference);
			}
			return ref GetNextSpan(sizeHint);
		}

		/// <summary>
		/// 获取下一个跨度
		/// </summary>
		private ref byte GetNextSpan(int sizeHint)
		{
			if (rentBuffers != null)
			{
				ArrayPool<byte>.Shared.Return(rentBuffers);
				rentBuffers = null;
			}

			if (Remaining == 0)
			{
				//MemoryPackSerializationException.ThrowSequenceReachedEnd();
			}

			try
			{
				bufferSource = bufferSource.Slice(advancedCount);
			}
			catch (ArgumentOutOfRangeException)
			{
				//MemoryPackSerializationException.ThrowSequenceReachedEnd();
			}

			advancedCount = 0;


			//判断是否有足够的空间
			if (sizeHint <= Remaining)
			{
				//判断第一个跨度是否有足够的空间
				if (sizeHint <= bufferSource.FirstSpan.Length)
				{
					bufferReference = bufferSource.FirstSpan;
					bufferLength = bufferSource.FirstSpan.Length;
					return ref MemoryMarshal.GetReference(bufferReference);
				}
				//没有足够的空间，需要分配新的缓冲区
				rentBuffers = ArrayPool<byte>.Shared.Rent(sizeHint);
				bufferSource.Slice(0, sizeHint).CopyTo(rentBuffers);
				Span<byte> span = rentBuffers.AsSpan(0, sizeHint);

				bufferReference = span;
				bufferLength = span.Length;
				return ref MemoryMarshal.GetReference(bufferReference);
			}
			//MemoryPackSerializationException.ThrowSequenceReachedEnd();
			return ref MemoryMarshal.GetReference(bufferReference);
		}


		/// <summary>
		/// 推进移动指针
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count)
		{
			if (count == 0) return;
			var rest = bufferLength - count;
			if (rest < 0)
			{
				if (TryAdvanceSequence(count))
				{
					return;
				}
			}
			bufferReference = bufferReference.Slice(count);
			advancedCount += count;
			consumed += count;
		}

		/// <summary>
		/// 尝试推进移动指针
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool TryAdvanceSequence(int count)
		{
			var rest = bufferSource.Length - count;
			if (rest < 0)
			{
				MemoryPackSerializationException.ThrowInvalidAdvance();
			}

			bufferSource = bufferSource.Slice(advancedCount + count);

			bufferReference = bufferSource.FirstSpan;
			bufferLength = bufferSource.FirstSpan.Length;
			advancedCount = 0;
			consumed += count;
			return true;
		}

		/// <summary>
		/// 获取剩余源
		/// </summary>
		public void GetRemainingSource(out ReadOnlySpan<byte> singleSource, out ReadOnlySequence<byte> remainingSource)
		{
			// 如果没有剩余的数据
			if (bufferSource.IsEmpty)
			{
				remainingSource = ReadOnlySequence<byte>.Empty;
				singleSource = bufferReference;
				return;
			}
			else // 如果有剩余的数据
			{
				// 如果剩余的数据是单个段
				if (bufferSource.IsSingleSegment)
				{
					remainingSource = ReadOnlySequence<byte>.Empty;
					singleSource = bufferSource.FirstSpan.Slice(advancedCount);
					return;
				}

				// 如果剩余的数据不是单个段

				singleSource = default;
				remainingSource = bufferSource.Slice(advancedCount);
				// 如果剩余的数据是单个段
				if (remainingSource.IsSingleSegment)
				{
					singleSource = remainingSource.FirstSpan;
					remainingSource = ReadOnlySequence<byte>.Empty;
					return;
				}
				return;
			}
		}

		/// <summary>
		/// 反序列化非托管类型
		/// </summary>
		public T Deserialize<T>() where T : unmanaged
		{
			unsafe
			{
				int size = Unsafe.SizeOf<T>();
				T value = Unsafe.ReadUnaligned<T>(ref GetSpanReference(size));
				Advance(size);
				return value;
			}
		}
	}
}
