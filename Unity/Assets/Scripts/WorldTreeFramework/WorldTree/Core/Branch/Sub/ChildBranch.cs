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
	[TreeDataSerializable(true)]
	public partial class ChildBranch : UnitDictionary<long, INode>, IBranchIdKey, ISerializable
	{
		[TreeDataIgnore]
		public int BranchCount => 1;

		public bool Contains(long key) => this.ContainsKey(key);

		public bool ContainsId(long id) => this.ContainsKey(id);

		public bool TryAddNode<N>(long key, N node) where N : class, INode => this.TryAdd(node.Id, node);

		public bool TryGetNodeKey(long nodeId, out long key)
		{ key = nodeId; return true; }

		public bool TryGetNode(long key, out INode node) => this.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => this.TryGetValue(id, out node);

		public INode GetNode(long key) => this.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.TryGetValue(id, out INode node) ? node : null;

		public void RemoveNode(long nodeId) => this.Remove(nodeId);
		public void ClearAll() => this.Clear();

		IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => this.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.Values.GetEnumerator();
		public IEnumerable<KeyValuePair<long, INode>> GetEnumerable() => this;

		public void OnSerialize()
		{
		}

		public void OnDeserialize()
		{
			using UnitList<INode> nodeList = Core.PoolGetUnit<UnitList<INode>>();
			foreach (var item in this) nodeList.Add(item.Value);
			this.Clear();
			foreach (var item in nodeList) this.TryAdd(item.Id, item);
		}

		#region 伪装分支集合

		IEnumerator<IBranch> IEnumerable<IBranch>.GetEnumerator() => new SingleBranchEnumerator(this);

		public bool ContainsBranch(long typeCode) => typeCode == Type;

		public bool TryGetBranch(long typeCode, out IBranch branch)
		{
			if (typeCode == Type)
			{
				branch = this;
				return true;
			}
			branch = null;
			return false;
		}

		public IBranch GetBranch(long typeCode) => typeCode == Type ? this : null;
		bool IBranchBase.TryAddBranch(long typeCode, IBranch branch) => throw new BranchOperationException();
		void IBranchBase.RemoveBranch(long typeCode) => throw new BranchOperationException();

		#endregion
	}
}