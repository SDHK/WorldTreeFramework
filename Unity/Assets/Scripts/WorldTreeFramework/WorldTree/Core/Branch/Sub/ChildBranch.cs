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
	[TreeDataSerializable]
	public partial class ChildBranch : Unit, IBranchIdKey
	{
		public int Count => nodeDict.Count;

		/// <summary>
		/// 节点集合
		/// </summary>
		protected UnitDictionary<long, INode> nodeDict;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out nodeDict);
		}

		public bool Contains(long key) => nodeDict.ContainsKey(key);

		public bool ContainsId(long id) => nodeDict.ContainsKey(id);

		public bool TryAddNode<N>(long key, N node) where N : class, INode => nodeDict.TryAdd(key, node);

		public bool TryGetNodeKey(long nodeId, out long key)
		{ key = nodeId; return true; }

		public bool TryGetNode(long key, out INode node) => this.nodeDict.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => this.nodeDict.TryGetValue(id, out node);

		public INode GetNode(long key) => this.nodeDict.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.nodeDict.TryGetValue(id, out INode node) ? node : null;

		public void RemoveNode(long nodeId) => nodeDict.Remove(nodeId);

		public void Clear()
		{
			nodeDict.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => nodeDict.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => nodeDict.Values.GetEnumerator();

		public override void OnDispose()
		{
			this.nodeDict.Dispose();
			this.nodeDict = null;
		}
	}
}