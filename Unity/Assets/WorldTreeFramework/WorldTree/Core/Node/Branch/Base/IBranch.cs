/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:58:19

* 描述： 世界树分支基类接口

*/
using System.Collections.Generic;

namespace WorldTree
{
	
	/// <summary>
	/// 世界树分支接口
	/// </summary>
	/// <remarks>
	/// <para>世界树节点的结构组织接口基类</para> 
	/// </remarks>
	public interface IBranch : IUnitPoolEventItem, IEnumerable<INode>
	{
		/// <summary>
		/// 节点数量
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// 节点id包含判断
		/// </summary>
		public bool ContainsId(long id);

		/// <summary>
		/// 尝试通过id获取节点
		/// </summary>
		public bool TryGetNodeById(long id, out INode node);

		/// <summary>
		/// 通过id获取节点
		/// </summary>
		public INode GetNodeById(long id);

		/// <summary>
		/// 将节点从分支中移除
		/// </summary>
		public void RemoveNode(long nodeId);

		/// <summary>
		/// 清空分支
		/// </summary>
		public void Clear();
	}

	/// <summary>
	/// 世界树分支泛型键值接口
	/// </summary>
	public interface IBranch<K> : IBranch
	{
		/// <summary>
		/// 节点键值包含判断
		/// </summary>
		public bool Contains(K key);

		/// <summary>
		/// 尝试通过节点获取键值
		/// </summary>
		public bool TryGetNodeKey(INode node, out K key);

		/// <summary>
		/// 尝试通过键值获取节点
		/// </summary>
		public bool TryGetNode(K key, out INode node);
		/// <summary>
		/// 通过键值获取节点
		/// </summary>
		public INode GetNode(K key);

		/// <summary>
		/// 尝试添加节点到字典
		/// </summary>
		public bool TryAddNode<N>(K key, N node) where N : class, INode;

	}

	/// <summary>
	/// 世界树藤接口
	/// </summary>
	public interface IRattan
	{
		/// <summary>
		/// 节点数量
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// 节点id包含判断
		/// </summary>
		public bool ContainsId(long id);

		/// <summary>
		/// 尝试通过id获取节点
		/// </summary>
		public bool TryGetNodeById(long id, out INode node);

		/// <summary>
		/// 通过id获取节点
		/// </summary>
		public INode GetNodeById(long id);

		/// <summary>
		/// 将节点从树藤中移除
		/// </summary>
		public void RemoveNode(long nodeId);

		/// <summary>
		/// 清空
		/// </summary>
		public void Clear();
	}

	/// <summary>
	/// 世界树分支泛型键值接口
	/// </summary>
	public interface IRattan<K> : IRattan
	{
		/// <summary>
		/// 节点键值包含判断
		/// </summary>
		public bool Contains(K key);

		/// <summary>
		/// 尝试通过节点获取键值
		/// </summary>
		public bool TryGetNodeKey(INode node, out K key);

		/// <summary>
		/// 尝试通过键值获取节点
		/// </summary>
		public bool TryGetNode(K key, out INode node);
		/// <summary>
		/// 通过键值获取节点
		/// </summary>
		public INode GetNode(K key);

		/// <summary>
		/// 尝试添加节点到字典
		/// </summary>
		public bool TryAddNode<N>(K key, N node) where N : class, INode;

	}
}
