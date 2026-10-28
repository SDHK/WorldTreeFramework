/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件通道。Channel 下按 Group 分组，Group 内按 Tag 兜底。

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 文件通道
	/// </summary>
	/// <remarks>
	/// Channel 是固定操作码；Group 可为空表示默认组；Tag 在 Group 内兜底。
	/// </remarks>
	public sealed class FileSourceChannel : CoreObject
	{
		/// <summary>
		/// 通道操作码
		/// </summary>
		public FileChannel Channel { get; }

		/// <summary>
		/// Group 到分组对象的映射
		/// </summary>
		private readonly Dictionary<string, FileGroup> groupDict = new Dictionary<string, FileGroup>(StringComparer.Ordinal);

		/// <summary>
		/// Group 表锁
		/// </summary>
		private readonly object groupLock = new object();

		/// <summary>
		/// 是否已释放
		/// </summary>
		private bool disposed;

		/// <summary>
		/// 当前通道内所有注册项（快照）
		/// </summary>
		public IReadOnlyList<FileSourceInfo> Sources
		{
			get
			{
				lock (groupLock)
				{
					List<FileSourceInfo> resultList = new List<FileSourceInfo>();
					foreach (FileGroup group in groupDict.Values)
						resultList.AddRange(group.Sources);
					return resultList.ToArray();
				}
			}
		}

		/// <summary>
		/// 通道内是否没有 Group
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				lock (groupLock) return groupDict.Count == 0;
			}
		}

		public FileSourceChannel(FileChannel channel)
		{
			Channel = channel;
		}

		/// <summary>
		/// 添加一个 Group 下的 Tag/Source
		/// </summary>
		public void AddSource(string group, string tag, IFileSource source, int priority = 0)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			group ??= string.Empty;

			lock (groupLock)
			{
				if (!groupDict.TryGetValue(group, out FileGroup fileGroup))
				{
					fileGroup = new FileGroup(group);
					fileGroup.Core = Core;
					fileGroup.OnCreate();
					groupDict.Add(group, fileGroup);
				}

				fileGroup.AddSource(tag, source, priority);
			}
		}

		/// <summary>
		/// 移除一个 Group 下的 Tag
		/// </summary>
		public bool RemoveSource(string group, string tag)
		{
			group ??= string.Empty;

			lock (groupLock)
			{
				if (!groupDict.TryGetValue(group, out FileGroup fileGroup)) return false;

				bool removed = fileGroup.RemoveSource(tag);
				if (fileGroup.IsEmpty)
				{
					groupDict.Remove(group);
					fileGroup.Dispose();
				}

				return removed;
			}
		}

		/// <summary>
		/// 解析地址对应的 Source
		/// </summary>
		public IFileSource ResolveSource(FileAddress address)
		{
			return RequireGroup(address.Group).ResolveSource(address.Tag);
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(FileAddress address)
		{
			return RequireGroup(address.Group).Exists(address.Tag, address.Key);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(FileAddress address)
		{
			return RequireGroup(address.Group).Stat(address.Tag, address.Key);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(INode parent, FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			return RequireGroup(address.Group).Open(parent, address.Tag, address.Key, mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(INode parent, FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			return RequireGroup(address.Group).OpenAsync(parent, address.Tag, address.Key, mode, access);
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(FileAddress address)
		{
			return RequireGroup(address.Group).List(address.Tag, address.Key);
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(FileAddress address)
		{
			RequireGroup(address.Group).Delete(address.Tag, address.Key);
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public void Move(FileAddress from, FileAddress to)
		{
			FileGroup fromFileGroup = RequireGroup(from.Group);
			FileGroup toFileGroup = RequireGroup(to.Group);

			if (!ReferenceEquals(fromFileGroup, toFileGroup))
				throw new FileException(FileError.NotSupported, from.Key, "跨 Group 移动暂不支持");

			fromFileGroup.Move(from.Tag, from.Key, to.Tag, to.Key);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(FileAddress address)
		{
			RequireGroup(address.Group).CreateDirectory(address.Tag, address.Key);
		}

		/// <summary>
		/// 释放所有 Group
		/// </summary>
		public override void OnDispose()
		{
			if (disposed) return;
			disposed = true;

			lock (groupLock)
			{
				foreach (FileGroup group in groupDict.Values)
					group.Dispose();

				groupDict.Clear();
			}
		}

		/// <summary>
		/// 要求 Group 存在；空字符串表示默认组
		/// </summary>
		private FileGroup RequireGroup(string group)
		{
			group ??= string.Empty;

			lock (groupLock)
			{
				if (groupDict.TryGetValue(group, out FileGroup fileGroup))
					return fileGroup;
			}

			throw new FileException(FileError.NotFound, group, "没有匹配的 Group");
		}
	}
}
