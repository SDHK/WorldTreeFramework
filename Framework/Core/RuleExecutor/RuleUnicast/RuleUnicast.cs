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
		/// 目标单位
		/// </summary>
		public NodeRef<IUnit> TargetUnit { get; set; }

		/// <summary>
		/// 法则列表
		/// </summary>
		public RuleList RuleList { get; set; }

		/// <summary>
		/// 尝试添加单位法则
		/// </summary>
		public bool TryAdd(IUnit unit, RuleList rule);
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
		/// 目标单位
		/// </summary>
		public NodeRef<IUnit> TargetUnit { get; set; }

		/// <summary>
		/// 法则列表
		/// </summary>
		public RuleList RuleList { get; set; }

		public int TraversalCount => TargetUnit.Value != null ? 1 : 0;

		public int RemainCount => TargetUnit.Value != null ? 1 : 0;

		public void Clear()
		{
			TargetUnit = default;
			RuleList = default;
		}

		public int RefreshTraversalCount()
		{
			return TraversalCount;
		}

		public bool TryAdd(IUnit unit, RuleList rule)
		{
			TargetUnit = new NodeRef<IUnit>(unit);
			RuleList = rule;
			return true;
		}
		public bool TryGetNext(out IUnit unit, out RuleList ruleList)
		{
			if (TargetUnit.Value != null)
			{
				unit = TargetUnit.Value;
				ruleList = RuleList;
				return true;
			}
			unit = null;
			ruleList = default;
			return false;
		}
	}

	public static class RuleUnicastRule
	{
		/// <summary>
		/// 添加单位法则：指定法则
		/// </summary>
		public static void Add<R, N, NR>(this IRuleUnicast<R> self, N unit, NR defaultRule = default)
			where R : IRule
			where N : class, IUnit, AsRule<NR>
			where NR : R
		{
			if (self.World.Line.Core.RuleManager.TryGetRuleList<NR>(unit.Type, out RuleList ruleList))
			{
				self.TargetUnit = unit;
				self.RuleList = ruleList;
				self.Log($"添加法则{typeof(NR)}到{unit}");
			}
			else
			{
				self.Log($"空法则{typeof(NR)}");
			}
		}

		/// <summary>
		/// 添加单位法则：默认法则
		/// </summary>
		public static void Add<R, N>(this IRuleUnicast<R> self, N unit)
			where R : IRule
			where N : class, IUnit, AsRule<R>
		{
			if (self.World.Line.Core.RuleManager.TryGetRuleList<R>(unit.Type, out RuleList ruleList))
			{
				self.TargetUnit = unit;
				self.RuleList = ruleList;
			}
			else
			{
				self.Log($"空法则{typeof(R)}");
			}
		}
	}
}