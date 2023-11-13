/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/28 12:58:19

* 描述： 世界树分支

*/
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 节点的链接约定
	/// </summary>
	/// <typeparam name="N"></typeparam>
	public interface ILink<in N> where N : class, INode { }

	/// <summary>
	/// 分支结构约定
	/// </summary>
	public interface AsBranch<in B> where B : class, IBranch { }

	/// <summary>
	/// 子节点约定
	/// </summary>
	/// <typeparam name="B"></typeparam>
	/// <typeparam name="N"></typeparam>
	public interface AsNode<in B, in N> : INode, AsBranch<B> where B : class, IBranch where N : class, INode { }

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
		/// 尝试通过id获取节点
		/// </summary>
		public bool TryGetNodeById(long id, out INode Node);

		/// <summary>
		/// 移除一个节点
		/// </summary>
		/// <remarks>单纯的将节点从分支字典中移除，不是释放和裁剪节点</remarks>
		public void RemoveNode(INode node);
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
		/// 尝试通过节点获取键值
		/// </summary>
		public bool TryGetNodeKey(INode node, out K key);

		/// <summary>
		/// 尝试通过键值获取节点
		/// </summary>
		public bool TryGetNode(K key, out INode Node);

		/// <summary>
		/// 尝试添加组件
		/// </summary>
		/// <remarks>从池里获取</remarks>
		/// <typeparam name="N">节点类型</typeparam>
		/// <param name="key">键值</param>
		/// <param name="Node">节点</param>
		public bool TryAddNode<N>(K key, out N Node, bool isPool = true) where N : class, INode;

		/// <summary>
		/// 尝试接入外部树结构
		/// </summary>
		public bool TryGraftNode(K key, INode node);
	}

	/// <summary>
	/// 世界树分支基类
	/// </summary>
	public abstract class Branch<K> : UnitPoolItem, IBranch<K>
	{
		public INode Self { get; set; }
		public int Count => Nodes.Count;
		public UnitDictionary<K, INode> Nodes;
		public UnitDictionary<long, K> NodeKeys;

		public virtual bool TryAddNode<N>(K key, out N Node, bool isPool = true) where N : class, INode
		{
			if (Nodes.TryGetValue(key, out INode node))
			{
				node = isPool ? Self.Core.GetNode(TypeInfo<N>.HashCode64) : Self.Core.NewNodeLifecycle(TypeInfo<N>.HashCode64);
				NodeKeys.Add(node.Id, key);
				node.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				node.Parent = Self;
				node.Core = Self.Core;
				node.Root = Self.Root;
				if (node.Domain != node) node.Domain = Self.Domain;
				Nodes.Add(key, node);
			}
			Node = node as N;
			return Node != null;
		}
		public virtual bool TryGraftNode(K key, INode node)
		{
			if (Nodes.TryAdd(key, node))
			{
				NodeKeys.Add(node.Id, key);
				node.BranchType = TypeInfo<ComponentBranch>.HashCode64;
				node.Parent = Self;
				return true;
			}
			return false;
		}

		public virtual bool TryGetNodeKey(INode node, out K key) => NodeKeys.TryGetValue(node.Id, out key);
		public virtual void RemoveNode(INode node)
		{
			if (NodeKeys.TryGetValue(node.Id, out K key))
			{
				NodeKeys.Remove(node.Id);
				Nodes.Remove(key);
			}
		}
		public virtual void RemoveAllNode()
		{
			if (Nodes.Count == 0) return;
			using (Self.PoolGet(out UnitStack<INode> nodes))
			{
				foreach (var item in Nodes) nodes.Push(item.Value);
				int length = nodes.Count;
				for (int i = 0; i < length; i++) nodes.Pop().Dispose();
			}
		}

		public void SetNode(INode node)
		{
			if (Self == null)
			{
				Self = node;
				Self.PoolGet(out Nodes);
				Self.PoolGet(out NodeKeys);
			}
		}

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.NodeKeys.Dispose();
			this.Self = null;
			this.Nodes = null;
			this.NodeKeys = null;
			base.OnRecycle();
		}

		public bool TryGetNode(K key, out INode Node) => this.Nodes.TryGetValue(key, out Node);

		public bool TryGetNodeById(long id, out INode Node)
		{
			if (NodeKeys.TryGetValue(id, out K key)) return this.Nodes.TryGetValue(key, out Node);
			Node = null;
			return false;
		}

		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();
	}

}
