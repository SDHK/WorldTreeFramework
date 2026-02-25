/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/19 15:59

* 描述： 级联定时器数据

*/
namespace WorldTree
{
	/// <summary>
	/// 级联定时器数据
	/// </summary>
	public class CascadeTickerData : Node
		, ChildOf<CascadeTicker>
		, AsRule<Awake<long, INode, RuleList>>
		, AsRule<TreeTaskTokenEvent>
	{
		/// <summary>
		/// 时序
		/// </summary>
		public long Tick;

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

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<CascadeTickerData>
		{
			protected override void Execute(CascadeTickerData self, TokenState state)
			{
				switch (state)
				{
					case TokenState.Running: break;
					case TokenState.Stop: break;
					case TokenState.Cancel:
						// 取消任务时，直接执行规则并销毁自身
						if (!self.Node.IsNull) self.RuleList.Send<ISendRule>(self.Node.Value);
						self.Dispose();
						break;
				}
			}
		}
	}

}
