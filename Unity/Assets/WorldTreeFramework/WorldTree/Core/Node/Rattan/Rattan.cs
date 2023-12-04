/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/04 07:54:45

* 描述： 世界树藤基类
* 

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 世界树藤基类
	/// </summary>
	public class Rattan<K> : UnitPoolItem, IRattan<K>
	{
		public int Count => Nodes.Count;
		protected UnitDictionary<K, NodeRef<INode>> Nodes;
		protected UnitDictionary<long, K> NodeKeys;

		public override void OnGet()
		{
			Core.PoolGet(out Nodes);
			Core.PoolGet(out NodeKeys);
		}

		public bool Contains(K key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => NodeKeys.ContainsKey(id);

		public bool TryGetNodeKey(long nodeId, out K key) => NodeKeys.TryGetValue(nodeId, out key);


		public bool TryAddNode<N>(K key, N node) where N : class, INode => Nodes.TryAdd(key, node) && NodeKeys.TryAdd(node.Id, key);


		public bool TryGetNode(K key, out NodeRef<INode> node) => this.Nodes.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out NodeRef<INode> node) => (node = this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out node) ? node : default).Get() != null;

		public NodeRef<INode> GetNode(K key) => this.Nodes.TryGetValue(key, out NodeRef<INode> node) ? node : null;

		public NodeRef<INode> GetNodeById(long id) => this.NodeKeys.TryGetValue(id, out K key) && this.Nodes.TryGetValue(key, out NodeRef<INode> node) ? node : null;

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

		public IEnumerator<NodeRef<INode>> GetEnumerator() => Nodes.Values.GetEnumerator();
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
