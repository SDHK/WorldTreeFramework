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
		public StaticListenerRuleActuator staticListenerRuleActuator;
		/// <summary>
		/// 动态监听器法则执行器
		/// </summary>
		public DynamicListenerRuleActuator dynamicListenerRuleActuator;

		public int TraversalCount => staticListenerRuleActuator?.TraversalCount ?? 0 + staticListenerRuleActuator?.TraversalCount ?? 0;

		public override string ToString()
		{
			return $"HybridListenerRuleActuator:{this.GetHashCode()} : {staticListenerRuleActuator == null} ??  {dynamicListenerRuleActuator == null}";
		}


		public int RefreshTraversalCount()
		{
			return staticListenerRuleActuator?.RefreshTraversalCount() ?? 0 + dynamicListenerRuleActuator?.RefreshTraversalCount() ?? 0;
		}

		public bool TryDequeue(out (INode, RuleList) value)
		{
			if (staticListenerRuleActuator != null)
			{
				if (staticListenerRuleActuator.TryDequeue(out value))
				{
					return true;
				}
			}
			if (dynamicListenerRuleActuator != null)
			{
				if (dynamicListenerRuleActuator.TryDequeue(out value))
				{
					return true;
				}
			}

			value = default;
			return false;
		}

		public bool TryPeek(out (INode, RuleList) value)
		{
			if (staticListenerRuleActuator != null)
			{
				if (staticListenerRuleActuator.TryPeek(out value))
				{
					return true;
				}
			}
			if (dynamicListenerRuleActuator != null)
			{
				if (dynamicListenerRuleActuator.TryPeek(out value))
				{
					return true;
				}
			}
			value = default;
			return false;
		}

		public (INode, RuleList) Dequeue()
		{
			return TryDequeue(out var value) ? value : default;
		}

		public (INode, RuleList) Peek()
		{
			return TryPeek(out var value) ? value : default;
		}

	}

	public static class HybridListenerRuleActuatorRule
	{
		class RemoveRule : RemoveRule<HybridListenerRuleActuator>
		{
			protected override void Execute(HybridListenerRuleActuator self)
			{
				self.staticListenerRuleActuator = null;
				self.dynamicListenerRuleActuator = null;
			}
		}
	}
}
