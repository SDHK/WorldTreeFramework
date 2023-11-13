/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/13 03:52:45

* 描述： 世界树分支基类
* 

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 世界树分支基类
	/// </summary>
	public abstract class Branch<K> : UnitPoolItem, IBranch<K>
	{
		public INode Self { get; set; }
		public int Count => Nodes.Count;
		protected UnitDictionary<K, INode> Nodes;
		protected UnitDictionary<long, K> NodeKeys;

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
