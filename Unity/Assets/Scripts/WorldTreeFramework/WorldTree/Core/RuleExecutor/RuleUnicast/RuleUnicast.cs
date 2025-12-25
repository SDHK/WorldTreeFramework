/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则单播执行器
*

*/

namespace WorldTree
{
	/// <summary>
	/// 法则单播接口
	/// </summary>
	public interface IRuleUnicast : IRuleExecutor
	{
		/// <summary>
		/// 目标节点
		/// </summary>
		public NodeRef<INode> TargetNode { get; set; }

		/// <summary>
		/// 法则列表
		/// </summary>
		public RuleList RuleList { get; set; }

		/// <summary>
		/// 尝试添加节点法则
		/// </summary>
		public bool TryAdd(INode node, RuleList rule);
	}

	/// <summary>
	/// 法则单播接口
	/// </summary>
	public interface IRuleUnicast<in R> : IRuleExecutor<R>, IRuleUnicast where R : IRule { }

	/// <summary>
	/// 法则单播执行器
	/// </summary>
	public class RuleUnicast<R> : Node, IRuleUnicast<R>, IRuleExecutorEnumerable
		, ChildOf<INode>
		where R : IRule
	{
		/// <summary>
		/// 目标节点
		/// </summary>
		public NodeRef<INode> TargetNode { get; set; }

		/// <summary>
		/// 法则列表
		/// </summary>
		public RuleList RuleList { get; set; }

		public int TraversalCount => TargetNode.Value != null ? 1 : 0;

		public int RemainCount => TargetNode.Value != null ? 1 : 0;

		public void Clear()
		{
			TargetNode = default;
			RuleList = default;
		}

		public int RefreshTraversalCount()
		{
			return TraversalCount;
		}

		public bool TryAdd(INode node, RuleList rule)
		{
			TargetNode = new NodeRef<INode>(node);
			RuleList = rule;
			return true;
		}
		public bool TryGetNext(out INode node, out RuleList ruleList)
		{
			if (TargetNode.Value != null)
			{
				node = TargetNode.Value;
				ruleList = RuleList;
				return true;
			}
			node = null;
			ruleList = default;
			return false;
		}
	}

	public static class RuleUnicastRule
	{
		/// <summary>
		/// 添加节点法则：指定法则
		/// </summary>
		public static void Add<R, N, NR>(this IRuleUnicast<R> self, N node, NR defaultRule = default)
			where R : IRule
			where N : class, INode, AsRule<NR>
			where NR : R
		{
			if (self.Core.RuleManager.TryGetRuleList<NR>(node.Type, out RuleList ruleList))
			{
				self.TargetNode = node;
				self.RuleList = ruleList;
				self.Log($"添加法则{typeof(NR)}到{node}");
			}
			else
			{
				self.Log($"空法则{typeof(NR)}");
			}
		}

		/// <summary>
		/// 添加节点法则：默认法则
		/// </summary>
		public static void Add<R, N>(this IRuleUnicast<R> self, N node)
			where R : IRule
			where N : class, INode, AsRule<R>
		{
			if (self.Core.RuleManager.TryGetRuleList<R>(node.Type, out RuleList ruleList))
			{
				self.TargetNode = node;
				self.RuleList = ruleList;
			}
			else
			{
				self.Log($"空法则{typeof(R)}");
			}
		}
	}
}