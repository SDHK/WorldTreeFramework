using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ET
{
	// KCP Segment Definition
	/// <summary>
	/// KCP片段定义
	/// </summary>
	internal struct SegmentStruct:IDisposable
    {
		/// <summary>
		/// KCP片段头部
		/// </summary>
		public Kcp.SegmentHead SegHead;
		/// <summary>
		/// 重传时间戳
		/// </summary>
		public uint Resendts;
		/// <summary>
		/// 超时重传时间
		/// </summary>
		public int  Rto;
		/// <summary>
		/// 快速重传
		/// </summary>
		public uint Fastack;
		/// <summary>
		/// 重传次数
		/// </summary>
		public uint Xmit;

		/// <summary>
		/// 缓冲区
		/// </summary>
		private byte[] buffers;

		/// <summary>
		/// ArrayPool
		/// </summary>
		private ArrayPool<byte> arrayPool;

		/// <summary>
		/// 是否为空
		/// </summary>
		public bool IsNull => this.buffers == null;

		/// <summary>
		/// 写入数量
		/// </summary>
		public int WrittenCount
        {
            get => (int) this.SegHead.Len;
            private set => this.SegHead.Len = (uint) value;
        }

		/// <summary>
		/// 已写入缓冲区
		/// </summary>
		public Span<byte> WrittenBuffer => this.buffers.AsSpan(0, (int) this.SegHead.Len);

		/// <summary>
		/// 空闲缓冲区
		/// </summary>
		public Span<byte> FreeBuffer => this.buffers.AsSpan(WrittenCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SegmentStruct(int size, ArrayPool<byte> arrayPool)
        {
            this.arrayPool = arrayPool;
            buffers = arrayPool.Rent(size);
            this.SegHead = default;
            this.Resendts = default;
            this.Rto = default;
            this.Fastack = default;
            this.Xmit = default;
        }

		/// <summary>
		/// 从缓冲区读取数据
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(Span<byte> data, ref int size)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(data),this.SegHead);
            size += Unsafe.SizeOf<Kcp.SegmentHead>();
        }

		/// <summary>
		/// 从缓冲区读取数据
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            this.WrittenCount += count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            arrayPool.Return(this.buffers);
        }
    }
}
