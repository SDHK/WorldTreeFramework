/****************************************

* 作者：闪电黑客
* 日期：2024/7/9 15:29

* 描述：

*/
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WorldTree
{
	/// <summary>
	/// Byte序列
	/// </summary>
	public class ByteSequence : Node, IByteSequence
		, AsAwake
		, AsComponentBranch
		, TempOf<INode>
	{

		/// <summary>
		/// 字符串编码
		/// </summary>
		public bool Utf8 = true;

		/// <summary>
		/// 序列段列表
		/// </summary>
		public UnitList<ByteSequenceSegment> segmentList = null;

		/// <summary>
		/// 当前写入缓存
		/// </summary>
		private ByteSequenceSegment writeCurrent;

		/// <summary>
		/// 数据长度
		/// </summary>
		public int Length => length;

		/// <summary>
		/// 读取片段指针位置
		/// </summary>
		public int ReadPoint => readPoint;

		/// <summary>
		/// 读取数据的剩余长度
		/// </summary>
		public int ReadRemain => length - readPoint;

		public int WriteBytePoint
		{
			get { return writeCurrent.Length; }

			//set
			//{
			//	if (writeCurrent.Length == value) return;

			//	if (value < 0 || value > writeCurrent.Length) this.LogError("写入指针越界");
			//	length += (value - writeCurrent.Length);
			//	writeCurrent.SetPoint(value);
			//}
		}

		/// <summary>
		/// 数据长度
		/// </summary>
		protected int length = 0;

		/// <summary>
		/// 读取片段指针
		/// </summary>
		protected int readSegmentPoint = 0;

		/// <summary>
		/// 读取byte指针
		/// </summary>
		protected int readBytePoint = 0;

		/// <summary>
		/// 读取总指针
		/// </summary>
		protected int readPoint = 0;

		/// <summary>
		/// 租用缓冲区
		/// </summary>
		private byte[] rentBuffers;

		/// <summary>
		/// 获取写入操作跨度
		/// </summary>
		public Span<byte> GetWriteSpan(int sizeHint)
		{
			if (!writeCurrent.IsNull)
			{
				// 拿到当前缓存的空白区域
				Span<byte> buffer = writeCurrent.FreeSpan;
				// 如果空白区域大于等于需要的空间，那么直接返回
				if (buffer.Length > sizeHint)
				{
					writeCurrent.Advance(sizeHint);
					length += sizeHint;
					return buffer;
				}
			}

			// 因为是结构体，所以需要等到缓存满了之后再添加到列表
			if (writeCurrent.Length != 0) segmentList.Add(writeCurrent);

			// 如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			ByteSequenceSegment next = new(sizeHint);
			writeCurrent = next;
			writeCurrent.Advance(sizeHint);
			length += sizeHint;
			return next.FreeSpan;
		}

		/// <summary>
		/// 添加Byte数组
		/// </summary>
		public void SetBytes(byte[] array)
		{
			if (array == null || array.Length == 0) return;
			segmentList.Add(new(array));
			length += array.Length;
		}

		/// <summary>
		/// 转换为Byte数组
		/// </summary>
		public byte[] ToBytes()
		{
			if (length == 0) return Array.Empty<byte>();
			// 是否池数组
			byte[] results = new byte[length];
			Span<byte> span = results.AsSpan();

			foreach (ByteSequenceSegment item in segmentList)
			{
				item.ByteSpan.CopyTo(span);
				span = span.Slice(item.Length);
			}
			if (!writeCurrent.IsNull) writeCurrent.ByteSpan.CopyTo(span);
			return results;
		}

		/// <summary>
		/// 写入指针回退
		/// </summary>
		/// <param name="sizeHint"></param>
		public void WriteBack(int sizeHint)
		{
			if (sizeHint <= 0) return;
			if (writeCurrent.IsNull) this.LogError("序列已经到达开头");
			if (writeCurrent.Length - sizeHint < 0) this.LogError("回退长度超出数据长度");
			writeCurrent.Advance(-sizeHint);
			length -= sizeHint;
		}

		/// <summary>
		/// 读取指针回退
		/// </summary>
		public void ReadBack(int sizeHint)
		{
			if (sizeHint <= 0) return;
			if (readPoint == 0) this.LogError("序列已经到达开头");
			if (readPoint - sizeHint < 0) this.LogError("回退长度超出数据长度");
			readPoint -= sizeHint;
			while (sizeHint != 0)
			{
				ByteSequenceSegment nowSegment;
				if (readBytePoint == 0)
				{
					readSegmentPoint--;
					nowSegment = segmentList[readSegmentPoint];
					readBytePoint = nowSegment.Length;
				}
				int nowRemain = readBytePoint;
				if (nowRemain == sizeHint)
				{
					//如果当前片段的长度等于剩余需要读取的空间
					readBytePoint = 0;
					break;
				}
				//如果当前片段的长度大于剩余需要读取的空间
				else if (nowRemain > sizeHint)
				{
					readBytePoint -= sizeHint;
					break;
				}
				//如果当前片段的长度小于剩余需要读取的空间
				else
				{
					readSegmentPoint--;
					sizeHint -= nowRemain;
				}
			}
		}

		/// <summary>
		/// 读取指针跳到指定位置
		/// </summary>
		/// <param name="readPoint"></param>
		public void ReadJump(int readPoint)
		{
			if (readPoint < 0 || readPoint > length)
			{
				this.LogError("跳转位置超出数据长度");
				return;
			}

			if (readPoint > this.readPoint)
			{
				// 向后跳
				int sizeHint = readPoint - this.readPoint;
				while (sizeHint != 0)
				{
					ByteSequenceSegment nowSegment = segmentList[readSegmentPoint];
					int nowRemain = nowSegment.Length - readBytePoint;
					if (nowRemain == sizeHint)
					{
						//如果当前片段的长度等于剩余需要读取的空间
						readBytePoint = 0;
						readSegmentPoint++;
						break;
					}
					//如果当前片段的长度大于剩余需要读取的空间
					else if (nowRemain > sizeHint)
					{
						readBytePoint += sizeHint;
						break;
					}
					//如果当前片段的长度小于剩余需要读取的空间
					else
					{
						readSegmentPoint++;
						sizeHint -= nowRemain;
					}
				}
			}
			else
			{
				// 向前跳
				int sizeHint = this.readPoint - readPoint;
				while (sizeHint != 0)
				{
					ByteSequenceSegment nowSegment;
					if (readBytePoint == 0)
					{
						readSegmentPoint--;
						nowSegment = segmentList[readSegmentPoint];
						readBytePoint = nowSegment.Length;
					}
					int nowRemain = readBytePoint;
					if (nowRemain == sizeHint)
					{
						//如果当前片段的长度等于剩余需要读取的空间
						readBytePoint = 0;
						break;
					}
					//如果当前片段的长度大于剩余需要读取的空间
					else if (nowRemain > sizeHint)
					{
						readBytePoint -= sizeHint;
						break;
					}
					//如果当前片段的长度小于剩余需要读取的空间
					else
					{
						readSegmentPoint--;
						sizeHint -= nowRemain;
					}
				}
			}

			this.readPoint = readPoint;
		}


		/// <summary>
		/// 读取指针跳过指定长度
		/// </summary>
		public void ReadSkip(int sizeHint)
		{
			if (sizeHint <= 0) return;
			if (readPoint == length) this.LogError("序列已经到达末尾");
			if (readPoint + sizeHint > length) this.LogError("跳过长度超出数据长度");
			readPoint += sizeHint;
			while (sizeHint != 0)
			{
				ByteSequenceSegment nowSegment = segmentList[readSegmentPoint];
				int nowRemain = nowSegment.Length - readBytePoint;
				if (nowRemain == sizeHint)
				{
					//如果当前片段的长度等于剩余需要读取的空间
					readBytePoint = 0;
					readSegmentPoint++;
					break;
				}
				//如果当前片段的长度大于剩余需要读取的空间
				else if (nowRemain > sizeHint)
				{
					readBytePoint += sizeHint;
					break;
				}
				//如果当前片段的长度小于剩余需要读取的空间
				else
				{
					readSegmentPoint++;
					sizeHint -= nowRemain;
				}
			}
		}

		/// <summary>
		/// 获取读取操作跨度
		/// </summary>
		public Span<byte> GetReadSpan(int sizeHint)
		{
			if (readPoint == length) this.LogError("序列已经到达末尾");
			if (readPoint + sizeHint > length) this.LogError("超出读取长度");
			if (!writeCurrent.IsNull)
			{
				segmentList.Add(writeCurrent);
				writeCurrent = default;
			}

			ByteSequenceSegment nowSegment = segmentList[readSegmentPoint];
			int nowRemain = nowSegment.Length - readBytePoint;
			Span<byte> span;
			if (nowRemain == sizeHint)
			{
				// 如果当前缓存的剩余空间等于需要的空间，那么直接返回
				span = nowSegment.ReadSpan(readBytePoint, sizeHint);
				readBytePoint = 0;
				readSegmentPoint++;
				readPoint += sizeHint;
				return span;
			}
			else if (nowRemain > sizeHint)
			{
				span = nowSegment.ReadSpan(readBytePoint, sizeHint);
				readBytePoint += sizeHint;
				readPoint += sizeHint;
				return span;
			}
			else //如果空白区域小于等于需要的空间，则需要重新申请一个缓存
			{
				if (rentBuffers != null) ArrayPool<byte>.Shared.Return(rentBuffers);
				rentBuffers = ArrayPool<byte>.Shared.Rent(MathInt.GetPowerOfTwo(sizeHint));

				//先读取当前缓存的剩余空间
				span = nowSegment.ReadSpan(readBytePoint, nowRemain);
				span.CopyTo(rentBuffers);
				//计算剩余需要读取的空间
				nowRemain = sizeHint - nowRemain;
				while (nowRemain != 0)
				{
					//序列指针移动
					readSegmentPoint++;
					nowSegment = segmentList[readSegmentPoint];
					//如果当前片段的长度等于剩余需要读取的空间
					if (nowSegment.Length == nowRemain)
					{
						nowSegment.ReadSpan(0, nowRemain).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						readBytePoint = 0;
						readSegmentPoint++;
						break;
					}
					//如果当前片段的长度大于剩余需要读取的空间
					else if (nowSegment.Length > nowRemain)
					{
						nowSegment.ReadSpan(0, nowRemain).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						readBytePoint = nowRemain;
						break;
					}
					else //如果当前片段的长度小于剩余需要读取的空间
					{
						nowSegment.ReadSpan(0, nowSegment.Length).CopyTo(rentBuffers.AsSpan(sizeHint - nowRemain));
						nowRemain -= nowSegment.Length;
					}
				}
				readPoint += sizeHint;
				return rentBuffers.AsSpan(0, sizeHint);
			}
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
			writeCurrent = default;

			if (length == 0) return;
			foreach (var item in segmentList) item.Clear();
			length = 0;
			segmentList.Clear();

			if (rentBuffers != null) ArrayPool<byte>.Shared.Return(rentBuffers);
		}

		/// <summary>
		/// 获取写入引用
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref byte GetWriteRefByte(int sizeHint) => ref MemoryMarshal.GetReference(GetWriteSpan(sizeHint));

		/// <summary>
		/// 获取读取引用
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref byte GetReadRefByte(int sizeHint) => ref MemoryMarshal.GetReference(GetReadSpan(sizeHint));

		#region 写入

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUnmanaged<T1>(in T1 value)
			where T1 : unmanaged
		{
			Unsafe.WriteUnaligned(ref GetWriteRefByte(Unsafe.SizeOf<T1>()), value);
		}

		/// <summary>
		/// 写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUnmanaged<T1, T2>(in T1 value1, in T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged
		{
			ref byte spanRef = ref GetWriteRefByte(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>());
			Unsafe.WriteUnaligned(ref spanRef, value1);
			Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()), value2);
		}

		/// <summary>
		/// 危险写入固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DangerousWriteUnmanaged<T1>(in T1 value)
		{
			Unsafe.WriteUnaligned(ref GetWriteRefByte(Unsafe.SizeOf<T1>()), value);
		}

		/// <summary>
		/// 危险写入非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DangerousWriteUnmanagedArray<T>(T[] value)
		{
			if (value == null)
			{
				WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			//获取数组数据长度
			var srcLength = Unsafe.SizeOf<T>() * value.Length;
			//包含数组数量的总长度
			var allocSize = srcLength + Unsafe.SizeOf<int>();

			//获取写入操作跨度
			ref byte spanRef = ref GetWriteRefByte(allocSize);

			//获取数组数据的指针
			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));

			//写入数组长度
			Unsafe.WriteUnaligned(ref spanRef, value.Length);
			//写入数组数据
			Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<int>()), ref src, (uint)srcLength);
		}


		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteString(string value)
		{
			if (Utf8)
			{
				WriteUtf8(value);
			}
			else
			{
				WriteUtf16(value);
			}
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf16(string value)
		{
			if (value == null)
			{
				WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}


			//获取字符串长度,因为 UTF-16 编码的每个字符占用 2 个字节，checked 防止溢出int值
			var copyByteCount = checked(value.Length * 2);

			//这行代码获取一个引用，指向一个足够大的缓冲区，以容纳字符串的字节数和额外的 4 个字节。
			ref byte dest = ref GetWriteRefByte(copyByteCount + 4);
			//这行代码将字符串的长度（以字符为单位）写入缓冲区的前 4 个字节
			Unsafe.WriteUnaligned(ref dest, value.Length);

			//这行代码将字符串的实际字节数据复制到缓冲区中，跳过前 4 个字节
			MemoryMarshal.AsBytes(value.AsSpan()).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dest, 4), copyByteCount));
		}

		/// <summary>
		/// 写入字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUtf8(string value)
		{
			if (value == null)
			{
				WriteUnmanaged((int)ValueMarkCode.NULL_OBJECT);
				return;
			}
			if (value.Length == 0)
			{
				WriteUnmanaged(0);
				return;
			}

			// (int utf16-length, int utf8-byte-count, utf8-bytes)
			ReadOnlySpan<char> source = value.AsSpan();
			// UTF8.GetMaxByteCount -> (length + 1) * 3
			int maxByteCount = (source.Length + 1) * 3;

			// write utf16-length ，字符长度
			//WriteUnmanaged(source.Length);

			//申请总空间，包含utf8长度和数据

			// 由于不知道空间大小，所以字符数*3只是获取一个可能的最大空间，字符最小可能是只占1个字节
			// 头部需要写入byte真实长度，int长度偏移+4
			ref byte destPointer = ref GetWriteRefByte(maxByteCount + 4);

			//申请数据空间，byte长度int要写到头部，所以要偏移4
			Span<byte> dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref destPointer, 4), maxByteCount);

			// 数据写入到dest，此时拿到了byte的真实长度
			int bytesWritten = Encoding.UTF8.GetBytes(value, dest);

			// 在头部写入 真实长度，取反用于区分8位和16位
			int nbytesWritten = ~bytesWritten;
			Unsafe.WriteUnaligned(ref destPointer, nbytesWritten);
			//~0 的结果是 -1，但前面if挡住了，所以不会出现-1，所以可以用来区分8位和16位

			// 重新定位指针，裁剪空间
			WriteBack(maxByteCount - bytesWritten);
		}


		#endregion

		#region 读取

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 ReadUnmanaged<T1>()
			where T1 : unmanaged
		{
			return Unsafe.ReadUnaligned<T1>(ref GetReadRefByte(Unsafe.SizeOf<T1>()));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 ReadUnmanaged<T1>(out T1 value)
			where T1 : unmanaged
		{
			return value = Unsafe.ReadUnaligned<T1>(ref GetReadRefByte(Unsafe.SizeOf<T1>()));
		}

		/// <summary>
		/// 读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadUnmanaged<T1, T2>(out T1 value1, out T2 value2)
			where T1 : unmanaged
			where T2 : unmanaged
		{
			ref byte spanRef = ref GetReadRefByte(Unsafe.SizeOf<T1>() + Unsafe.SizeOf<T2>());
			value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
			value2 = Unsafe.ReadUnaligned<T2>(ref Unsafe.Add(ref spanRef, Unsafe.SizeOf<T1>()));
		}


		/// <summary>
		///  危险读取固定长度数值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T1 DangerousReadUnmanaged<T1>(out T1 value)
		{
			return value = Unsafe.ReadUnaligned<T1>(ref GetReadRefByte(Unsafe.SizeOf<T1>()));
		}



		/// <summary>
		/// 危险读取非托管数组
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void DangerousReadUnmanagedArray<T>(ref T[] value)
		{
			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
			{
				value = null;
				return;
			}
			else if (length == 0)
			{
				value = Array.Empty<T>();
				return;
			}
			else if (ReadRemain < length)
			{
				this.LogError($"数组长度超出数据长度: {length}.");
				value = null;
				return;
			}

			var byteCount = length * Unsafe.SizeOf<T>();
			ref byte spanRef = ref GetReadRefByte(byteCount);

			//假如数组为空或长度不一致，那么重新分配
			if (value == null || value.Length != length) value = new T[length];

			ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value.AsSpan()));
			Unsafe.CopyBlockUnaligned(ref src, ref spanRef, (uint)byteCount);
		}

		/// <summary>
		/// 读取字符串
		/// </summary>
		public string ReadString()
		{
			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
			{
				return null;
			}
			else if (length == 0)
			{
				return string.Empty;
			}

			if (length > 0)
			{
				return ReadUtf16(length);
			}
			else
			{
				return ReadUtf8(length);
			}
		}

		/// <summary>
		/// 跳过字符串
		/// </summary>
		public void SkipString()
		{
			if (ReadUnmanaged(out int length) == ValueMarkCode.NULL_OBJECT)
			{
				return;
			}
			else if (length == 0)
			{
				return;
			}
			if (length > 0)
			{
				ReadSkip(length);
			}
			else
			{
				ReadSkip(~length);
			}
		}

		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadUtf16(int length)
		{
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}
			ref byte src = ref GetReadRefByte(length);
			return new string(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, char>(ref src), (int)(length*0.5f)));
		}


		/// <summary>
		/// 读取字符串
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)] // non default, no inline
		public string ReadUtf8(int length)
		{
			length = ~length;
			if (ReadRemain < length)
			{
				this.LogError($"字符串长度超出数据长度: {length}.");
				return null;
			}

			// 获取总数据，包含utf8长度和数据
			//*3只是一个可能的最大空间，字符最小可能是只占1个字节

			//读取 utf8ByteLength
			//var utf8BytesLength = ReadUnmanaged<int>();

			ref var spanRef = ref GetReadRefByte(length);

			if (length <= 0)
			{
				ReadOnlySpan<byte> src = MemoryMarshal.CreateReadOnlySpan(ref spanRef, length);
				return Encoding.UTF8.GetString(src);
			}
			else
			{
				//if (ReadRemain < utf8BytesLength)
				//{
				//	this.LogError($"字符串长度超出数据长度: {utf8BytesLength}.");
				//	return null;
				//}

				ReadOnlySpan<byte> src = MemoryMarshal.CreateReadOnlySpan(ref spanRef, length);
				return Encoding.UTF8.GetString(src);
			}
		}

		#endregion
	}

	public static partial class ByteSequenceRule
	{
		class Add : AddRule<ByteSequence>
		{
			protected override void Execute(ByteSequence self)
			{
				self.Core.PoolGetUnit(out self.segmentList);
			}
		}

		class Remove : RemoveRule<ByteSequence>
		{
			protected override void Execute(ByteSequence self)
			{
				self.Clear();
				self.segmentList.Dispose();
			}
		}
	}
}
