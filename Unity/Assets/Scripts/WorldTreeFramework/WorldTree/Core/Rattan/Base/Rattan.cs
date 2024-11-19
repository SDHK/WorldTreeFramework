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
	public abstract class Rattan<K> : Unit, IRattan<K>
	{
		public int Count => nodeDict.Count;
		/// <summary>
		/// 节点字典
		/// </summary>
		/// <remarks>键值，节点 </remarks>
		protected UnitDictionary<K, NodeRef<INode>> nodeDict;

		/// <summary>
		/// 节点键值字典
		/// </summary>
		/// <remarks>节点id，节点键值</remarks>
		protected UnitDictionary<long, K> nodeKeyDict;


		public override void OnCreate()
		{
			Core.PoolGetUnit(out nodeDict);
			Core.PoolGetUnit(out nodeKeyDict);
		}

		public bool Contains(K key) => TryGetNode(key, out _);

		public bool ContainsId(long id) => TryGetNodeKey(id, out _);

		public bool TryGetNodeKey(long nodeId, out K key) => nodeKeyDict.TryGetValue(nodeId, out key) && TryGetNode(key, out _);

		public bool TryAddNode<N>(K key, N node) where N : class, INode
		{
			if (!TryGetNode(key, out _) && !nodeKeyDict.ContainsKey(node.Id))
			{
				nodeDict.Add(key, node);
				nodeKeyDict.Add(node.Id, key);
				return true;
			}
			return false;
		}

		public bool TryGetNode(K key, out INode node)
		{
			if (nodeDict.TryGetValue(key, out var nodeRef))
			{
				if (nodeRef.Value is null)
				{
					nodeDict.Remove(key);
					nodeKeyDict.Remove(nodeRef.InstanceId);
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
			if (nodeKeyDict.TryGetValue(nodeId, out K key))
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
			if (nodeKeyDict.TryGetValue(nodeId, out K key))
			{
				nodeKeyDict.Remove(nodeId);
				nodeDict.Remove(key);
			}
		}

		public void Clear()
		{
			nodeDict.Clear();
			nodeKeyDict.Clear();
		}

		public IEnumerator<INode> GetEnumerator()
		{
			foreach (var item in nodeDict.Values)
			{
				INode node = item.Value;
				if (node != null) yield return node;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override void OnDispose()
		{
			this.nodeDict.Dispose();
			this.nodeKeyDict.Dispose();
			this.nodeDict = null;
			this.nodeKeyDict = null;
		}
	}
}
