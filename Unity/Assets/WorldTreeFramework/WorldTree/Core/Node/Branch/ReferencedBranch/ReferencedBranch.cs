﻿using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public class ReferencedChildBranch : ReferencedParentBranch { }

	public class ReferencedParentBranch : UnitPoolItem, IBranch<long>
	{
		public INode Self { get; set; }

		public int Count => Nodes.Count;

		protected UnitDictionary<long, INode> Nodes;

		public void SetNode(INode node)
		{
			if (Self == null)
			{
				Self = node;
				Self.PoolGet(out Nodes);
			}
		}


		public bool Contains(long key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => Nodes.ContainsKey(id);


		public bool TryAddNode<N>(long key, N node) where N : class, INode => Nodes.TryAdd(key, node);

		public bool TryGetNodeKey(INode node, out long key) { key = node.Id; return true; }

		public bool TryGetNode(long key, out INode node) => this.Nodes.TryGetValue(key, out node);
		public bool TryGetNodeById(long id, out INode node) => this.Nodes.TryGetValue(id, out node);

		public INode GetNode(long key) => this.Nodes.TryGetValue(key, out INode node) ? node : null;
		public INode GetNodeById(long id) => this.Nodes.TryGetValue(id, out INode node) ? node : null;



		public void RemoveNode(long key) => RemoveNodeAndBranchDispose(key);

		public void RemoveNodeById(long id) => RemoveNodeAndBranchDispose(id);

		public void RemoveAllNode()
		{
			Nodes.Clear();
			//移除分支自己
			this.Self.m_Branchs.Remove(this.Type);
			//如果分支字典为空，那么就释放分支字典
			if (this.Self.m_Branchs.Count == 0)
			{
				this.Self.m_Branchs.Dispose();
				this.Self.m_Branchs = null;
			}
			//释放分支自己
			this.Dispose();
		}

		public void RemoveNodeAndBranchDispose(long nodeId)
		{
			Nodes.Remove(nodeId);

			//如果分支字典为空，那么就释放分支字典
			if (Nodes.Count == 0)
			{
				//移除分支自己
				this.Self.m_Branchs.Remove(this.Type);
				//如果分支字典为空，那么就释放分支字典
				if (this.Self.m_Branchs.Count == 0)
				{
					this.Self.m_Branchs.Dispose();
					this.Self.m_Branchs = null;
				}
				//释放分支自己
				this.Dispose();
			}
		}


		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

		public override void OnRecycle()
		{
			this.Nodes.Dispose();
			this.Self = null;
			this.Nodes = null;
			base.OnRecycle();
		}
	}


	public static class NodeReferencedBranchRule
	{
		/// <summary>
		/// 建立引用关系
		/// </summary>
		public static void Referenced(this INode self, INode node)
		{
			self.AddBranch<ReferencedChildBranch>().TryAddNode(node.Id, node);
			node.AddBranch<ReferencedParentBranch>().TryAddNode(self.Id, self);
		}

		/// <summary>
		/// 解除引用关系
		/// </summary>
		public static void DeReferenced(this INode self, INode node)
		{
			if (self.TryGetBranch(out ReferencedChildBranch branch))
			{
				branch.RemoveNodeAndBranchDispose(node.Id);
				self.TrySendRule(TypeInfo<IDeReferencedChildRule>.Default, node);
			}

			if (node.TryGetBranch(out ReferencedParentBranch branch2))
			{
				branch2.RemoveNodeAndBranchDispose(self.Id);
				node.TrySendRule(TypeInfo<IDeReferencedParentRule>.Default, self);
			}
		}

		/// <summary>
		/// 解除所有引用关系
		/// </summary>
		public static void DeReferencedAll(this INode self)
		{
			//移除父级
			if (self.TryGetBranch(out ReferencedParentBranch parentBranch))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in parentBranch) nodes.Enqueue(item);
					while (nodes.Count != 0) self.DeReferenced(nodes.Dequeue());
				}
			}

			//移除子级
			if (self.TryGetBranch(out ReferencedChildBranch childBranch))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in childBranch) nodes.Enqueue(item);
					while (nodes.Count != 0) self.DeReferenced(nodes.Dequeue());
				}
			}
		}


		/// <summary>
		/// 解除所有引用关系, 通知自己的移除生命周期事件
		/// </summary>
		public static void SendAllReferencedNodeRemove(this INode self)
		{
			//移除父级
			if (self.TryGetBranch(out ReferencedParentBranch parentBranch))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in parentBranch) nodes.Enqueue(item);
					while (nodes.Count != 0)
					{
						var node = nodes.Dequeue();
						node.GetBranch<ReferencedChildBranch>()?.RemoveNodeAndBranchDispose(self.Id);
						node.TrySendRule(TypeInfo<IReferencedChildRemoveRule>.Default, self);
					}
				}
			}

			//移除子级
			if (self.TryGetBranch(out ReferencedChildBranch childBranch))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in childBranch) nodes.Enqueue(item);
					while (nodes.Count != 0)
					{
						var node = nodes.Dequeue();
						node.GetBranch<ReferencedParentBranch>()?.RemoveNodeAndBranchDispose(self.Id);
						node.TrySendRule(TypeInfo<IReferencedParentRemoveRule>.Default, self);
					}
				}
			}
		}

	}
}