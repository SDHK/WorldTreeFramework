using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public class ReferencedChildBranch : UnitPoolItem, IBranch<long>
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



		public void RemoveNode(long key) => Nodes.Remove(key);

		public void RemoveNodeById(long id) => Nodes.Remove(id);

		public void RemoveAllNode()=> Nodes.Clear();

		public void RemoveNodeInDictionary(INode node) => Nodes.Remove(node.Id);


		public IEnumerator<INode> GetEnumerator() => Nodes.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Nodes.Values.GetEnumerator();

	}
	public class ReferencedParentBranch : Branch<long> { }


	public static class NodeReferencedBranchRule
	{


	}
}
