﻿/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/8 14:42

* 描述： 动态监听器法则执行器
* 
* 暂时没有用上动态的功能，先留着吧

*/

namespace WorldTree
{
	/// <summary>
	/// 动态监听器法则执行器
	/// </summary>
	public class DynamicListenerRuleActuator : RuleGroupActuatorBase, IListenerIgnorer, IRuleActuator<IRule>
		, ComponentOf<HybridListenerRuleActuator>
		, AsAwake<RuleGroup>
	{
		public override string ToString()
		{
			return $"DynamicListenerRuleActuator : {ruleGroupDict?.RuleType.CodeToType()}";
		}
	}

	public static class DynamicListenerRuleActuatorRule
	{
		private class AwakeRule : AwakeRule<DynamicListenerRuleActuator, RuleGroup>
		{
			protected override void Execute(DynamicListenerRuleActuator self, RuleGroup arg1)
			{
				self.ruleGroupDict = arg1;
			}
		}

		/// <summary>
		/// 执行器填装监听器
		/// </summary>
		public static void RuleActuatorAddListener(this DynamicListenerRuleActuator self, long nodeType)
		{
			//遍历法则集合获取监听器类型
			foreach (var listenerType in self.ruleGroupDict)
			{
				//从池里拿到已存在的监听器
				if (self.Core.ReferencedPoolManager.TryGetPool(listenerType.Key, out ReferencedPool listenerPool))
				{
					//全部注入到执行器
					foreach (var listenerPair in listenerPool)
					{
						IDynamicNodeListener nodeListener = (listenerPair.Value as IDynamicNodeListener);

						//判断目标是否被该监听器监听
						if (nodeListener.ListenerTarget != 0)
						{
							if (nodeListener.ListenerState == ListenerState.Node)
							{
								//判断是否全局监听 或 是指定的目标类型
								if (nodeListener.ListenerTarget == TypeInfo<INode>.TypeCode || nodeListener.ListenerTarget == nodeType)
								{
									self.TryAdd(nodeListener);
								}
							}
							else if (nodeListener.ListenerState == ListenerState.Rule)
							{
								//判断的实体类型是否拥有目标系统
								if (self.Core.RuleManager.TryGetRuleList(nodeType, nodeListener.ListenerTarget, out _))
								{
									self.TryAdd(nodeListener);
								}
							}
						}
					}
				}
			}
		}
	}
}