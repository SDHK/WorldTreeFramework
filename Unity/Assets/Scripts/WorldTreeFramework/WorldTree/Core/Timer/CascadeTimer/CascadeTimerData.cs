/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器

*/
namespace WorldTree
{
	/// <summary>
	/// 级联定时器数据
	/// </summary>
	public class CascadeTimerData : Node
		, ChildOf<CascadeTimer>
		, AsRule<Awake<long, INode, long>>
	{
		/// <summary>
		/// 定时时间戳 
		/// </summary>
		public long TimeStamp;

		/// <summary>
		/// 执行节点 
		/// </summary>
		public NodeRef<INode> Node;

		/// <summary>
		/// 事件规则类型 
		/// </summary>
		public long RuleType;

		/// <summary>
		/// 是否为空 
		/// </summary>
		public bool IsNull => Node.IsNull;
	}

	public static class CacadeTimeDataRule
	{
		class AwakeRule : AwakeRule<CascadeTimerData, long, INode, long>
		{
			protected override void Execute(CascadeTimerData self, long timeStamp, INode node, long ruleType)
			{
				self.TimeStamp = timeStamp;
				self.Node = new(node);
				self.RuleType = ruleType;
			}
		}
	}

}
