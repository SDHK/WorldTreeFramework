/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Codice.CM.Common.CmCallContext;

namespace WorldTree
{
	/// <summary>
	/// 树序列化写入器
	/// </summary>
	public ref struct TreeSerializeWriter
	{
		IBufferWriter<byte> bufferWriter;
		/// <summary>
		/// 框架核心
		/// </summary>
		private WorldTreeCore core;

		public TreeSerializeWriter(WorldTreeCore core, IBufferWriter<byte> bufferWriter)
		{
			this.core = core;
			this.bufferWriter = bufferWriter;
		}

		/// <summary>
		/// 序列化写入非托管类型
		/// </summary>
		public void SerializeUnmanaged<T>(T value) where T : unmanaged
		{
			unsafe
			{
				int size = Unsafe.SizeOf<T>();
				Span<byte> span = bufferWriter.GetSpan(size);
				bufferWriter.Advance(size);

				ref byte destination = ref MemoryMarshal.GetReference(span);
				ref byte source = ref Unsafe.As<T, byte>(ref value);
				Unsafe.CopyBlockUnaligned(ref destination, ref source, (uint)size);
			}
		}
	}
}
