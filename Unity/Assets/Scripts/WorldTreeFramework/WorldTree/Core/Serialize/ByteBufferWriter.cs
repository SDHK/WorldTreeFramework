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
	/// 字节缓存写入器
	/// </summary>
	public class ByteBufferWriter : Unit, IBufferWriter<byte>
	{
		/// <summary>
		/// 缓存列表
		/// </summary>
		private UnitList<ByteBuffer> bufferList = null;

		/// <summary>
		/// 当前缓存
		/// </summary>
		private ByteBuffer current;

		/// <summary>
		/// 长度
		/// </summary>
		private int length;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out bufferList);
			current = default;
			length = 0;
		}

		public override void OnDispose()
		{
			bufferList.Dispose();
			bufferList = null;
			current = default;
			length = 0;
		}

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
		public Span<byte> GetSpan(int sizeHint)
		{
			if (!current.IsNull)
			{
				// 拿到当前缓存的空白区域
				Span<byte> buffer = current.FreeBuffer;
				// 如果空白区域大于等于需要的空间，那么直接返回
				if (buffer.Length > sizeHint) return buffer;
			}

			// 如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			ByteBuffer next = new ByteBuffer(sizeHint);
			bufferList.Add(next);
			current = next;
			return next.FreeBuffer;
		}

		/// <summary>
		/// 转换为数组并重置
		/// </summary>
		public byte[] ToArrayAndReset()
		{
			if (length == 0) return Array.Empty<byte>();
			// 是否池数组
			byte[] results = new byte[length];
			Span<byte> span = results.AsSpan();

			foreach (ByteBuffer item in bufferList)
			{
				item.Buffer.CopyTo(span);
				span = span.Slice(item.Length);
				item.Clear();
			}
			length = 0;
			bufferList.Clear();
			current = default;
			return results;
		}

		/// <summary>
		/// 获取内存：不支持
		/// </summary>
		public Memory<byte> GetMemory(int sizeHint = 0)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 重置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			if (length == 0) return;
			foreach (var item in bufferList) item.Clear();
			length = 0;
			bufferList.Clear();
			current = default;
		}


	}
}
