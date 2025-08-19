/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

 */

using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// 节点法则执行对:只用于法则执行器
	/// </summary>
	// 针对缓存优化的紧凑结构体
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RuleExecutorPair
	{
		/// <summary>
		/// 节点实例ID
		/// </summary>
		public long InstanceId;

		/// <summary>
		/// 节点
		/// </summary>
		private INode node;

		/// <summary>
		/// 法则
		/// </summary>
		public RuleList Rule;

		public RuleExecutorPair(INode node, RuleList rule)
		{
			this.node = node;
			this.Rule = rule;
			this.InstanceId = node.InstanceId;
		}

		/// <summary>
		/// 获取节点，如果节点被回收，那么会返回null
		/// </summary>
		public INode Node
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
