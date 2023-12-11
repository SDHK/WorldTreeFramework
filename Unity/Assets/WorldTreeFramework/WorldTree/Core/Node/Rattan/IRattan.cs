/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/04 07:40:53

* 描述： 世界树藤基类接口
* 
* 功能和分支很像，接口功能都一样，
* 但是分支是树的主体结构，是绝对的树状结构。
* 而藤是树的关系结构，它可以连接任意层级任意分支任意节点。
* 
* 节点双向记录引用是可以做，但是不现实的，因此需要依靠NodeRef，
* 所以藤结构它需要处理节点意外丢失的情况。
* 

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 世界树藤接口
	/// </summary>
	public interface IRattan : IUnitPoolEventItem, IEnumerable<INode>
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
	/// 世界树藤泛型键值接口
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
		public bool TryGetNodeKey(long nodeId, out K key);

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
