/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 虚拟路径管理器。地址为 channel[/group[/tag]]:key。

*/

using System;
using System.Collections.Generic;
using System.IO;

namespace WorldTree
{
	/// <summary>
	/// 虚拟路径管理器
	/// </summary>
	/// <remarks>
	/// Channel 是固定操作码；Group 可为空；Tag 在 Group 内兜底。
	/// </remarks>
	public class FileManager : CoreObject
	{
		/// <summary>
		/// 通道表：FileChannel -> 通道
		/// </summary>
		private readonly Dictionary<FileChannel, FileSourceChannel> channelDict = new Dictionary<FileChannel, FileSourceChannel>();

		/// <summary>
		/// 通道表锁
		/// </summary>
		private readonly object channelLock = new object();

		/// <summary>
		/// 是否已释放
		/// </summary>
		private bool disposed;

		/// <summary>
		/// 当前所有 Source（快照）
		/// </summary>
		public IReadOnlyList<FileSourceInfo> Sources
		{
			get
			{
				lock (channelLock)
				{
					List<FileSourceInfo> resultList = new List<FileSourceInfo>();
					foreach (FileSourceChannel channel in channelDict.Values)
						resultList.AddRange(channel.Sources);
					return resultList.ToArray();
				}
			}
		}

		/// <summary>
		/// 添加一个文件源
		/// </summary>
		public void AddSource(FileChannel channel, string group, string tag, IFileSource source, int priority = 0)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			lock (channelLock)
			{
				if (!channelDict.TryGetValue(channel, out FileSourceChannel sourceChannel))
				{
					sourceChannel = new FileSourceChannel(channel);
					sourceChannel.Core = Core;
					sourceChannel.OnCreate();
					channelDict.Add(channel, sourceChannel);
				}

				sourceChannel.AddSource(group, tag, source, priority);
			}
		}

		/// <summary>
		/// 移除一个文件源
		/// </summary>
		public bool RemoveSource(FileChannel channel, string group, string tag)
		{
			lock (channelLock)
			{
				if (!channelDict.TryGetValue(channel, out FileSourceChannel sourceChannel)) return false;

				bool removed = sourceChannel.RemoveSource(group, tag);
				if (sourceChannel.IsEmpty)
				{
					channelDict.Remove(channel);
					sourceChannel.Dispose();
				}

				return removed;
			}
		}

		/// <summary>
		/// 解析地址对应的文件源
		/// </summary>
		public IFileSource Resolve(FileAddress address)
		{
			return ResolveChannel(address.Channel).ResolveSource(address);
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(FileAddress address)
		{
			return ResolveChannel(address.Channel).Exists(address);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(FileAddress address)
		{
			return ResolveChannel(address.Channel).Stat(address);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(INode parent, FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			return ResolveChannel(address.Channel).Open(parent, address, mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(INode parent, FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			return ResolveChannel(address.Channel).OpenAsync(parent, address, mode, access);
		}

		/// <summary>
		/// 打开只读文件
		/// </summary>
		public FileHandle OpenRead(INode parent, FileAddress address)
		{
			return Open(parent, address, FileOpenMode.Open, FileOpenAccess.Read);
		}

		/// <summary>
		/// 打开写入文件
		/// </summary>
		public FileHandle OpenWrite(INode parent, FileAddress address)
		{
			return Open(parent, address, FileOpenMode.Create, FileOpenAccess.Write);
		}

		/// <summary>
		/// 读取整个文件
		/// </summary>
		public byte[] ReadAllBytes(INode parent, FileAddress address)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));

			using (FileHandle handle = OpenRead(parent, address))
			{
				long length = handle.Length;
				if (length > int.MaxValue)
					throw new FileException(FileError.IOError, address.Key, "文件过大，无法一次读取");

				if (length >= 0)
				{
					byte[] buffers = new byte[(int)length];
					int offset = 0;
					while (offset < buffers.Length)
					{
						int read = handle.Read(buffers, offset, buffers.Length - offset);
						if (read <= 0) break;
						offset += read;
					}

					if (offset == buffers.Length) return buffers;
					Array.Resize(ref buffers, offset);
					return buffers;
				}

				using (MemoryStream stream = new MemoryStream())
				{
					byte[] chunks = new byte[8192];
					int read;
					while ((read = handle.Read(chunks, 0, chunks.Length)) > 0)
						stream.Write(chunks, 0, read);
					return stream.ToArray();
				}
			}
		}

		/// <summary>
		/// 写入整个文件
		/// </summary>
		public void WriteAllBytes(INode parent, FileAddress address, byte[] data)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			if (data == null) throw new ArgumentNullException(nameof(data));

			using (FileHandle handle = OpenWrite(parent, address))
			{
				if (data.Length > 0) handle.Write(data, 0, data.Length);
				handle.Flush();
			}
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(FileAddress address)
		{
			return ResolveChannel(address.Channel).List(address);
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(FileAddress address)
		{
			ResolveChannel(address.Channel).Delete(address);
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public void Move(FileAddress from, FileAddress to)
		{
			FileSourceChannel fromChannel = ResolveChannel(from.Channel);
			FileSourceChannel toChannel = ResolveChannel(to.Channel);

			if (!ReferenceEquals(fromChannel, toChannel))
				throw new FileException(FileError.NotSupported, from.ToString(), "跨通道移动暂不支持");

			fromChannel.Move(from, to);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(FileAddress address)
		{
			ResolveChannel(address.Channel).CreateDirectory(address);
		}

		/// <summary>
		/// 释放所有通道和 Source
		/// </summary>
		public override void OnDispose()
		{
			if (disposed) return;
			disposed = true;

			lock (channelLock)
			{
				foreach (FileSourceChannel channel in channelDict.Values)
					channel.Dispose();

				channelDict.Clear();
			}
		}

		/// <summary>
		/// 解析通道
		/// </summary>
		private FileSourceChannel ResolveChannel(FileChannel channel)
		{
			lock (channelLock)
			{
				if (channelDict.TryGetValue(channel, out FileSourceChannel sourceChannel))
					return sourceChannel;
			}

			throw new FileException(FileError.NotFound, channel.ToString(), "没有匹配的通道");
		}
	}
}
