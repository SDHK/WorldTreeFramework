/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/11 11:52:09

* 描述： 法则执行器基类
*
* 

*/

namespace WorldTree
{

	/// <summary>
	/// 执行项
	/// </summary>
	public class ActuatorItem : Unit
	{
		/// <summary>
		/// 节点id
		/// </summary>
		public long NodeId;

		/// <summary>
		/// 节点
		/// </summary>
		private INode node;

		/// <summary>
		/// 获取节点，如果节点被回收，那么会返回null
		/// </summary>
		public INode Node => (node is null || NodeId == node.Id) ? node : node = null;

		/// <summary>
		/// 法则列表引用
		/// </summary>
		public RuleList RuleList;


		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear()
		{
			NodeId = 0;
			node = null;
			RuleList = null;
		}
	}
}