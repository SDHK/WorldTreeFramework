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
		/// 解析地址对应的文件源
		/// </summary>
		public IFileSource Resolve(FileAddress address)
		{
			return Manager.Resolve(address);
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(string address)
		{
			return Manager.Exists(FileAddress.Parse(address));
		}

		/// <summary>
		/// 是否存在
		/// </summary>
		public bool Exists(FileAddress address)
		{
			return Manager.Exists(address);
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(string address)
		{
			return Manager.Stat(FileAddress.Parse(address));
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		public FileItemInfo Stat(FileAddress address)
		{
			return Manager.Stat(address);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(string address, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.Open(node, FileAddress.Parse(address), mode, access);
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		public FileHandle Open(FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.Open(node, address, mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(string address, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.OpenAsync(node, FileAddress.Parse(address), mode, access);
		}

		/// <summary>
		/// 异步打开文件
		/// </summary>
		public TreeTask<FileHandle> OpenAsync(FileAddress address, FileOpenMode mode, FileOpenAccess access)
		{
			return Manager.OpenAsync(node, address, mode, access);
		}

		/// <summary>
		/// 打开只读文件
		/// </summary>
		public FileHandle OpenRead(string address)
		{
			return Manager.OpenRead(node, FileAddress.Parse(address));
		}

		/// <summary>
		/// 打开只读文件
		/// </summary>
		public FileHandle OpenRead(FileAddress address)
		{
			return Manager.OpenRead(node, address);
		}

		/// <summary>
		/// 打开写入文件
		/// </summary>
		public FileHandle OpenWrite(string address)
		{
			return Manager.OpenWrite(node, FileAddress.Parse(address));
		}

		/// <summary>
		/// 打开写入文件
		/// </summary>
		public FileHandle OpenWrite(FileAddress address)
		{
			return Manager.OpenWrite(node, address);
		}

		/// <summary>
		/// 读取整个文件
		/// </summary>
		public byte[] ReadAllBytes(string address)
		{
			return Manager.ReadAllBytes(node, FileAddress.Parse(address));
		}

		/// <summary>
		/// 读取整个文件
		/// </summary>
		public byte[] ReadAllBytes(FileAddress address)
		{
			return Manager.ReadAllBytes(node, address);
		}

		/// <summary>
		/// 写入整个文件
		/// </summary>
		public void WriteAllBytes(string address, byte[] data)
		{
			Manager.WriteAllBytes(node, FileAddress.Parse(address), data);
		}

		/// <summary>
		/// 写入整个文件
		/// </summary>
		public void WriteAllBytes(FileAddress address, byte[] data)
		{
			Manager.WriteAllBytes(node, address, data);
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(string address)
		{
			return Manager.List(FileAddress.Parse(address));
		}

		/// <summary>
		/// 枚举目录
		/// </summary>
		public IEnumerable<FileItemInfo> List(FileAddress address)
		{
			return Manager.List(address);
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(string address)
		{
			Manager.Delete(FileAddress.Parse(address));
		}

		/// <summary>
		/// 删除文件或目录
		/// </summary>
		public void Delete(FileAddress address)
		{
			Manager.Delete(address);
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
			Manager.CreateDirectory(FileAddress.Parse(address));
		}

		/// <summary>
		/// 创建目录
		/// </summary>
		public void CreateDirectory(FileAddress address)
		{
			Manager.CreateDirectory(address);
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
