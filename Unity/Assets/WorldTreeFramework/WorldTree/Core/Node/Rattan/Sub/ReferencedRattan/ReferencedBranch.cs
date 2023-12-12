using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public class ReferencedChildRattan : ReferencedParentRattan { }

	public class ReferencedParentRattan : UnitPoolItem, IRattan<long>
	{
		public INode Self { get; set; }

		public int Count => Nodes.Count;

		protected UnitDictionary<long, INode> Nodes;

		public override void OnGet()
		{
			Core.PoolGet(out Nodes);
		}


		public bool Contains(long key) => Nodes.ContainsKey(key);

		public bool ContainsId(long id) => Nodes.ContainsKey(id);


		public bool TryAddNode<N>(long key, N node) where N : class, INode => Nodes.TryAdd(key, node);

		public bool TryGetNodeKey(long nodeId, out long key) { key = nodeId; return true; }

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


	public static class NodeReferencedRattanRule
	{
		/// <summary>
		/// 建立引用关系
		/// </summary>
		public static void Referenced(this INode self, INode node)
		{
			self.AddRattan<ReferencedChildRattan>().TryAddNode(node.Id, node);
			node.AddRattan<ReferencedParentRattan>().TryAddNode(self.Id, self);
		}

		/// <summary>
		/// 解除引用关系
		/// </summary>
		public static void DeReferenced(this INode self, INode node)
		{
			if (self.TryGetRattan(out ReferencedChildRattan _))
			{
				self.RemoveRattanNode<ReferencedChildRattan>(node);
				self.TrySendRule(TypeInfo<IDeReferencedChildRule>.Default, node);
			}

			if (node.TryGetRattan(out ReferencedParentRattan _))
			{
				self.RemoveRattanNode<ReferencedParentRattan>(self);
				node.TrySendRule(TypeInfo<IDeReferencedParentRule>.Default, self);
			}
		}

		/// <summary>
		/// 解除所有引用关系
		/// </summary>
		public static void DeReferencedAll(this INode self)
		{
			//移除父级
			if (self.TryGetRattan(out ReferencedParentRattan parentRattan))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in parentRattan) nodes.Enqueue(item);
					while (nodes.Count != 0) self.DeReferenced(nodes.Dequeue());
				}
			}

			//移除子级
			if (self.TryGetRattan(out ReferencedChildRattan childRattan))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in childRattan) nodes.Enqueue(item);
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
			if (self.TryGetRattan(out ReferencedParentRattan parentRattan))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in parentRattan) nodes.Enqueue(item);
					while (nodes.Count != 0)
					{
						var node = nodes.Dequeue();
						node.RemoveRattanNode<ReferencedChildRattan>(self);
						node.TrySendRule(TypeInfo<IReferencedChildRemoveRule>.Default, self);
					}
				}
			}

			//移除子级
			if (self.TryGetRattan(out ReferencedChildRattan childRattan))
			{
				using (self.PoolGet(out UnitQueue<INode> nodes))
				{
					foreach (var item in childRattan) nodes.Enqueue(item);
					while (nodes.Count != 0)
					{
						var node = nodes.Dequeue();
						node.RemoveRattanNode<ReferencedParentRattan>(self);
						node.TrySendRule(TypeInfo<IReferencedParentRemoveRule>.Default, self);
					}
				}
			}
		}

	}
}
