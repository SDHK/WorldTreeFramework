/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WorldTree
{

	public static class ByteSequenceWriterRule
	{
		class Add : AddRule<ByteSequenceWriter>
		{
			protected override void Execute(ByteSequenceWriter self)
			{
				self.Core.PoolGetUnit(out self.sequenceList);
				self.current = default;
				self.length = 0;
			}
		}

		class Remove : RemoveRule<ByteSequenceWriter>
		{
			protected override void Execute(ByteSequenceWriter self)
			{
				self.sequenceList.Dispose();
				self.sequenceList = null;
				self.current = default;
				self.length = 0;
			}
		}
	}

	/// <summary>
	/// 字节缓存写入器
	/// </summary>
	public class ByteSequenceWriter : Node, IBufferWriter<byte>
		, AsAwake
		, AsComponentBranch
		, TempOf<INode>
	{
		/// <summary>
		/// 缓存列表
		/// </summary>
		public UnitList<ByteSequence> sequenceList = null;

		/// <summary>
		/// 当前缓存
		/// </summary>
		public ByteSequence current;

		/// <summary>
		/// 数据长度
		/// </summary>
		public int length;

		/// <summary>
		/// 数据长度
		/// </summary>
		public int Length => length;

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
				Span<byte> buffer = current.FreeSpan;
				// 如果空白区域大于等于需要的空间，那么直接返回
				if (buffer.Length > sizeHint)
				{
					return buffer;
				}
			}

			// 因为是结构体，所以需要等到缓存满了之后再添加到列表
			if (current.Length != 0) sequenceList.Add(current);

			// 如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			ByteSequence next = new(sizeHint);
			current = next;
			return next.FreeSpan;
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

			foreach (ByteSequence item in sequenceList)
			{
				item.ByteSpan.CopyTo(span);
				span = span.Slice(item.Length);
				item.Clear();
			}

			if (!current.IsNull)
			{
				current.ByteSpan.CopyTo(span);
				current.Clear();
			}

			length = 0;
			sequenceList.Clear();
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
		/// 写入并重置
		/// </summary>
		public async ValueTask WriteToAndResetAsync(Stream stream, CancellationToken cancellationToken)
		{
			if (length == 0) return;
			foreach (var item in sequenceList)
			{
				await stream.WriteAsync(item.ByteMemory, cancellationToken).ConfigureAwait(false);
				item.Clear(); // reset
			}
			length = 0;
			sequenceList.Clear();
			current = default;
		}

		/// <summary>
		/// 重置
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			if (length == 0) return;
			foreach (var item in sequenceList) item.Clear();
			length = 0;
			sequenceList.Clear();
			current = default;
		}

		/// <summary>
		/// 序列化写入非托管类型
		/// </summary>
		public void Serialize<T>(T value) where T : unmanaged
		{
			unsafe
			{
				int size = Unsafe.SizeOf<T>();
				Span<byte> span = GetSpan(size);
				Advance(size);
				ref byte destination = ref MemoryMarshal.GetReference(span);
				ref byte source = ref Unsafe.As<T, byte>(ref value);
				Unsafe.CopyBlockUnaligned(ref destination, ref source, (uint)size);
			}
		}
	}
}
