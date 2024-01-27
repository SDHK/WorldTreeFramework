
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 18:26

* 描述： 混合型监听器法则执行器

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 混合型监听器法则执行器
	/// </summary>
	public class HybridListenerRuleActuator : Node, IListenerIgnorer, IRuleActuatorEnumerable, IRuleActuator<IRule>
		, ComponentOf<HybridListenerRuleActuatorGroup>
		, AsRule<IAwakeRule>
	{
		/// <summary>
		/// 静态监听器法则执行器
		/// </summary>
		public StaticListenerRuleActuator staticListenerRuleActuator;
		/// <summary>
		/// 动态监听器法则执行器
		/// </summary>
		public DynamicListenerRuleActuator dynamicListenerRuleActuator;

		public override string ToString()
		{
			return $"HybridListenerRuleActuator:{this.GetHashCode()} : {staticListenerRuleActuator == null} ??  {dynamicListenerRuleActuator == null}";
		}

		public IEnumerator<(INode, RuleList)> GetEnumerator()
		{
			if (staticListenerRuleActuator != null)
			{
				foreach (var item in staticListenerRuleActuator)
				{
					yield return item;
				}
			}
			if (dynamicListenerRuleActuator != null)
			{
				foreach (var item in dynamicListenerRuleActuator)
				{
					yield return item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static class HybridListenerRuleActuatorRule
	{
		class RemoveRule : RemoveRule<HybridListenerRuleActuator>
		{
			protected override void OnEvent(HybridListenerRuleActuator self)
			{
				self.staticListenerRuleActuator = null;
				self.dynamicListenerRuleActuator = null;
			}
		}
	}
}
