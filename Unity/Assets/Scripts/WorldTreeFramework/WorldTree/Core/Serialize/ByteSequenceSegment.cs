/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// Byte序列片段
	/// </summary>
	public struct ByteSequenceSegment
	{
		/// <summary>
		/// 数据
		/// </summary>
		private byte[] bytes;

		/// <summary>
		/// 长度
		/// </summary>
		private int length;

		/// <summary>
		/// 长度
		/// </summary>
		public int Length => length;

		/// <summary>
		/// 是否为空
		/// </summary>
		public bool IsNull => bytes == null;
		/// <summary>
		/// 已写入的数据
		/// </summary>
		public Span<byte> ByteSpan => bytes.AsSpan(0, length);

		/// <summary>
		/// 已写入的数据
		/// </summary>
		/// <remarks>效率不如Span，但可用于异步</remarks>
		public Memory<byte> ByteMemory => bytes.AsMemory(0, length);

		/// <summary>
		/// 未写入的空白区
		/// </summary>
		public Span<byte> FreeSpan => bytes.AsSpan(length);

		/// <summary>
		/// 是否是池
		/// </summary>
		private bool isPool;

		public ByteSequenceSegment(int size)
		{
			isPool = true;
			bytes = ArrayPool<byte>.Shared.Rent(MathInt.GetPowerOfTwo(size));
			length = 0;
		}

		public ByteSequenceSegment(byte[] bytes, bool isPool = false)
		{
			this.isPool = isPool;
			this.bytes = bytes;
			length = bytes.Length;
		}

		/// <summary>
		/// 获取操作跨度
		/// </summary>
		public Span<byte> ReadSpan(int start, int size) => bytes.AsSpan(start, size);

		/// <summary>
		/// 推进移动指针
		/// </summary>
		/// <param name="count">要推进的字节数</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count) => length += count;

		/// <summary>
		/// 清理
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (this.isPool && bytes != null) ArrayPool<byte>.Shared.Return(bytes);
			bytes = null;
			length = 0;
		}
	}
}
