/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件源基类。提供 CoreObject 生命周期，具体读取通道由子类实现。

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 文件源基类
	/// </summary>
	/// <remarks>
	/// 适合作为本地目录、包、YooAsset、HTTP 等读取通道的基类。
	/// 需要节点树能力时，可另行实现 <see cref="IFileSource"/> 并继承 Node。
	/// </remarks>
	public abstract class FileSource : CoreObject, IFileSource
	{
		/// <summary>
		/// 源名称，用于日志和调试
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// 能力标志
		/// </summary>
		public abstract FileCapabilities Capabilities { get; }

		/// <summary>
		/// 是否存在
		/// </summary>
		public abstract bool Exists(string path);

		/// <summary>
		/// 获取文件信息；不存在时返回 default（Exists == false）
		/// </summary>
		public abstract FileInfo Stat(string path);

		/// <summary>
		/// 在 parent 下创建 FileHandle 并打开文件
		/// </summary>
		public abstract FileHandle Open(INode parent, string path, FileOpenMode mode, FileOpenAccess access);

		/// <summary>
		/// 在 parent 下创建 FileHandle 并异步打开文件
		/// </summary>
		public abstract TreeTask<FileHandle> OpenAsync(INode parent, string path, FileOpenMode mode, FileOpenAccess access);

		/// <summary>
		/// 枚举目录
		/// </summary>
		public abstract IEnumerable<FileInfo> List(string path);

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public abstract void Delete(string path);

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public abstract void Move(string from, string to);

		/// <summary>
		/// 创建目录
		/// </summary>
		public abstract void CreateDirectory(string path);
	}
}
