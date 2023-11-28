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

		public void SetNode(INode node)
		{
			if (Self == null)
			{
				Self = node;
				Self.PoolGet(out Nodes);
				Self.PoolGet(out NodeKeys);
			}
		}

		public bool Contains(K key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => NodeKeys.ContainsKey(id);

		public bool TryAddNode<N>(K key, N node) where N : class, INode => Nodes.TryAdd(key, node) && NodeKeys.TryAdd(node.Id, key);

		public bool TryGetNodeKey(INode node, out K key) => NodeKeys.TryGetValue(node.Id, out key);

		public bool TryGetNode(K key, out INode node) => this.Nodes.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => (node = this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out node) ? node : null) != null;

		public INode GetNode(K key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out INode node) ? node : null;


		public void RemoveNodeById(long id) => GetNodeById(id)?.Dispose();

		public void RemoveNode(K key) => GetNode(key)?.Dispose();

		public void RemoveAllNode()
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

		public void RemoveNodeAndBranchDispose(long nodeId)
		{
			if (NodeKeys.TryGetValue(nodeId, out K key))
			{
				NodeKeys.Remove(nodeId);
				Nodes.Remove(key);
			}

			//如果分支字典为空，那么就释放分支字典
			if (Nodes.Count == 0)
			{
				//移除分支自己
				this.Self.m_Branchs.Remove(this.Type);
				//如果分支字典为空，那么就释放分支字典
				if (this.Self.m_Branchs.Count == 0)
				{
					this.Self.m_Branchs.Dispose();
					this.Self.m_Branchs = null;
				}
				//释放分支自己
				this.Dispose();
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
