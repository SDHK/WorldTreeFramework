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
		/// <summary>
		/// 节点ID
		/// </summary>
		public readonly long NodeId;
		/// <summary>
		/// 节点
		/// </summary>
		private N node;

		/// <summary>
		/// 获取节点，如果节点被回收，那么会返回null
		/// </summary>
		/// <remarks> 每次获取都会进行判断，建议获取后将值用变量存起来 </remarks>
		public N Value => (node is null || NodeId == node.Id) ? node : node = null;

		public NodeRef(N node)
		{
			if (node is null)
			{
				NodeId = 0;
				this.node = null;
				return;
			}
			NodeId = node.Id;
			this.node = node;
		}
		/// <summary>
		/// 尝试获取节点
		/// </summary>
		public bool TryGet(out N node) => (node = Value) is not null;

		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear() => node = null;

		public static implicit operator NodeRef<N>(N node) => new(node);

		public static implicit operator N(NodeRef<N> nodeRef) => nodeRef.Value;
	}
}
