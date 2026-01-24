/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定序器数据

*/
namespace WorldTree
{
	/// <summary>
	/// 级联定序器数据
	/// </summary>
	public class CascadeTickerData : Node
		, ChildOf<CascadeTicker>
		, AsRule<Awake<long, INode, RuleList>>
	{
		/// <summary>
		/// 时序
		/// </summary>
		public long Tick;

		/// <summary>
		/// 循环时序 
		/// </summary>
		public long LoopTick;

		/// <summary>
		/// 执行节点 
		/// </summary>
		public NodeRef<INode> Node;

		/// <summary>
		/// 事件规则类型 
		/// </summary>
		public RuleList RuleList;

		/// <summary>
		/// 是否为空 
		/// </summary>
		public bool IsNull => Node.IsNull;
	}

	public static class CascadeTickerDataRule
	{
		class AwakeRule : AwakeRule<CascadeTickerData, long, INode, RuleList>
		{
			protected override void Execute(CascadeTickerData self, long tick, INode node, RuleList ruleList)
			{
				self.Tick = tick;
				self.Node = new(node);
				self.RuleList = ruleList;
			}
		}
	}

}
