/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 20:34

* 描述： 节点监听执行器帮助类

*/

namespace WorldTree
{
	/// <summary>
	/// 节点监听执行器帮助类
	/// </summary>
	public static class NodeListenerExecutorHelper
	{
		/// <summary>
		/// 获取监听这个单位的监听器法则执行器
		/// </summary>
		public static IRuleExecutor<R> GetListenerExecutor<R>(IUnit unit)
		   where R : IListenerRule
		{
			if (unit.World.Line.ReferencedPoolManager == null) return null;
			if (!unit.World.Line.ReferencedPoolManager.TryGetPool(unit.Type, out ReferencedPool nodePool)) return null;
			if (!unit.World.Line.Core.RuleManager.TryGetTargetRuleGroup(TypeInfo<R>.Code, unit.Type, out RuleGroup ruleGroup)) return null;
			if (!nodePool.TryGetComponent(out ListenerRuleExecutorGroup executorGroup)) nodePool.AddComponent(out executorGroup);
			return executorGroup.AddRuleExecutor(ruleGroup) as IRuleExecutor<R>;
		}
	}
}
