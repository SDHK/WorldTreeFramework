/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*

 */

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器抽象基类
	/// </summary>
	public abstract class RuleGroupExecutor : RuleListExecutor, IRuleExecutorEnumerable
		, AsChildBranch
	{

		/// <summary>
		/// 法则类型
		/// </summary>
		[TreeDataIgnore]
		public long RuleType => ruleGroupDict.RuleType;

		/// <summary>
		/// 单法则集合
		/// </summary>
		[TreeDataIgnore]
		[Protected] public RuleGroup ruleGroupDict;

		/// <summary>
		/// 尝试添加单位
		/// </summary>
		public virtual bool TryAdd(IUnit unit)
		{
			if (unit == null) return false;
			if (ruleGroupDict == null || !ruleGroupDict.TryGetValue(unit.Type, out RuleList rule)) return false;
			return base.TryAdd(unit, rule);
		}
	}

	public static class RuleGroupExecutorRule
	{
		class RemoveRule : RemoveRule<RuleGroupExecutor>
		{
			protected override void Execute(RuleGroupExecutor self)
			{
				self.GetBaseRule(default(RuleListExecutor), default(Remove)).Send(self);
				self.ruleGroupDict = null;
			}
		}
	}
}
