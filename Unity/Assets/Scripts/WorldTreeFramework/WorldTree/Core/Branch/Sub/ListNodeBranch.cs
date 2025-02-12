/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/13 03:52:45

* 描述： 世界树列表分支
*

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 列表分支，Key为-1添加到末尾
	/// </summary>
	[TreeDataSerializable]
	public partial class ListNodeBranch : Unit, IBranch<int>, ISerializable
	{
		public int Count => nodeList == null ? 0 : nodeList.Count;

		/// <summary>
		/// 节点集合
		/// </summary>
		/// <remarks>键值，节点</remarks>
		protected UnitList<INode> nodeList;

		/// <summary>
		/// 键值集合
		/// </summary>
		/// <remarks>节点Id，键值</remarks>
		[TreeDataIgnore]
		protected UnitDictionary<long, int> keyDict;

		public override void OnCreate()
		{
			Core.PoolGetUnit(out nodeList);
			Core.PoolGetUnit(out keyDict);
		}

		public bool Contains(int key) => nodeList.Count > key;

		public bool ContainsId(long id) => keyDict.ContainsKey(id);

		public virtual bool TryAddNode<N>(int key, N node) where N : class, INode
		{
			if (key == -1)
			{
				nodeList.Add(node);
				return keyDict.TryAdd(node.Id, key);
			}
			else if (key < nodeList.Count)
			{
				nodeList.Insert(key, node);
				for (int i = key; i < nodeList.Count; i++)
				{
					keyDict[nodeList[i].Id] = i;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetNodeKey(long nodeId, out int key) => keyDict.TryGetValue(nodeId, out key);

		public bool TryGetNode(int key, out INode node)
		{
			if (nodeList.Count > key)
			{
				node = nodeList[key];
				return true;
			}
			node = null;
			return false;
		}

		public bool TryGetNodeById(long id, out INode node)
		{
			if (this.keyDict.TryGetValue(id, out int key) && TryGetNode(key, out node))
			{
				return true;
			}
			else
			{
				node = null;
				return false;
			}
		}

		public INode GetNode(int key) => TryGetNode(key, out INode node) ? node : null;

		public INode GetNodeById(long id) => this.keyDict.TryGetValue(id, out int key) && TryGetNode(key, out INode node) ? node : null;

		public void RemoveNode(long nodeId)
		{
			if (keyDict.TryGetValue(nodeId, out int key))
			{
				keyDict.Remove(nodeId);
				nodeList.RemoveAt(key);
				for (int i = key; i < nodeList.Count; i++)
				{
					keyDict[nodeList[i].Id] = i;
				}
			}
		}

		/// <summary>
		/// 交换两个节点
		/// </summary>
		public bool Swap(int index1, int index2)
		{
			if (index1 == index2 || index1 < 0 || index1 >= Count || index2 < 0 || index2 >= Count) return false;
			(nodeList[index2], nodeList[index1]) = (nodeList[index1], nodeList[index2]);
			keyDict[nodeList[index1].Id] = index1;
			keyDict[nodeList[index2].Id] = index2;
			return true;
		}

		public void Clear()
		{
			nodeList.Clear();
			keyDict.Clear();
		}

		public IEnumerator<INode> GetEnumerator() => nodeList.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => nodeList.GetEnumerator();

		public IEnumerable<KeyValuePair<int, INode>> GetEnumerable()
		{
			for (int i = 0; i < nodeList.Count; i++)
			{
				yield return new KeyValuePair<int, INode>(i, nodeList[i]);
			}
		}

		public override void OnDispose()
		{
			this.nodeList.Dispose();
			this.keyDict.Dispose();
			this.nodeList = null;
			this.keyDict = null;
		}
		public virtual void OnSerialize() { }

		public virtual void OnDeserialize()
		{
			keyDict.Clear();
			for (int i = 0; i < nodeList.Count; i++)
			{
				keyDict.TryAdd(nodeList[i].Id, i);
			}
		}
	}


	public static class ListBranchSupplementRule
	{
		/// <summary>
		/// 交换两个下标的节点：不触发任何生命周期
		/// </summary>
		public static bool Swap(this AsListNodeBranch self, int index1, int index2)
			=> self.ListNodeBranch()?.Swap(index1, index2) ?? false;
	}
}