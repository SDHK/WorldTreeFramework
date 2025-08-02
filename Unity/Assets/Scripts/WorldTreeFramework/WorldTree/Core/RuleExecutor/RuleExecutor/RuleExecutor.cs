/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 泛型法则执行器
*

*/

namespace WorldTree
{

	/// <summary>
	/// 单法则调用器
	/// </summary>
	public class RuleInvoker<R> : Node, IRuleExecutor<R>, IRuleExecutorEnumerable
		, ChildOf<INode>
		where R : IRule
	{
		/// <summary>
		/// 目标节点
		/// </summary>
		public NodeRef<INode> TargetNode;

		/// <summary>
		/// 法则列表
		/// </summary>
		public RuleList RuleList;

		public int TraversalCount => TargetNode.Value != null ? 1 : 0;

		/// <summary>
		/// a
		/// </summary>
		public void Clear()
		{
			TargetNode = default;
			RuleList = default;
		}

		public int RefreshTraversalCount()
		{
			return TraversalCount;
		}

		/// <summary>
		/// a
		/// </summary>
		public void Remove(long id)
		{
			if (TargetNode == null) return;
			if (TargetNode.Id != id) return;
			TargetNode = default;
			RuleList = default;
		}

		/// <summary>
		/// a
		/// </summary>
		public void Remove(INode node)
		{
			if (TargetNode != node) return;
			TargetNode = default;
			RuleList = default;
		}


		public bool TryDequeue(out INode node, out RuleList ruleList)
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

		public bool TryPeek(out INode node, out RuleList ruleList)
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

	/// <summary>
	/// 泛型法则执行器
	/// </summary>
	public class RuleExecutor<R> : RuleExecutorBase, IRuleExecutor<R>
		, ChildOf<INode>
		, AsRule<Awake>
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