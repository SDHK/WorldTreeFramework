/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 14:42

* 描述：字节缓存段

*/
using Sirenix.Serialization;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 字节缓存段
	/// </summary>
	internal struct ByteBuffer
	{
		/// <summary>
		/// 缓存
		/// </summary>
		private byte[] buffers;

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
		public bool IsNull => buffers == null;
		/// <summary>
		/// 有效的缓存
		/// </summary>
		public Span<byte> Buffer => buffers.AsSpan(0, length);

		/// <summary>
		/// 有效的缓存
		/// </summary>
		/// <remarks>效率不如Span，但可用于异步</remarks>
		public Memory<byte> Memory => buffers.AsMemory(0, length);

		/// <summary>
		/// 空白的缓存
		/// </summary>
		public Span<byte> FreeBuffer => buffers.AsSpan(length);

		public ByteBuffer(int size)
		{
			buffers = ArrayPool<byte>.Shared.Rent(MathInt.GetPowerOfTwo(size));
			length = 0;
		}

		/// <summary>
		/// 位移长度
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count)
		{
			length += count;
		}

		/// <summary>
		/// 清理
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			if (buffers != null) ArrayPool<byte>.Shared.Return(buffers);
			buffers = null;
			length = 0;
		}
	}


}
