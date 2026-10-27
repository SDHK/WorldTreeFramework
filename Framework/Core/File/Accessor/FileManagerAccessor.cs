/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/27 0:00

* 描述： 节点绑定的 FileManager 访问器。转接 FileManager 全部方法。

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 节点绑定的 FileManager 访问器
	/// </summary>
	/// <remarks>
	/// 不暴露全局 FileManager；所有打开操作自动把当前 Node 作为句柄 parent。
	/// </remarks>
	public readonly struct FileManagerAccessor
	{
		/// <summary>
		/// 绑定的节点
		/// </summary>
		private readonly INode node;

		/// <summary>
		/// 全局 FileManager
		/// </summary>
		private FileManager Manager => node.World.Line.Core.FileManager;

		public FileManagerAccessor(INode node)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			this.node = node;
		}

		/// <summary>
		/// 当前所有 Source（快照）
		/// </summary>
		public IReadOnlyList<FileSourceInfo> Sources => Manager.Sources;

		/// <summary>
		/// 添加一个文件源
		/// </summary>
		public void AddSource(FileChannel channel, string group, string tag, IFileSource source, int priority = 0)
		{
			Manager.AddSource(channel, group, tag, source, priority);
		}

		/// <summary>
		/// 移除一个文件源
		/// </summary>
		public bool RemoveSource(FileChannel channel, string group, string tag)
		{
			return Manager.RemoveSource(channel, group, tag);
		}

		/// <summary>
		/// 解析 Group/Tag 对应的文件源
		/// </summary>
		public IFileSource Resolve(FileChannel channel, string group, string tag)
		{
			return Manager.Resolve(channel, group, tag);
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.Exists(fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(FileChannel channel, string group, string tag, string key)
		{
			return Manager.Exists(channel, group, tag, key);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.Stat(fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(FileChannel channel, string group, string tag, string key)
		{
			return Manager.Stat(channel, group, tag, key);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(string address, FileOpenMode mode, FileOpenAccess access)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.Open(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key, mode, access);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(FileChannel channel, string group, string tag, string key, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.Open(node, channel, group, tag, key, mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(string address, FileOpenMode mode, FileOpenAccess access)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.OpenAsync(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key, mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(FileChannel channel, string group, string tag, string key, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.OpenAsync(node, channel, group, tag, key, mode, access);
		}

		/// <summary>
		/// 打开只读文件
		/// </summary>
		public FileHandle OpenRead(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.OpenRead(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 打开只读文件
		/// </summary>
		public FileHandle OpenRead(FileChannel channel, string group, string tag, string key)
		{
			return Manager.OpenRead(node, channel, group, tag, key);
		}

		/// <summary>
		/// 打开写入文件
		/// </summary>
		public FileHandle OpenWrite(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.OpenWrite(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 打开写入文件
		/// </summary>
		public FileHandle OpenWrite(FileChannel channel, string group, string tag, string key)
		{
			return Manager.OpenWrite(node, channel, group, tag, key);
		}

		/// <summary>
		/// 读取整个文件
		/// </summary>
		public byte[] ReadAllBytes(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.ReadAllBytes(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 读取整个文件
		/// </summary>
		public byte[] ReadAllBytes(FileChannel channel, string group, string tag, string key)
		{
			return Manager.ReadAllBytes(node, channel, group, tag, key);
		}

		/// <summary>
		/// 写入整个文件
		/// </summary>
		public void WriteAllBytes(string address, byte[] data)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			Manager.WriteAllBytes(node, fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key, data);
		}

		/// <summary>
		/// 写入整个文件
		/// </summary>
		public void WriteAllBytes(FileChannel channel, string group, string tag, string key, byte[] data)
		{
			Manager.WriteAllBytes(node, channel, group, tag, key, data);
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			return Manager.List(fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(FileChannel channel, string group, string tag, string key)
		{
			return Manager.List(channel, group, tag, key);
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			Manager.Delete(fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(FileChannel channel, string group, string tag, string key)
		{
			Manager.Delete(channel, group, tag, key);
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public void Move(string from, string to)
		{
			Manager.Move(FileAddress.Parse(from), FileAddress.Parse(to));
		}

		/// <summary>
		/// 移动文件或目录
		/// </summary>
		public void Move(FileAddress from, FileAddress to)
		{
			Manager.Move(from, to);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(string address)
		{
			FileAddress fileAddress = FileAddress.Parse(address);
			Manager.CreateDirectory(fileAddress.Channel, fileAddress.Group, fileAddress.Tag, fileAddress.Key);
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(FileChannel channel, string group, string tag, string key)
		{
			Manager.CreateDirectory(channel, group, tag, key);
		}
	}

	/// <summary>
	/// 节点获取 FileManagerAccessor 的扩展
	/// </summary>
	public static class NodeFileManagerExtension
	{
		/// <summary>
		/// 获取节点绑定的 FileManagerAccessor
		/// </summary>
		public static FileManagerAccessor GetFileManager(this INode self)
		{
			if (self == null) throw new ArgumentNullException(nameof(self));
			return new FileManagerAccessor(self);
		}
	}
}
