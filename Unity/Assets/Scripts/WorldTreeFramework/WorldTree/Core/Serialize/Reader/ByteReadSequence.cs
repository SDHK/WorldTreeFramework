/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：字节只读序列片段生成器

*/
using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace WorldTree
{

	/// <summary>
	/// 字节只读序列片段
	/// </summary>
	public class ByteReadSequence : ReadOnlySequenceSegment<byte>, IUnit
	{
		public long Type { get; set; }
		public bool IsDisposed { get; set; }
		public bool IsFromPool { get; set; }
		public WorldTreeCore Core { get; set; }

		/// <summary>
		/// 数组是否回收到池
		/// </summary>
		private bool returnToPool;

		/// <summary>
		/// 设置缓存
		/// </summary>
		public void SetBuffer(ReadOnlyMemory<byte> buffer, bool returnToPool)
		{
			Memory = buffer;
			this.returnToPool = returnToPool;
		}

		/// <summary>
		/// 设置运行索引和下一个片段
		/// </summary>
		public void SetRunningIndexAndNext(long runningIndex, ByteReadSequence nextSegment)
		{
			RunningIndex = runningIndex;
			Next = nextSegment;
		}

		public void Dispose() => Core.PoolRecycle(this);

		public void OnCreate() { }

		public void OnDispose()
		{
			if (returnToPool)
			{
				if (MemoryMarshal.TryGetArray(Memory, out var segment) && segment.Array != null)
				{
					ArrayPool<byte>.Shared.Return(segment.Array, clearArray: false);
				}
				returnToPool = false;
			}
			Memory = default;
			RunningIndex = 0;
			Next = null;
		}
	}
}
