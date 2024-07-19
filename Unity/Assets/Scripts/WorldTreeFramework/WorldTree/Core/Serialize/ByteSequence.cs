/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// Byte序列
	/// </summary>
	public class ByteSequence : Node
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
		/// 数据长度
		/// </summary>
		public int length = 0;

		/// <summary>
		/// 读取序列指针
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
		/// 位移长度
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count)
		{
			current.Advance(count);
			length += count;
		}

		/// <summary>
		/// 获取操作跨度
		/// </summary>
		private Span<byte> GetSpan(int sizeHint)
		{
			if (!current.IsNull)
			{
				// 拿到当前缓存的空白区域
				Span<byte> buffer = current.FreeSpan;
				// 如果空白区域大于等于需要的空间，那么直接返回
				if (buffer.Length > sizeHint)
				{
					return buffer;
				}
			}

			// 因为是结构体，所以需要等到缓存满了之后再添加到列表
			if (current.Length != 0) segmentList.Add(current);

			// 如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			ByteSequenceSegment next = new(sizeHint);
			current = next;
			return next.FreeSpan;
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

			if (length == 0) return;
			foreach (var item in segmentList) item.Clear();
			length = 0;
			segmentList.Clear();
			current = default;
		}

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUnmanaged<T1>(in T1 value1)
			where T1 : unmanaged
		{
			var size = Unsafe.SizeOf<T1>();
			ref byte spanRef = ref MemoryMarshal.GetReference(GetSpan(size));
			Unsafe.WriteUnaligned(ref spanRef, value1);
			Advance(size);
		}


	}
}
