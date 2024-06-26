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
	public abstract class Rattan<K> : UnitPoolItem, IRattan<K>
	{
		public int Count => Nodes.Count;
		protected UnitDictionary<K, NodeRef<INode>> Nodes;
		protected UnitDictionary<long, K> NodeKeys;

		public override void OnGet()
		{
			Core.PoolGetUnit(out Nodes);
			Core.PoolGetUnit(out NodeKeys);
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
				if (nodeRef.Value is null)
				{
					Nodes.Remove(key);
					NodeKeys.Remove(nodeRef.NodeId);
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
			foreach (var item in Nodes.Values)
			{
				INode node = item.Value;
				if (node != null) yield return node;
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
