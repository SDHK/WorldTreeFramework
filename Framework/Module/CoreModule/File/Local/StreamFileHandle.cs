/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 基于 Stream 的通用文件句柄节点。

*/

using System;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 基于 Stream 的通用文件句柄节点
	/// </summary>
	public sealed class StreamFileHandle : FileHandle, TempOf<INode>, AsRule<Awake>
	{
		/// <summary>
		/// 底层流
		/// </summary>
		private Stream stream;

		/// <summary>
		/// 取底层流，未绑定时抛出异常
		/// </summary>
		private Stream Stream => stream ?? throw new InvalidOperationException("文件流未绑定");

		/// <summary>
		/// 绑定底层流
		/// </summary>
		public void Bind(Stream stream)
		{
			if (this.stream != null) throw new InvalidOperationException("文件流已绑定");
			this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
		}

		/// <summary>
		/// 总长度；未知时返回 -1
		/// </summary>
		public override long Length => Stream.Length;

		/// <summary>
		/// 当前读写位置
		/// </summary>
		public override long Position
		{
			get => Stream.Position;
			set => Stream.Position = value;
		}

		/// <summary>
		/// 可读
		/// </summary>
		public override bool CanRead => Stream.CanRead;

		/// <summary>
		/// 可写
		/// </summary>
		public override bool CanWrite => Stream.CanWrite;

		/// <summary>
		/// 可定位
		/// </summary>
		public override bool CanSeek => Stream.CanSeek;

		/// <summary>
		/// 读取
		/// </summary>
		public override int Read(byte[] buffer, int offset, int count) => Stream.Read(buffer, offset, count);

		/// <summary>
		/// 写入
		/// </summary>
		public override void Write(byte[] buffer, int offset, int count) => Stream.Write(buffer, offset, count);

		/// <summary>
		/// 定位
		/// </summary>
		public override long Seek(long offset, SeekOrigin origin) => Stream.Seek(offset, origin);

		/// <summary>
		/// 刷新缓冲
		/// </summary>
		public override void Flush() => Stream.Flush();

		/// <summary>
		/// 关闭底层流
		/// </summary>
		internal void DisposeStream()
		{
			stream?.Dispose();
			stream = null;
		}
	}

	/// <summary>
	/// StreamFileHandle 的法则
	/// </summary>
	public static class StreamFileHandleRule
	{
		class RemoveRule : RemoveRule<StreamFileHandle>
		{
			protected override void Execute(StreamFileHandle self)
			{
				self.DisposeStream();
			}
		}
	}
}
