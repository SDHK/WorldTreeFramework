﻿/****************************************

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
	public abstract class Branch<K> : Unit, IBranch<K>
	{
		public int Count => nodeDict == null ? 0 : nodeDict.Count;

		/// <summary>
		/// 节点集合
		/// </summary>
		/// <remarks>键值，节点</remarks>
		protected UnitDictionary<K, INode> nodeDict;

		/// <summary>
		/// 键值集合
		/// </summary>
		/// <remarks>节点Id，键值</remarks>
		protected UnitDictionary<long, K> keyDict;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out nodeDict);
			Core.PoolGetUnit(out keyDict);
		}

		public bool Contains(K key) => nodeDict.ContainsKey(key);

		public bool ContainsId(long id) => keyDict.ContainsKey(id);

		public bool TryAddNode<N>(K key, N node) where N : class, INode => nodeDict.TryAdd(key, node) && keyDict.TryAdd(node.Id, key);

		public bool TryGetNodeKey(long nodeId, out K key) => keyDict.TryGetValue(nodeId, out key);

		public bool TryGetNode(K key, out INode node) => this.nodeDict.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => (node = this.keyDict.TryGetValue(id, out K key) && this.nodeDict.TryGetValue(key, out node) ? node : null) != null;

		public INode GetNode(K key) => this.nodeDict.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.keyDict.TryGetValue(id, out K key) && this.nodeDict.TryGetValue(key, out INode node) ? node : null;

		public void RemoveNode(long nodeId)
		{
			if (keyDict.TryGetValue(nodeId, out K key))
			{
				keyDict.Remove(nodeId);
				nodeDict.Remove(key);
			}
		}

		public void Clear()
		{
			nodeDict.Clear();
			keyDict.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => nodeDict.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => nodeDict.Values.GetEnumerator();

		public override void OnDispose()
		{
			this.nodeDict.Dispose();
			this.keyDict.Dispose();
			this.nodeDict = null;
			this.keyDict = null;
		}
	}
}