/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 单个挂载点的文件源契约。

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 单个挂载点的文件源契约
	/// </summary>
	/// <remarks>
	/// 所有 path 都是挂载点内的相对路径，不带 scheme 前缀；
	/// <see cref="FileItemInfo.Path"/> 同样返回相对路径。
	/// 同步源实现 <see cref="OpenAsync"/> 时直接返回已完成的 <see cref="TreeTask{T}"/>。
	/// </remarks>
	public interface IFileSource : IDisposable
	{
		/// <summary>
		/// 源名称，用于日志和调试
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 能力标志
		/// </summary>
		FileCapabilities Capabilities { get; }

		/// <summary>
		/// 是否存在
		/// </summary>
		bool Exists(string path);

		/// <summary>
		/// 获取文件信息；不存在时返回 default（Exists == false）
		/// </summary>
		FileItemInfo Stat(string path);

		/// <summary>
		/// 在 parent 下创建 FileHandle 并打开文件
		/// </summary>
		FileHandle Open(INode parent, string path, FileOpenMode mode, FileOpenAccess access);

		/// <summary>
		/// 在 parent 下创建 FileHandle 并异步打开文件
		/// </summary>
		TreeTask<FileHandle> OpenAsync(INode parent, string path, FileOpenMode mode, FileOpenAccess access);

		/// <summary>
		/// 枚举目录；不支持时抛 <see cref="FileException"/>（NotSupported）
		/// </summary>
		IEnumerable<FileItemInfo> List(string path);

		/// <summary>
		/// 删除文件或目录；只读源抛 NotSupported
		/// </summary>
		void Delete(string path);

		/// <summary>
		/// 移动文件或目录；只读源抛 NotSupported
		/// </summary>
		void Move(string from, string to);

		/// <summary>
		/// 创建目录；只读源抛 NotSupported
		/// </summary>
		void CreateDirectory(string path);
	}
}
