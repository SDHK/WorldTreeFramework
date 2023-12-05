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

		public bool Contains(K key) => TryGetNode(key, out _);

		public bool ContainsId(long id) => TryGetNodeKey(id, out _);

		public bool TryGetNodeKey(long nodeId, out K key) => NodeKeys.TryGetValue(nodeId, out key) && TryGetNode(key, out _);

		public bool TryAddNode<N>(K key, N node) where N : class, INode
		{
			if (!TryGetNode(key, out _) && !NodeKeys.ContainsKey(node.Id))
			{
				Nodes.Add(key, node);
				NodeKeys.Add(node.Id, key);
				return true;
			}
			return false;
		}

		public bool TryGetNode(K key, out INode node)
		{
			if (Nodes.TryGetValue(key, out var nodeRef))
			{
				if (nodeRef.IsNull)
				{
					Nodes.Remove(key);
					NodeKeys.Remove(nodeRef.nodeId);
					node = null;
					return false;
				}
				else
				{
					node = nodeRef.Value;
					return true;
				}
			}
			node = null;
			return false;
		}

		public bool TryGetNodeById(long nodeId, out INode node)
		{
			if (NodeKeys.TryGetValue(nodeId, out K key))
			{
				return TryGetNode(key, out node);
			}
			node = null;
			return false;
		}

		public INode GetNode(K key) => TryGetNode(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => TryGetNodeById(id, out INode node) ? node : null;

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

		public IEnumerator<INode> GetEnumerator()
		{
			using (this.Core.PoolGet(out UnitQueue<NodeRef<INode>> Queue))
			{
				foreach (var item in Nodes.Values) Queue.Enqueue(item);
				while (Queue.Count != 0)
				{
					NodeRef<INode> nodeRef = Queue.Dequeue();
					if (nodeRef.IsNull)
					{
						RemoveNode(nodeRef.nodeId);
						continue;
					}
					yield return nodeRef.Value;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.NodeKeys.Dispose();
			this.Nodes = null;
			this.NodeKeys = null;
		}
	}
}
