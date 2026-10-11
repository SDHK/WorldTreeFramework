/****************************************

* 作者： 闪电黑客
* 日期： 2023/11/13 03:52:45

* 描述： 世界树字段节点分支
* 
* 用于存储字段节点的分支，这种节点通常是稳定结构不需要动态修改，列表用于自动回收。
*

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 字段节点分支
	/// </summary>
	[TreeDataSerializable(true)]
	public partial class FieldBranch : UnitList<INode>, IBranchIndexKey, ISerializable
	{
		[TreeDataIgnore]
		public new int Count => base.Count;
		[TreeDataIgnore]
		public int BranchCount => 1;

		public bool Contains(int key) => this.Count > key;
		public bool ContainsId(long id) => this.Find(node => node.Id == id) != null;


		public virtual bool TryAddNode<N>(int key, N node) where N : class, INode
		{
			if (key == -1 || key == this.Count)
			{
				this.Add(node);
				return true;
			}
			else if (key < this.Count)
			{
				this.Insert(key, node);
				return true;
			}
			else
			{
				return false;
			}
		}



		public bool TryGetNodeKey(long nodeId, out int key)
		{

			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Id == nodeId)
				{
					key = i;
					return true;
				}
			}
			key = -1;
			return false;
		}

		public bool TryGetNode(int key, out INode node)
		{
			if (this.Count > key)
			{
				node = this[key];
				return true;
			}
			node = null;
			return false;
		}


		public bool TryGetNodeById(long id, out INode node)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Id == id)
				{
					node = this[i];
					return true;
				}
			}
			node = null;
			return false;
		}
		public INode GetNode(int key)
		{
			return this[key];
		}

		public INode GetNodeById(long id)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Id == id)
				{
					return this[i];
				}
			}
			return null;
		}
		public void RemoveNode(long nodeId)
		{
			if (TryGetNodeKey(nodeId, out int key))
			{
				this.RemoveAt(key);
			}
		}

		public void ClearAll()
		{
			this.Clear();
		}

		public IEnumerable<KeyValuePair<int, INode>> GetEnumerable()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return new KeyValuePair<int, INode>(i, this[i]);
			}
		}

		public virtual void OnSerialize() { }
		public virtual void OnDeserialize() { }




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
		public bool TryAddBranch(long typeCode, IBranch branch) => throw new BranchOperationException();
		public void RemoveBranch(long typeCode) => throw new BranchOperationException();

		#endregion




	}
}