/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 18:26

* 描述： 混合型监听器法则执行器

*/

namespace WorldTree
{

	/// <summary>
	/// 混合型监听器法则执行器
	/// </summary>
	public class HybridListenerRuleActuator : Node, IListenerIgnorer, IRuleActuatorEnumerable, IRuleActuator<IRule>
		, ComponentOf<HybridListenerRuleActuatorGroup>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 静态监听器法则执行器
		/// </summary>
		public StaticListenerRuleActuator StaticListenerRuleActuator;
		/// <summary>
		/// 动态监听器法则执行器
		/// </summary>
		public DynamicListenerRuleActuator DynamicListenerRuleActuator;

		public int TraversalCount => StaticListenerRuleActuator?.TraversalCount ?? 0 + StaticListenerRuleActuator?.TraversalCount ?? 0;

		public override string ToString()
		{
			return $"HybridListenerRuleActuator:{this.GetHashCode()} : {StaticListenerRuleActuator == null} ??  {DynamicListenerRuleActuator == null}";
		}


		public int RefreshTraversalCount()
		{
			return StaticListenerRuleActuator?.RefreshTraversalCount() ?? 0 + DynamicListenerRuleActuator?.RefreshTraversalCount() ?? 0;
		}

		public bool TryDequeue(out INode node, out RuleList ruleList)
		{
			if (StaticListenerRuleActuator != null)
			{
				if (StaticListenerRuleActuator.TryDequeue(out  node, out  ruleList))
				{
					return true;
				}
			}
			if (DynamicListenerRuleActuator != null)
			{
				if (DynamicListenerRuleActuator.TryDequeue(out node, out ruleList))
				{
					return true;
				}
			}
			node = null;
			ruleList = null;
			return false;
		}

		public bool TryPeek(out INode node, out RuleList ruleList)
		{
			if (StaticListenerRuleActuator != null)
			{
				if (StaticListenerRuleActuator.TryPeek(out node, out ruleList))
				{
					return true;
				}
			}
			if (DynamicListenerRuleActuator != null)
			{
				if (DynamicListenerRuleActuator.TryPeek(out node, out ruleList))
				{
					return true;
				}
			}
			node = null;
			ruleList = null;
			return false;
		}
	}

	public static class HybridListenerRuleActuatorRule
	{
		class RemoveRule : RemoveRule<HybridListenerRuleActuator>
		{
			protected override void Execute(HybridListenerRuleActuator self)
			{
				self.StaticListenerRuleActuator = null;
				self.DynamicListenerRuleActuator = null;
			}
		}
	}
}
