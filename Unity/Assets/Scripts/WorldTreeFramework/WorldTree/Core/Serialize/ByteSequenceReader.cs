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

	public static class ByteSequenceReaderRule
	{
		class Remove : RemoveRule<ByteSequenceReader>
		{
			protected override void Execute(ByteSequenceReader self)
			{
				if (self.rentBuffers != null)
				{
					ArrayPool<byte>.Shared.Return(self.rentBuffers);
					self.rentBuffers = null;
				}
				self.bufferReference = default;
				self.bufferSource = default;
				self.bufferLength = 0;
				self.length = 0;
				self.consumeLength = 0;
				self.advancedCount = 0;
			}
		}
	}


	/// <summary>
	/// 字节序列读取器
	/// </summary>
	public class ByteSequenceReader : Node
		, AsAwake
		, AsComponentBranch
		, TempOf<INode>
	{
		/// <summary>
		/// 只读储存
		/// </summary>
		public ReadOnlyMemory<byte> bufferReference;

		/// <summary>
		/// 只读序列
		/// </summary>
		public ReadOnlySequence<byte> bufferSource;

		/// <summary>
		/// 数据长度
		/// </summary>
		public int bufferLength;

		/// <summary>
		/// 总长度
		/// </summary>
		public long length;

		/// <summary>
		/// 消耗长度
		/// </summary>
		public int consumeLength;

		/// <summary>
		/// 租用缓冲区
		/// </summary>
		public byte[] rentBuffers;

		/// <summary>
		/// 已经推进的长度
		/// </summary>
		public int ConsumeLength => consumeLength;

		/// <summary>
		/// 剩余长度
		/// </summary>
		public long RemainLength => length - consumeLength;

		/// <summary>
		/// 总长度
		/// </summary>
		public long Length => length;

		/// <summary>
		/// 推进移动数
		/// </summary>
		public int advancedCount;

		/// <summary>
		/// 设置只读序列
		/// </summary>
		public void SetReadOnlySequence(in ReadOnlySequence<byte> sequence)
		{
			bufferSource = sequence.IsSingleSegment ? ReadOnlySequence<byte>.Empty : sequence;
			ReadOnlyMemory<byte> span = sequence.First;
			bufferReference = span;
			bufferLength = span.Length;
			advancedCount = 0;
			consumeLength = 0;
			if (rentBuffers != null)
			{
				ArrayPool<byte>.Shared.Return(rentBuffers);
				rentBuffers = null;
			}
			length = sequence.Length;
		}

		/// <summary>
		/// 获取引用
		/// </summary>
		public ref byte GetSpanReference(int sizeHint)
		{
			if (sizeHint <= bufferLength)
			{
				return ref MemoryMarshal.GetReference(bufferReference.Span);
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

			if (RemainLength == 0)
			{
				this.LogError("序列已经到达末尾");
			}

			try
			{
				bufferSource = bufferSource.Slice(advancedCount);
			}
			catch (ArgumentOutOfRangeException)
			{
				this.LogError("序列已经到达末尾");
			}

			advancedCount = 0;


			//判断是否有足够的空间
			if (sizeHint <= RemainLength)
			{
				//判断第一个跨度是否有足够的空间
				if (sizeHint <= bufferSource.First.Length)
				{
					bufferReference = bufferSource.First;
					bufferLength = bufferSource.First.Length;
					return ref MemoryMarshal.GetReference(bufferReference.Span);
				}
				//没有足够的空间，需要分配新的缓冲区
				rentBuffers = ArrayPool<byte>.Shared.Rent(MathInt.GetPowerOfTwo(sizeHint));
				bufferSource.Slice(0, sizeHint).CopyTo(rentBuffers);
				var span = rentBuffers.AsMemory(0, sizeHint);

				bufferReference = span;
				bufferLength = span.Length;
				return ref MemoryMarshal.GetReference(bufferReference.Span);
			}

			this.LogError("序列已经到达末尾");
			return ref MemoryMarshal.GetReference(bufferReference.Span);
		}

		/// <summary>
		/// 推进移动指针
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count)
		{
			if (count == 0) return;

			//如果剩余的数据长度小于需要推进的长度
			int rest = bufferLength - count;
			if (rest < 0)
			{
				if (TryAdvanceSequence(count))
				{
					return;
				}
			}
			bufferReference = bufferReference.Slice(count);
			advancedCount += count;
			consumeLength += count;
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
				this.LogError("移动超出序列长度");
			}

			bufferSource = bufferSource.Slice(advancedCount + count);

			bufferReference = bufferSource.First;
			bufferLength = bufferSource.First.Length;
			advancedCount = 0;
			consumeLength += count;
			return true;
		}

		/// <summary>
		/// 获取剩余源
		/// </summary>
		public void GetRemainingSource(out ReadOnlyMemory<byte> singleSource, out ReadOnlySequence<byte> remainingSource)
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
					singleSource = bufferSource.First.Slice(advancedCount);
					return;
				}

				// 如果剩余的数据不是单个段
				singleSource = default;
				remainingSource = bufferSource.Slice(advancedCount);
				// 如果剩余的数据是单个段
				if (remainingSource.IsSingleSegment)
				{
					singleSource = remainingSource.First;
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
			int size = Unsafe.SizeOf<T>();
			T value = Unsafe.ReadUnaligned<T>(ref GetSpanReference(size));
			Advance(size);
			return value;
		}

	}
}
