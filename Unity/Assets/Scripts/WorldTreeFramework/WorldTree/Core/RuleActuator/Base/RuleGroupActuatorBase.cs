/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

 */

namespace WorldTree
{
	/// <summary>
	/// 法则集合执行器基类
	/// </summary>
	public abstract class RuleGroupActuatorBase : RuleActuatorBase, IRuleGroupActuator
	{
		/// <summary>
		/// 单法则集合
		/// </summary>
		[Protected]public RuleGroup ruleGroupDict;

		public override string ToString()
		{
			return $"RuleGroupActuator : {ruleGroupDict?.RuleType.CodeToType()}";
		}

		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node) => ruleGroupDict.TryGetValue(node.Type, out RuleList ruleList) && TryAdd(node, ruleList);
	}

	public static class RuleGroupActuatorBaseRule
	{
		class RemoveRule : RemoveRule<RuleGroupActuatorBase>
		{
			protected override void Execute(RuleGroupActuatorBase self)
			{
				self.ruleGroupDict = null;
				self.Clear();
			}
		}
	}
}
