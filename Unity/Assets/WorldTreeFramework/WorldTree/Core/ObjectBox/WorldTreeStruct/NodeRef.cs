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

		public NodeRef(N node)
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
		/// <summary>
		/// 尝试获取节点
		/// </summary>
		public bool TryGet(out N node) => (node = Value) is not null;

		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear()=> node = null;

		public static implicit operator NodeRef<N>(N node) => new(node);

		public static implicit operator N(NodeRef<N> nodeRef) => nodeRef.Value;
	}
}
