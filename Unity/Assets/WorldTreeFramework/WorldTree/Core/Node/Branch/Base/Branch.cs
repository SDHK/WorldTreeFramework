/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/13 03:52:45

* 描述： 世界树分支基类
* 

*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

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

		public void SetNode(INode node)
		{
			if (Self == null)
			{
				Self = node;
				Self.PoolGet(out Nodes);
				Self.PoolGet(out NodeKeys);
			}
		}

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

		public virtual bool TryGetNode(K key, out INode node) => this.Nodes.TryGetValue(key, out node);

		public virtual bool TryGetNodeById(long id, out INode node) => (node = this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out node) ? node : null) != null;

		public virtual INode GetNode(K key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public virtual INode GetNodeById(long id) => this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out INode node) ? node : null;


		public virtual bool TryCutNode(K key, out INode node) => (node = this.GetNode(key)?.TreeCutSelf()) != null;

		public virtual bool TryCutNodeById(long id, out INode node) => (node = this.GetNodeById(id)?.TreeCutSelf()) != null;

		public virtual INode CutNode(K key) => this.GetNode(key)?.TreeCutSelf();

		public virtual INode CutNodeById(long id) => this.GetNodeById(id)?.TreeCutSelf();


		public virtual void RemoveNodeById(long id) => GetNodeById(id)?.Dispose();

		public virtual void RemoveNode(K key) => GetNode(key)?.Dispose();

		public virtual void RemoveAllNode()
		{
			if (Nodes.Count == 0) return;
			//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
			using (Self.PoolGet(out UnitStack<INode> nodes))
			{
				foreach (var item in Nodes) nodes.Push(item.Value);
				while (nodes.Count != 0) nodes.Pop().Dispose();
			}
			//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
			if (Nodes.Count != 0)
			{
				foreach (var item in Nodes)
				{
					World.LogError($"移除节点出错，意外的新节点，分支:{this.GetType()} 节点:{item.Value.GetType()}:{item.Value.Id}");
				}
			}
		}

		public virtual void RemoveNodeInDictionary(INode node)
		{
			if (NodeKeys.TryGetValue(node.Id, out K key))
			{
				NodeKeys.Remove(node.Id);
				Nodes.Remove(key);
			}
		}


		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.NodeKeys.Dispose();
			this.Self = null;
			this.Nodes = null;
			this.NodeKeys = null;
			base.OnRecycle();
		}
	}
}
