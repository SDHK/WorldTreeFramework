/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件元数据信息。

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 文件元数据信息
	/// </summary>
	/// <remarks>
	/// <see cref="IFileSource"/> 返回的 Path 是挂载点内的相对路径；
	/// 经 <see cref="FileManager"/> 返回时会被补上挂载前缀。
	/// default(FileInfo) 表示不存在。
	/// </remarks>
	public readonly struct FileInfo
	{
		/// <summary>
		/// 是否存在；default 时为 false
		/// </summary>
		public bool Exists { get; }

		/// <summary>
		/// 路径
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// 大小（字节）
		/// </summary>
		public long Size { get; }

		/// <summary>
		/// 是否目录
		/// </summary>
		public bool IsDirectory { get; }

		/// <summary>
		/// 最后写入时间（UTC）
		/// </summary>
		public DateTime LastWriteTime { get; }

		/// <summary>
		/// 内容版本标识（HTTP ETag / 内容哈希 / 包版本）
		/// </summary>
		public string ETag { get; }

		public FileInfo(string path, long size = 0, bool isDirectory = false, DateTime lastWriteTime = default, string etag = null)
		{
			Exists = true;
			Path = path;
			Size = size;
			IsDirectory = isDirectory;
			LastWriteTime = lastWriteTime;
			ETag = etag;
		}

		public override string ToString()
		{
			if (!Exists) return "<not found>";
			return $"{Path} ({Size}){(IsDirectory ? " [dir]" : string.Empty)}";
		}
	}
}
