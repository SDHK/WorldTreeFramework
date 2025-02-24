/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/04 08:19:28

* 描述： 节点引用

*/

namespace WorldTree
{


	public partial struct NodeRef<N>
	{
		class TreeDataSerialize : TreeDataSerializeRule<NodeRef<N>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref SerializedTypeMode typeMode)
			{
				if (self.TryWriteDataHead(value, typeMode, ~1, out NodeRef<N> obj, false)) return;
				self.WriteDynamic(1);
				self.WriteValue(obj.Id);
			}
		}
		class TreeDataDeserialize : TreeDataDeserializeRule<NodeRef<N>>
		{
			protected override void Execute(TreeDataByteSequence self, ref object value, ref int nameCode)
			{
				if (self.TryReadArrayHead(typeof(NodeRef<N>), ref value, 1, out int _)) return;
				self.ReadDynamic(out int _);
				value = new NodeRef<N>() { Core = self.Core, Id = self.ReadValue<long>() };


			}
		}
	}


	/// <summary>
	/// 节点引用
	/// </summary>
	public partial struct NodeRef<N>
		where N : class, INode
	{
		/// <summary>
		/// 世界树核心
		/// </summary>
		[TreeDataIgnore]
		public WorldTreeCore Core;

		/// <summary>
		/// 节点数据ID，普通节点为0
		/// </summary>
		public long Id;

		/// <summary>
		/// 节点实例ID
		/// </summary>
		[TreeDataIgnore]
		public long InstanceId;

		/// <summary>
		/// 节点
		/// </summary>
		[TreeDataIgnore]
		private N node;

		/// <summary>
		/// 获取节点，如果节点被回收，那么会返回null
		/// </summary>
		public N Value
		{
			get
			{
				if (node is null || InstanceId != node.InstanceId)
				{
					if (Core == null || Id == 0 || !Core.ReferencedPoolManager.AllNodeDataDict.TryGetValue(Id, out INodeData nodeData))
					{
						node = null;
					}
					else
					{
						node = nodeData as N;
						InstanceId = node.InstanceId;
					}
				}
				return node;
			}
		}

		public NodeRef(N node, bool isDataRef = true)
		{
			if (node is null)
			{
				Id = 0;
				InstanceId = 0;
				this.node = null;
				Core = null;
				return;
			}
			Core = isDataRef ? node.Core : null;
			Id = node is INodeData ? node.Id : 0;
			InstanceId = node.InstanceId;
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

		public static bool operator ==(NodeRef<N> a, N b) => a.InstanceId == b?.InstanceId;
		public static bool operator !=(NodeRef<N> a, N b) => a.InstanceId != b?.InstanceId;

		public override bool Equals(object obj) => obj is N node ? InstanceId == node.InstanceId : false;
		public override int GetHashCode() => node?.GetHashCode() ?? 0;

		public static implicit operator bool(NodeRef<N> nodeRef) => nodeRef.node is not null;
	}

	//public struct Fixed
}
