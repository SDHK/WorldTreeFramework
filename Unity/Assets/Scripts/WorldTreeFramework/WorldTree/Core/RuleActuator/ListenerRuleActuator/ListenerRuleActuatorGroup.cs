/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 混合型监听器法则执行器组

*/

namespace WorldTree
{
	/// <summary>
	/// 混合型监听器法则执行器组
	/// </summary>
	public class ListenerRuleActuatorGroup : Node, IListenerIgnorer, ComponentOf<ReferencedPool>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 监听器执行器字典集合
		/// </summary>
		public UnitDictionary<long, ListenerRuleActuator> ActuatorDict = new();
	}

	public static class HybridListenerRuleActuatorGroupRule
	{
		/// <summary>
		/// 添加监听执行器,并自动填装监听器
		/// </summary>
		public static IRuleActuator AddRuleActuator(this ListenerRuleActuatorGroup self, RuleGroup ruleGroup)
		{
			if (!self.ActuatorDict.TryGetValue(ruleGroup.RuleType, out ListenerRuleActuator ruleActuator))
			{
				self.AddComponent(out ruleActuator, ruleGroup);
				ruleActuator.RuleActuatorAddListener();
				self.ActuatorDict.Add(ruleGroup.RuleType, ruleActuator);
			}
			return ruleActuator;
		}


		#region 静态监听器

		/// <summary>
		/// 尝试添加监听器监听目标池
		/// </summary>
		/// <remarks>只在目标池存在时有效</remarks>
		public static void TryAddListener(this ReferencedPoolManager self, INodeListener listener)
		{
			//判断是否为监听器
			if (self.Core.RuleManager.ListenerRuleTargetGroupDict.TryGetValue(listener.Type, out var ruleGroupDictionary))
			{
				foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroup.Value.ContainsKey(listener.Type))
					{
						foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
						{
							//是否有这个目标缓存池
							if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
							{
								//是否有监听器集合组件
								if (nodePool.TryGetComponent(out ListenerRuleActuatorGroup ListenerRuleActuatorGroup))
								{
									//是否有这个监听类型的执行器
									if (ListenerRuleActuatorGroup.ActuatorDict.TryGetValue(ruleGroup.Key, out ListenerRuleActuator listenerRuleActuator))
									{
										//监听器添加到执行器
										listenerRuleActuator.TryAdd(listener);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 移除这个监听器
		/// </summary>
		/// <remarks>只在目标池存在时有效</remarks>
		public static void RemoveListener(this ReferencedPoolManager self, INodeListener listener)
		{
			//判断是否为监听器
			if (self.Core.RuleManager.ListenerRuleTargetGroupDict.TryGetValue(listener.Type, out var ruleGroupDictionary))
			{
				foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
				{
					//判断监听法则集合 是否有这个 监听器节点类型
					if (ruleGroup.Value.ContainsKey(listener.Type))
					{
						foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
						{
							//是否有这个目标池
							if (self.TryGetPool(ruleList.Key, out ReferencedPool nodePool))
							{
								//是否有监听器集合组件
								if (nodePool.TryGetComponent(out ListenerRuleActuatorGroup ListenerRuleActuatorGroup))
								{
									//是否有这个监听类型的执行器
									if (ListenerRuleActuatorGroup.ActuatorDict.TryGetValue(ruleGroup.Key, out ListenerRuleActuator listenerRuleActuator))
									{
										//监听器添加到执行器
										listenerRuleActuator.Remove(listener);
									}
								}
							}
						}
					}
				}
			}
		}


		#endregion

	}



}
