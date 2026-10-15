/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 节点法则引用 
* 
* 用于执行器中的节点和法则的绑定存储。
* 这个结构是不可被序列化的，也不能找回数据节点。
* 

 */

using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// 节点法则引用
	/// </summary>
	// 针对缓存优化的紧凑结构体
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct NodeRuleRef
	{
		/// <summary>
		/// 节点实例ID
		/// </summary>
		public long InstanceId;

		/// <summary>
		/// 单位。节点也是单位。
		/// </summary>
		private IUnit node;

		/// <summary>
		/// 法则
		/// </summary>
		public RuleList Rule;

		public NodeRuleRef(IUnit node, RuleList rule)
		{
			this.node = node;
			this.Rule = rule;
			this.InstanceId = node.InstanceId;
		}

		/// <summary>
		/// 获取单位。实例号变了说明已经还池，返回 null。
		/// </summary>
		public IUnit Node
		{
			get
			{
				if (node != null && InstanceId != node.InstanceId) { node = null; Rule = null; }
				return node;
			}
		}

		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear()
		{
			node = null;
			Rule = null;
		}
	}
}
