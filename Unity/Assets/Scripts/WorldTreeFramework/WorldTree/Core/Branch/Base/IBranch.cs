/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:58:19

* 描述： 世界树分支基类接口
*
* 分支是树的主体部分之一。
* 除了根节点，其余节点必定属于某个分支，节点自身也会记录属于哪个分支。
*

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
	[TreeDataSerializable]
	public partial interface IBranch : IUnit, IEnumerable<INode>
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
		/// 获取节点键值枚举器
		/// </summary>
		public IEnumerable<KeyValuePair<K, INode>> GetEnumerable();

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

	/// <summary>
	/// 以Id为键值的分支
	/// </summary>
	public interface IBranchIdKey : IBranch<long> { }

	/// <summary>
	/// 以Type为键值的分支
	/// </summary>
	public interface IBranchTypeKey : IBranch<long> { }

	/// <summary>
	/// 以下标为键值的分支
	/// </summary>
	public interface IBranchIndexKey : IBranch<int> { }

	/// <summary>
	/// 分支无约束标记
	/// </summary>
	public interface IBranchUnConstraint { }

	/// <summary>
	/// 节点: 可用分支
	/// </summary>
	/// <typeparam name="B"></typeparam>
	public interface AsBranch<in B> where B : IBranch
	{ }


	/// <summary>
	/// 父节点限制
	/// </summary>
	/// <typeparam name="P">父节点</typeparam>
	public interface NodeOf<in P>
		where P : class, INode
	{

	}
	/// <summary>
	/// 父节点分支限制
	/// </summary>
	/// <typeparam name="P">父节点</typeparam>
	/// <typeparam name="B">分支</typeparam>
	public interface NodeOf<in P, in B> : NodeOf<P>
		where P : class, INode
		where B : class, IBranch
	{ }
}