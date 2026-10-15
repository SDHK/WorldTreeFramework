/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 单位法则引用
* 
* 用于执行器中的单位和法则的绑定存储。
* 这个结构是不可被序列化的，也不能找回数据节点。
* 

 */

using System.Runtime.InteropServices;

namespace WorldTree
{
	/// <summary>
	/// 单位法则引用
	/// </summary>
	// 针对缓存优化的紧凑结构体
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct UnitRuleRef
	{
		/// <summary>
		/// 单位实例ID
		/// </summary>
		public long InstanceId;

		/// <summary>
		/// 单位。节点也是单位。
		/// </summary>
		private IUnit unit;

		/// <summary>
		/// 法则
		/// </summary>
		public RuleList Rule;

		public UnitRuleRef(IUnit unit, RuleList rule)
		{
			this.unit = unit;
			this.Rule = rule;
			this.InstanceId = unit.InstanceId;
		}

		/// <summary>
		/// 获取单位。实例号变了说明已经还池，返回 null。
		/// </summary>
		public IUnit Unit
		{
			get
			{
				if (unit != null && InstanceId != unit.InstanceId) { unit = null; Rule = null; }
				return unit;
			}
		}

		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear()
		{
			unit = null;
			Rule = null;
		}
	}
}
