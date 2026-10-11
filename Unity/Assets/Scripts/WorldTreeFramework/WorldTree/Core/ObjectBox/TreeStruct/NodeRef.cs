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
				if (self.TryReadArrayHead(typeof(NodeRef<N>), ref value, 1, out _, out _)) return;
				self.ReadDynamic(out int _);
				value = new NodeRef<N>() { World = self.World, Id = self.ReadValue<long>() };
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
		/// 世界
		/// </summary>
		[TreeDataIgnore]
		public World World;

		/// <summary>
		/// 节点ID：数据和实例混合标识
		/// </summary>
		public long Id;

		/// <summary>
		/// 节点实例ID：建议使用Id来判断唯一性，InstanceId仅作节点生成顺序标识
		/// </summary>
		/// <remarks>
		/// 实例id只在一个世界树实例内唯一。
		/// </remarks>
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
		[TreeDataIgnore]
		public N Value
		{
			get
			{
				if (node is null || InstanceId != node.InstanceId)
				{
					if (World == null || Id == 0 || !World.Line.ReferencedPoolManager.TryGetNodeData<N>(Id, out INodeData nodeData))
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
				World = null;
				return;
			}

			Id = node.Id;
			if (node is INodeData)
			{
				World = isDataRef ? node.World : null;
			}
			else
			{
				World = null;
			}
			InstanceId = node.InstanceId;
			this.node = node;
		}

		/// <summary>
		/// 是否为空引用 
		/// </summary>
		[TreeDataIgnore]
		public bool IsNull => Value is null;

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

		public static bool operator ==(NodeRef<N> a, N b) => Same(a.Value, b);
		public static bool operator !=(NodeRef<N> a, N b) => !(a == b);
		public override bool Equals(object obj)
		{
			if (obj is NodeRef<N> other) return Same(Value, other.Value);
			if (obj is N node) return Same(Value, node);
			return false;
		}
		/// <summary>
		/// 同一对象，或同一条非 0 的 Id，视为相等
		/// </summary>
		private static bool Same(N value, N other)
		{
			if (value == other) return true;
			if (value is null || other is null) return false;
			return value.Id != 0 && value.Id == other.Id;
		}
		public override int GetHashCode() => node?.GetHashCode() ?? 0;

		public static implicit operator bool(NodeRef<N> nodeRef) => nodeRef.node is not null;
	}
}
