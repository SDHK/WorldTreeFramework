/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 字节缓存段
	/// </summary>
	internal class ByteBufferWriter : Node, IBufferWriter<byte>
	{
		/// <summary>
		/// 缓存列表
		/// </summary>
		private List<ByteBuffer> bufferList;

		/// <summary>
		/// 位移长度
		/// </summary>
		public void Advance(int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 获取内存：不支持
		/// </summary>
		public Memory<byte> GetMemory(int sizeHint = 0)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 获取内存
		/// </summary>
		public Span<byte> GetSpan(int sizeHint = 0)
		{
			throw new NotImplementedException();
		}
	}
}
