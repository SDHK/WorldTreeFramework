/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 文件内容分组。Group 内按 Tag 兜底，Tag 内按 Priority 排序。

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 文件内容分组
	/// </summary>
	/// <remarks>
	/// Tag 是逻辑选择标签（版本/别名/备用路径）。
	/// 读取时先试指定 Tag，再按 fallbackTagsList 顺序兜底；写入必须指定精确 Tag。
	/// </remarks>
	public sealed class FileGroup : CoreObject
	{
		/// <summary>
		/// 分组名
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Tag 到 Source 链的映射
		/// </summary>
		private readonly Dictionary<string, List<FileSourceInfo>> tagDict = new Dictionary<string, List<FileSourceInfo>>(StringComparer.Ordinal);

		/// <summary>
		/// Tag 兜底顺序
		/// </summary>
		private readonly List<string> fallbackTagsList = new List<string>();

		/// <summary>
		/// Tag 表锁
		/// </summary>
		private readonly object tagLock = new object();

		/// <summary>
		/// 是否已释放
		/// </summary>
		private bool disposed;

		/// <summary>
		/// 当前分组内所有注册项（快照）
		/// </summary>
		public IReadOnlyList<FileSourceInfo> Sources
		{
			get
			{
				lock (tagLock)
				{
					List<FileSourceInfo> resultList = new List<FileSourceInfo>();
					foreach (List<FileSourceInfo> sourcesList in tagDict.Values)
						resultList.AddRange(sourcesList);
					return resultList.ToArray();
				}
			}
		}

		/// <summary>
		/// 分组内是否没有 Source
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				lock (tagLock) return tagDict.Count == 0;
			}
		}

		public FileGroup(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Group 不能为空", nameof(name));

			Name = name;
		}

		/// <summary>
		/// 添加一个 Tag 下的 Source
		/// </summary>
		public void AddSource(string tag, IFileSource source, int priority = 0)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			tag ??= string.Empty;
			FileSourceInfo info = new FileSourceInfo(tag, source, priority);

			lock (tagLock)
			{
				if (!tagDict.TryGetValue(tag, out List<FileSourceInfo> sourcesList))
				{
					sourcesList = new List<FileSourceInfo>();
					tagDict.Add(tag, sourcesList);
					fallbackTagsList.Add(tag);
				}

				int index = 0;
				while (index < sourcesList.Count && sourcesList[index].Priority <= priority) index++;
				sourcesList.Insert(index, info);
			}
		}

		/// <summary>
		/// 移除一个 Tag 下的所有 Source
		/// </summary>
		public bool RemoveSource(string tag)
		{
			tag ??= string.Empty;

			List<IFileSource> sourcesList = new List<IFileSource>();

			lock (tagLock)
			{
				if (!tagDict.TryGetValue(tag, out List<FileSourceInfo> infoList)) return false;

				for (int i = 0; i < infoList.Count; i++)
				{
					IFileSource source = infoList[i].Source;
					if (!sourcesList.Contains(source)) sourcesList.Add(source);
				}

				tagDict.Remove(tag);
				fallbackTagsList.Remove(tag);

				for (int i = sourcesList.Count - 1; i >= 0; i--)
				{
					if (ContainsSource(sourcesList[i])) sourcesList.RemoveAt(i);
				}
			}

			for (int i = 0; i < sourcesList.Count; i++) sourcesList[i].Dispose();
			return true;
		}

		/// <summary>
		/// 设置 Tag 兜底顺序；只保留已存在的 Tag
		/// </summary>
		public void SetFallbackTags(IReadOnlyList<string> tags)
		{
			if (tags == null) return;

			lock (tagLock)
			{
				fallbackTagsList.Clear();
				for (int i = 0; i < tags.Count; i++)
				{
					string tag = tags[i] ?? string.Empty;
					if (!tagDict.ContainsKey(tag) || fallbackTagsList.Contains(tag)) continue;
					fallbackTagsList.Add(tag);
				}
			}
		}

		/// <summary>
		/// 解析 Tag 对应的 Source；未命中时按 Tag 链兜底
		/// </summary>
		public IFileSource ResolveSource(string tag)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				if (sourcesList.Count > 0) return sourcesList[0].Source;
			}

			throw new FileException(FileError.NotFound, tag, "没有匹配的 Source");
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(string tag, string key)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				for (int j = 0; j < sourcesList.Count; j++)
					if (sourcesList[j].Source.Exists(key)) return true;
			}

			return false;
		}

		/// <summary>
		/// 获取文件信息；不存在时返回 default
		/// </summary>
		public FileInfo Stat(string tag, string key)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				for (int j = 0; j < sourcesList.Count; j++)
				{
					FileInfo result = sourcesList[j].Source.Stat(key);
					if (result.Exists) return result;
				}
			}

			return default;
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(INode parent, string tag, string key, FileOpenMode mode, FileOpenAccess access)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				for (int j = 0; j < sourcesList.Count; j++)
				{
					if (!sourcesList[j].Source.Exists(key)) continue;
					return sourcesList[j].Source.Open(parent, key, mode, access);
				}
			}

			throw new FileException(FileError.NotFound, key, "没有匹配的 Source");
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(INode parent, string tag, string key, FileOpenMode mode, FileOpenAccess access)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				for (int j = 0; j < sourcesList.Count; j++)
				{
					if (!sourcesList[j].Source.Exists(key)) continue;
					return sourcesList[j].Source.OpenAsync(parent, key, mode, access);
				}
			}

			throw new FileException(FileError.NotFound, key, "没有匹配的 Source");
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileInfo> List(string tag, string key)
		{
			List<string> candidatesList = GetTagCandidates(tag);
			for (int i = 0; i < candidatesList.Count; i++)
			{
				if (!TryGetTagSources(candidatesList[i], out List<FileSourceInfo> sourcesList)) continue;
				for (int j = 0; j < sourcesList.Count; j++)
				{
					List<FileInfo> entriesList = new List<FileInfo>(sourcesList[j].Source.List(key));
					if (entriesList.Count > 0) return entriesList;
				}
			}

			return Array.Empty<FileInfo>();
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(string tag, string key)
		{
			RequireExactSource(tag).Delete(key);
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public void Move(string fromTag, string fromKey, string toTag, string toKey)
		{
			IFileSource fromSource = RequireExactSource(fromTag);
			IFileSource toSource = RequireExactSource(toTag);

			if (!ReferenceEquals(fromSource, toSource))
				throw new FileException(FileError.NotSupported, fromKey, "跨 Source 移动暂不支持");

			fromSource.Move(fromKey, toKey);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(string tag, string key)
		{
			RequireExactSource(tag).CreateDirectory(key);
		}

		/// <summary>
		/// 释放所有 Source
		/// </summary>
		public override void OnDispose()
		{
			if (disposed) return;
			disposed = true;

			List<IFileSource> sourcesList = new List<IFileSource>();
			lock (tagLock)
			{
				foreach (List<FileSourceInfo> infoList in tagDict.Values)
				{
					for (int i = 0; i < infoList.Count; i++)
					{
						IFileSource source = infoList[i].Source;
						if (!sourcesList.Contains(source)) sourcesList.Add(source);
					}
				}

				tagDict.Clear();
				fallbackTagsList.Clear();
			}

			for (int i = 0; i < sourcesList.Count; i++) sourcesList[i].Dispose();
		}

		/// <summary>
		/// 取 Tag 候选顺序
		/// </summary>
		private List<string> GetTagCandidates(string tag)
		{
			tag ??= string.Empty;

			lock (tagLock)
			{
				List<string> candidatesList = new List<string>();

				if (tag != "*" && tag.Length != 0 && tagDict.ContainsKey(tag))
					candidatesList.Add(tag);

				for (int i = 0; i < fallbackTagsList.Count; i++)
				{
					string fallbackTag = fallbackTagsList[i];
					if (candidatesList.Contains(fallbackTag)) continue;
					candidatesList.Add(fallbackTag);
				}

				return candidatesList;
			}
		}

		/// <summary>
		/// 取某个 Tag 的 Source 链快照
		/// </summary>
		private bool TryGetTagSources(string tag, out List<FileSourceInfo> sourcesList)
		{
			lock (tagLock)
			{
				if (tagDict.TryGetValue(tag ?? string.Empty, out List<FileSourceInfo> infoList))
				{
					sourcesList = new List<FileSourceInfo>(infoList);
					return true;
				}
			}

			sourcesList = null;
			return false;
		}

		/// <summary>
		/// 写操作要求精确 Tag
		/// </summary>
		private IFileSource RequireExactSource(string tag)
		{
			tag ??= string.Empty;

			lock (tagLock)
			{
				if (tag != "*" && tagDict.TryGetValue(tag, out List<FileSourceInfo> sourcesList) && sourcesList.Count > 0)
					return sourcesList[0].Source;
			}

			throw new FileException(FileError.NotSupported, tag, "写操作必须指定精确存在的 Tag");
		}

		/// <summary>
		/// 分组内是否还引用这个 Source
		/// </summary>
		private bool ContainsSource(IFileSource source)
		{
			foreach (List<FileSourceInfo> sourcesList in tagDict.Values)
			{
				for (int i = 0; i < sourcesList.Count; i++)
				{
					if (ReferenceEquals(sourcesList[i].Source, source)) return true;
				}
			}

			return false;
		}
	}
}
