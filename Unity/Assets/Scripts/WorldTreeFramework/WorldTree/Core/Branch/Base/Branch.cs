/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/13 03:52:45

* 描述： 世界树分支基类
* 继承自UnitDictionary实现，无序字典
* 有序是UnitSortedDictionary: 以键值排序，但在添加项时有56B的GC，不考虑

*/

using System.Collections;
using System.Collections.Generic;
namespace WorldTree
{
	/// <summary>
	/// 世界树分支基类
	/// </summary>
	public abstract class Branch<K> : UnitDictionary<K, INode>, IBranch<K>, ISerializable
	{
		[TreeDataIgnore]
		public int BranchCount => 1;

		/// <summary>
		/// 键值集合
		/// </summary>
		/// <remarks>节点Id，键值</remarks>
		[TreeDataIgnore]
		protected UnitDictionary<long, K> keyDict;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out keyDict);
		}

		public bool Contains(K key) => this.ContainsKey(key);

		public bool ContainsId(long id) => keyDict.ContainsKey(id);

		public virtual bool TryAddNode<N>(K key, N node) where N : class, INode => this.TryAdd(key, node) && keyDict.TryAdd(node.Id, key);

		public bool TryGetNodeKey(long nodeId, out K key) => keyDict.TryGetValue(nodeId, out key);

		public bool TryGetNode(K key, out INode node) => this.TryGetValue(key, out node);

		public bool TryGetNodeById(long id, out INode node) => (node = this.keyDict.TryGetValue(id, out K key) && this.TryGetValue(key, out node) ? node : null) != null;

		public INode GetNode(K key) => this.TryGetValue(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.keyDict.TryGetValue(id, out K key) && this.TryGetValue(key, out INode node) ? node : null;

		public void RemoveNode(long nodeId)
		{
			if (keyDict.TryGetValue(nodeId, out K key))
			{
				keyDict.Remove(nodeId);
				this.Remove(key);
			}
		}

		public void ClearAll()
		{
			this.Clear();
			keyDict.Clear();
		}

		IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => this.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.Values.GetEnumerator();

		public IEnumerable<KeyValuePair<K, INode>> GetEnumerable() => this;


		public override void OnDispose()
		{
			this.ClearAll();
			this.Dispose();
			this.keyDict.Dispose();
			this.keyDict = null;
		}
		public virtual void OnSerialize() { }

		public virtual void OnDeserialize()
		{
			keyDict.Clear();
			foreach (var item in this)
			{
				item.Value.BranchType = this.Type;
				keyDict.TryAdd(item.Value.Id, item.Key);
			}
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

		//报错，分支自己是不能添加自己的
		bool IBranchBase.TryAddBranch(long typeCode, IBranch branch) => throw new BranchOperationException();
		void IBranchBase.RemoveBranch(long typeCode) => throw new BranchOperationException();

		#endregion
	}

}