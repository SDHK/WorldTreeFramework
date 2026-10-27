/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 业务用文件句柄节点抽象基类。继承它即获得 Node 能力和句柄契约，
*       具体资源由子类保存，并在 OnDispose 中释放。

*/

using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 业务用文件句柄节点抽象基类
	/// </summary>
	/// <remarks>
	/// 只定义句柄契约和节点生命周期，不持有底层句柄。
	/// 具体子类由 source 提供，自行保存底层资源，并在 OnDispose 中释放后调用 base.OnDispose()。
	/// 节点还池后 <see cref="NodeRef{N}"/> 会自动置空。
	/// </remarks>
	public abstract class FileHandle : Node, IFileHandle
	{
		/// <summary>
		/// 总长度；未知时返回 -1
		/// </summary>
		public abstract long Length { get; }

		/// <summary>
		/// 当前读写位置
		/// </summary>
		public abstract long Position { get; set; }

		/// <summary>
		/// 可读
		/// </summary>
		public abstract bool CanRead { get; }

		/// <summary>
		/// 可写
		/// </summary>
		public abstract bool CanWrite { get; }

		/// <summary>
		/// 可定位
		/// </summary>
		public abstract bool CanSeek { get; }

		/// <summary>
		/// 读取
		/// </summary>
		public abstract int Read(byte[] buffer, int offset, int count);

		/// <summary>
		/// 写入
		/// </summary>
		public abstract void Write(byte[] buffer, int offset, int count);

		/// <summary>
		/// 定位
		/// </summary>
		public abstract long Seek(long offset, SeekOrigin origin);

		/// <summary>
		/// 刷新缓冲
		/// </summary>
		public abstract void Flush();
	}
}
