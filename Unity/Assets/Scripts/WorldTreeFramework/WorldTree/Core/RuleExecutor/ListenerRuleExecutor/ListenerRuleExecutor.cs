/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/8 14:38

* 描述： 监听器法则执行器

*/


namespace WorldTree
{
	/// <summary>
	/// 监听器法则执行器
	/// </summary>
	public class ListenerRuleExecutor : RuleGroupExecutorBase, IListenerIgnorer, IRuleExecutor<IRule>
		, ComponentOf<ListenerRuleExecutorGroup>
		, AsRule<Awake<RuleGroup>>
	{
		public override string ToString()
		{
			return $"ListenerRuleExecutor : {(ruleGroupDict == null ? null : Core.CodeToType(ruleGroupDict.RuleType))}";
		}
	}

	public static class ListenerRuleExecutorRule
	{
		class AwakeRule : AwakeRule<ListenerRuleExecutor, RuleGroup>
		{
			protected override void Execute(ListenerRuleExecutor self, RuleGroup arg1)
			{
				self.ruleGroupDict = arg1;
			}
		}

		/// <summary>
		/// 执行器填装监听器
		/// </summary>
		public static void RuleExecutorAddListener(this ListenerRuleExecutor self)
		{
			//遍历法则集合获取监听器类型
			foreach (var listenerType in self.ruleGroupDict)
			{
				//从池里拿到已存在的监听器
				if (!self.Core.ReferencedPoolManager.TryGetPool(listenerType.Key, out ReferencedPool listenerPool)) continue;
				//全部注入到执行器
				foreach (var listener in listenerPool.NodeDict) self.TryAdd(listener.Value);
			}
		}
	}


}
