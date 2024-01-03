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
		public int Count => Nodes == null ? 0 : Nodes.Count;
		protected UnitDictionary<K, INode> Nodes;
		protected UnitDictionary<long, K> NodeKeys;

		public override void OnGet()
		{
			Core.PoolGetUnit(out Nodes);
			Core.PoolGetUnit(out NodeKeys);
		}

		public bool Contains(K key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => NodeKeys.ContainsKey(id);

		public bool TryAddNode<N>(K key, N node) where N : class, INode => Nodes.TryAdd(key, node) && NodeKeys.TryAdd(node.Id, key);

		public bool TryGetNodeKey(long nodeId, out K key) => NodeKeys.TryGetValue(nodeId, out key);

		public bool TryGetNode(K key, out INode node) => this.Nodes.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => (node = this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out node) ? node : null) != null;

		public INode GetNode(K key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public void RemoveNode(long nodeId)
		{
			if (NodeKeys.TryGetValue(nodeId, out K key))
			{
				NodeKeys.Remove(nodeId);
				Nodes.Remove(key);
			}
		}

		public void Clear()
		{
			Nodes.Clear();
			NodeKeys.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.NodeKeys.Dispose();
			this.Nodes = null;
			this.NodeKeys = null;
		}
	}
}
