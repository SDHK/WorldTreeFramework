/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 泛型法则执行器
*

*/

namespace WorldTree
{
	/// <summary>
	/// 全局法则数据
	/// </summary>
	public class GlobalRuleExecutorData : Unit, IRuleExecutor<IRule>
	{
		/// <summary>
		/// 法则类型码
		/// </summary>
		public long RuleTypeCode;
	}

	/// <summary>
	/// 泛型法则执行器
	/// </summary>
	public class RuleExecutor<R> : RuleExecutorBase, ChildOf<INode>, IRuleExecutor<R>
		, AsAwake
		where R : IRule
	{
		public override string ToString()
		{
			return $"RuleExecutor<{typeof(R)}>";
		}
	}

	public static class RuleExecutorRule
	{
		/// <summary>
		/// 添加节点法则：指定法则
		/// </summary>
		public static void Add<R, N, NR>(this RuleExecutor<R> self, N node, NR defaultRule = default)
			where R : IRule
			where N : class, INode, AsRule<NR>
			where NR : R
		{
			if (self.Core.RuleManager.TryGetRuleList<NR>(node.Type, out RuleList ruleList))
			{
				self.TryAdd(node, ruleList);
			}
			else
			{
				self.Log($"空法则{typeof(NR)}");
			}
		}

		/// <summary>
		/// 添加节点法则：默认法则
		/// </summary>
		public static void Add<R, N>(this RuleExecutor<R> self, N node)
			where R : IRule
			where N : class, INode, AsRule<R>
		{
			if (self.Core.RuleManager.TryGetRuleList<R>(node.Type, out RuleList ruleList))
			{
				self.TryAdd(node, ruleList);
			}
			else
			{
				self.Log($"空法则{typeof(R)}");
			}
		}
	}
}