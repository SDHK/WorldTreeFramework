/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 混合型监听器法则执行器组

*/

namespace WorldTree
{
	/// <summary>
	/// 监听器法则执行器组
	/// </summary>
	public class ListenerRuleExecutorGroup : Node, IListenerIgnorer, ComponentOf<ReferencedPool>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 监听器执行器字典集合
		/// </summary>
		public UnitDictionary<long, ListenerRuleExecutor> ExecutorDict = new();
	}

	public static class ListenerRuleExecutorGroupRule
	{
		/// <summary>
		/// 添加监听执行器,并自动填装监听器
		/// </summary>
		public static IRuleExecutor AddRuleExecutor(this ListenerRuleExecutorGroup self, RuleGroup ruleGroup)
		{
			if (!self.ExecutorDict.TryGetValue(ruleGroup.RuleType, out ListenerRuleExecutor ruleExecutor))
			{
				self.AddComponent(out ruleExecutor, ruleGroup);
				ruleExecutor.RuleExecutorAddListener();
				self.ExecutorDict.Add(ruleGroup.RuleType, ruleExecutor);
			}
			return ruleExecutor;
		}


		#region 静态监听器

		/// <summary>
		/// 尝试添加监听器监听目标池
		/// </summary>
		/// <remarks>只在目标池存在时有效</remarks>
		public static void TryAddListener(this ReferencedPoolManager self, INodeListener listener)
		{
			//判断是否为监听器
			if (!self.Core.RuleManager.ListenerRuleTargetGroupDict.TryGetValue(listener.Type, out var ruleGroupDictionary)) return;
			foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
			{
				//判断监听法则集合 是否有这个 监听器节点类型
				if (!ruleGroup.Value.ContainsKey(listener.Type)) continue;
				foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
				{
					//是否有这个目标缓存池
					if (!self.TryGetPool(ruleList.Key, out ReferencedPool nodePool)) continue;
					//是否有监听器集合组件
					if (!nodePool.TryGetComponent(out ListenerRuleExecutorGroup ListenerRuleExecutorGroup)) continue;
					//是否有这个监听类型的执行器
					if (!ListenerRuleExecutorGroup.ExecutorDict.TryGetValue(ruleGroup.Key, out ListenerRuleExecutor listenerRuleExecutor)) continue;
					//监听器添加到执行器
					listenerRuleExecutor.TryAdd(listener);
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
			if (!self.Core.RuleManager.ListenerRuleTargetGroupDict.TryGetValue(listener.Type, out var ruleGroupDictionary)) return;
			foreach (var ruleGroup in ruleGroupDictionary)//遍历法则集合集合获取系统类型
			{
				//判断监听法则集合 是否有这个 监听器节点类型
				if (!ruleGroup.Value.ContainsKey(listener.Type)) continue;
				foreach (var ruleList in ruleGroup.Value)//遍历法则集合获取目标类型
				{
					//是否有这个目标池
					if (!self.TryGetPool(ruleList.Key, out ReferencedPool nodePool)) continue;
					//是否有监听器集合组件
					if (!nodePool.TryGetComponent(out ListenerRuleExecutorGroup ListenerRuleExecutorGroup)) continue;
					//是否有这个监听类型的执行器
					if (!ListenerRuleExecutorGroup.ExecutorDict.TryGetValue(ruleGroup.Key, out ListenerRuleExecutor listenerRuleExecutor)) continue;
					//监听器移除
					listenerRuleExecutor.Remove(listener);
				}
			}
		}
		#endregion

	}



}
