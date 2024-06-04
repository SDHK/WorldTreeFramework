/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/11 04:04:48

* 描述： 子节点分支
*
* 主要分支之一
*
* 设定根据实例自身id为键的分支。
*

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 子分支
	/// </summary>
	public class ChildBranch : UnitPoolItem, IBranchIdKey
	{
		public INode Self { get; set; }

		public int Count => Nodes.Count;

		protected UnitDictionary<long, INode> Nodes;

		public override void OnGet()
		{
			Core.PoolGetUnit(out Nodes);
		}

		public bool Contains(long key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => Nodes.ContainsKey(id);

		public bool TryAddNode<N>(long key, N node) where N : class, INode => Nodes.TryAdd(key, node);

		public bool TryGetNodeKey(long nodeId, out long key)
		{ key = nodeId; return true; }

		public bool TryGetNode(long key, out INode node) => this.Nodes.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => this.Nodes.TryGetValue(id, out node);

		public INode GetNode(long key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.Nodes.TryGetValue(id, out INode node) ? node : null;

		public void RemoveNode(long nodeId) => Nodes.Remove(nodeId);

		public void Clear()
		{
			Nodes.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.Self = null;
			this.Nodes = null;
		}
	}
}