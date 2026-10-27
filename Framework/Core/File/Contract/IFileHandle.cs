/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件句柄契约。

*/

using System;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 文件句柄契约
	/// </summary>
	/// <remarks>
	/// Read 返回实际读到的字节数，0 表示 EOF；可能小于请求数量，调用方必须循环。
	/// Length 未知时返回 -1。
	/// </remarks>
	public interface IFileHandle : IDisposable
	{
		/// <summary>
		/// 总长度；未知时返回 -1
		/// </summary>
		long Length { get; }

		/// <summary>
		/// 当前读写位置
		/// </summary>
		long Position { get; set; }

		/// <summary>
		/// 可读
		/// </summary>
		bool CanRead { get; }

		/// <summary>
		/// 可写
		/// </summary>
		bool CanWrite { get; }

		/// <summary>
		/// 可定位
		/// </summary>
		bool CanSeek { get; }

		/// <summary>
		/// 读取
		/// </summary>
		int Read(byte[] buffer, int offset, int count);

		/// <summary>
		/// 写入
		/// </summary>
		void Write(byte[] buffer, int offset, int count);

		/// <summary>
		/// 定位
		/// </summary>
		long Seek(long offset, SeekOrigin origin);

		/// <summary>
		/// 刷新缓冲
		/// </summary>
		void Flush();
	}
}
