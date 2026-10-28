/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 本地文件源。根目录 + 相对路径映射到 FileStream。

*/

using System;
using System.Collections.Generic;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 本地文件源
	/// </summary>
	public sealed class LocalFileSource : FileSource
	{
		/// <summary>
		/// 根目录（绝对路径，带分隔符）
		/// </summary>
		private readonly string rootPath;

		/// <summary>
		/// 源名称
		/// </summary>
		private readonly string name;

		public LocalFileSource(string rootPath, string name = "local")
		{
			if (string.IsNullOrWhiteSpace(rootPath))
				throw new ArgumentException("根目录不能为空", nameof(rootPath));

			this.rootPath = Path.GetFullPath(rootPath);
			if (!this.rootPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				this.rootPath += Path.DirectorySeparatorChar;

			this.name = name ?? "local";
		}

		/// <summary>
		/// 源名称
		/// </summary>
		public override string Name => name;

		/// <summary>
		/// 能力
		/// </summary>
		public override FileCapabilities Capabilities =>
			FileCapabilities.Read | FileCapabilities.Write | FileCapabilities.Seek | FileCapabilities.List;

		/// <summary>
		/// 是否存在
		/// </summary>
		public override bool Exists(string path)
		{
			string fullPath = GetFullPath(path);
			return File.Exists(fullPath) || Directory.Exists(fullPath);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public override FileItemInfo Stat(string path)
		{
			string fullPath = GetFullPath(path);

			if (File.Exists(fullPath))
			{
				System.IO.FileInfo info = new System.IO.FileInfo(fullPath);
				return new FileItemInfo(path, info.Length, false, info.LastWriteTimeUtc);
			}

			if (Directory.Exists(fullPath))
			{
				System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(fullPath);
				return new FileItemInfo(path, 0, true, info.LastWriteTimeUtc);
			}

			return default;
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public override FileHandle Open(INode parent, string path, FileOpenMode mode, FileOpenAccess access)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			if (string.IsNullOrEmpty(path))
				throw new FileException(FileError.InvalidPath, path, "文件路径不能为空");

			string fullPath = GetFullPath(path);
			FileMode fileMode = ToFileMode(mode);
			FileAccess fileAccess = ToFileAccess(access);

			FileStream stream = new FileStream(fullPath, fileMode, fileAccess, FileShare.Read);
			try
			{
				parent.AddTemp(out StreamFileHandle handle);
				handle.Bind(stream);
				return handle;
			}
			catch
			{
				stream.Dispose();
				throw;
			}
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public override TreeTask<FileHandle> OpenAsync(INode parent, string path, FileOpenMode mode, FileOpenAccess access)
		{
			TreeTask<FileHandle> task = TreeTaskManager.Get(parent).AddTemp(out TreeTask<FileHandle> _);
			task.SetResult(Open(parent, path, mode, access));
			return task;
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public override IEnumerable<FileItemInfo> List(string path)
		{
			string fullPath = GetFullPath(path);
			if (!Directory.Exists(fullPath)) yield break;

			string relativeRoot = path ?? string.Empty;
			relativeRoot = relativeRoot.Replace('\\', '/').Trim('/');

			foreach (string entry in Directory.EnumerateFileSystemEntries(fullPath))
			{
				string entryName = Path.GetFileName(entry);
				string entryPath = relativeRoot.Length == 0 ? entryName : relativeRoot + "/" + entryName;

				if (File.Exists(entry))
				{
					System.IO.FileInfo info = new System.IO.FileInfo(entry);
					yield return new FileItemInfo(entryPath, info.Length, false, info.LastWriteTimeUtc);
				}
				else if (Directory.Exists(entry))
				{
					System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(entry);
					yield return new FileItemInfo(entryPath, 0, true, info.LastWriteTimeUtc);
				}
			}
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public override void Delete(string path)
		{
			string fullPath = GetFullPath(path);

			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
				return;
			}

			if (Directory.Exists(fullPath))
			{
				Directory.Delete(fullPath, true);
				return;
			}

			throw new FileException(FileError.NotFound, path);
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public override void Move(string from, string to)
		{
			string fromPath = GetFullPath(from);
			string toPath = GetFullPath(to);

			if (File.Exists(fromPath))
			{
				File.Move(fromPath, toPath);
				return;
			}

			if (Directory.Exists(fromPath))
			{
				Directory.Move(fromPath, toPath);
				return;
			}

			throw new FileException(FileError.NotFound, from);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public override void CreateDirectory(string path)
		{
			Directory.CreateDirectory(GetFullPath(path));
		}

		/// <summary>
		/// 把相对路径解析为根目录下的绝对路径，并阻止越界
		/// </summary>
		private string GetFullPath(string path)
		{
			if (string.IsNullOrEmpty(path)) return rootPath;

			string relativePath = path.Replace('\\', '/').TrimStart('/');
			string fullPath = Path.GetFullPath(Path.Combine(rootPath, relativePath));

			if (!fullPath.StartsWith(rootPath, StringComparison.Ordinal))
				throw new FileException(FileError.InvalidPath, path, "路径越界");

			return fullPath;
		}

		/// <summary>
		/// 映射打开模式
		/// </summary>
		private static FileMode ToFileMode(FileOpenMode mode)
		{
			switch (mode)
			{
				case FileOpenMode.Create:
					return FileMode.Create;
				case FileOpenMode.CreateNew:
					return FileMode.CreateNew;
				case FileOpenMode.Append:
					return FileMode.Append;
				case FileOpenMode.Truncate:
					return FileMode.Truncate;
				default:
					return FileMode.Open;
			}
		}

		/// <summary>
		/// 映射访问方式
		/// </summary>
		private static FileAccess ToFileAccess(FileOpenAccess access)
		{
			bool read = (access & FileOpenAccess.Read) != 0;
			bool write = (access & FileOpenAccess.Write) != 0;

			if (read && write) return FileAccess.ReadWrite;
			if (write) return FileAccess.Write;
			return FileAccess.Read;
		}
	}
}
