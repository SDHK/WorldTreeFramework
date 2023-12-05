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
		public readonly long nodeId;
		private N node;

		public N Value => (node is null || nodeId == node.Id) ? node : node = null;
		
		/// <summary>
		/// 是否为空
		/// </summary>
		public bool IsNull => Value is null;

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

		public bool TryGet(out N node)=> (node = Value) is not null;

		public static implicit operator NodeRef<N>(N node) => new(node);

		public static implicit operator N(NodeRef<N> nodeRef) => nodeRef.Value;
	}
}
