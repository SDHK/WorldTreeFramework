/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:58:19

* 描述： 世界树分支基类接口

*/
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 节点限制
	/// </summary>
	/// <typeparam name="P">父节点</typeparam>
	/// <typeparam name="B">分支</typeparam>
	public interface NodeOf<in P, in B> : INode where P : class, INode where B : class, IBranch { }

	/// <summary>
	/// 世界树分支接口
	/// </summary>
	/// <remarks>
	/// <para>世界树节点的结构组织接口基类</para> 
	/// </remarks>
	public interface IBranch : IUnitPoolEventItem, IEnumerable<INode>
	{
		/// <summary>
		/// 自身节点
		/// </summary>
		public INode Self { get; set; }

		/// <summary>
		/// 节点数量
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// 设置挂载节点
		/// </summary>
		public void SetNode(INode node);


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
		/// 将节点从分支字典中移除
		/// </summary>
		/// <remarks>单纯的将节点从分支字典中移除，不是释放和裁剪节点</remarks>
		public void RemoveNodeInDictionary(INode node);

		/// <summary>
		/// 根据id移除节点
		/// </summary>
		public void RemoveNodeById(long id);

		/// <summary>
		/// 移除释放所有分支节点
		/// </summary>
		public void RemoveAllNode();
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

		/// <summary>
		/// 根据键值移除节点
		/// </summary>
		public void RemoveNode(K key);
	}
}
