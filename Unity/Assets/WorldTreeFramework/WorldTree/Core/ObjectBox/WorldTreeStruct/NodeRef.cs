/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/04 08:19:28

* 描述： 节点引用

*/

namespace WorldTree
{
	/// <summary>
	/// 节点引用
	/// </summary>
	public struct NodeRef<N>
		where N : class, INode
	{
		private readonly long nodeId;
		private N node;

		private NodeRef(N node)
		{
			if (node is null)
			{
				nodeId = 0;
				this.node = null;
				return;
			}
			nodeId = node.Id;
			this.node = node;
		}
		public N Get() => node;
		public bool TryGet(out N node)
		{
			node = this.node;
			return node is not null;
		}
		public static implicit operator NodeRef<N>(N node) => new(node);

		public static implicit operator N(NodeRef<N> nodeRef)
			=> (nodeRef.node is null || nodeRef.nodeId == nodeRef.node.Id) ? nodeRef.node : nodeRef.node = null;
	}
}
