/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则多播执行器
*

*/

namespace WorldTree
{
	/// <summary>
	/// 法则多播接口
	/// </summary>
	public interface IRuleMulticast : IRuleExecutor
	{
		/// <summary>
		/// 尝试添加节点法则
		/// </summary>
		public bool TryAdd(INode node, RuleList rule);
	}

	/// <summary>
	/// 法则多播接口
	/// </summary>
	public interface IRuleMulticast<in R> : IRuleExecutor<R>, IRuleMulticast where R : IRule { }

	/// <summary>
	/// 法则多播执行器
	/// </summary>
	public class RuleMulticast<R> : RuleListExecutor, IRuleExecutor<R>, IRuleMulticast
		, ChildOf<INode>
		, AsRule<Awake>
		where R : IRule
	{
		public override string ToString()
		{
			return $"RuleMulticaster<{typeof(R)}>";
		}
	}

	public static class RuleMulticastRule
	{
		/// <summary>
		/// 添加节点法则：指定法则
		/// </summary>
		public static void Add<R, N, NR>(this RuleMulticast<R> self, N node, NR defaultRule = default)
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
		public static void Add<R, N>(this RuleMulticast<R> self, N node)
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