/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局单法则执行器

*/

using System.Linq;

namespace WorldTree
{

	/// <summary>
	/// 全局法则执行器
	/// </summary>
	public class GlobalRuleActuator<R> : RuleGroupActuatorBase, INodeListener, IRuleActuator<R>
		, ComponentOf<GlobalRuleActuatorManager>
		, AsRule<IAwakeRule>
		where R : IRule
	{
		public override string ToString()
		{
			return $"GlobalRuleActuator : {ruleGroup?.RuleType.HashCore64ToType()}";
		}
	}


	public static class GlobalRuleActuatorRule
	{
		class ListenerAddRule<R> : ListenerAddRule.Rule<GlobalRuleActuator<R>, R>
			where R : IRule
		{
			protected override void OnEvent(GlobalRuleActuator<R> self, INode node)
			{
				self.TryAdd(node);
			}
		}

		class ListenerRemoveRule<R> : ListenerRemoveRule.Rule<GlobalRuleActuator<R>, R>
			where R : IRule
		{
			protected override void OnEvent(GlobalRuleActuator<R> self, INode node)
			{
				self.Remove(node);
			}
		}


		class AddRuleGeneric<R> : AddRule<GlobalRuleActuator<R>>
			where R : IRule
		{
			protected override void OnEvent(GlobalRuleActuator<R> self)
			{
				self.ruleGroup = self.Core.RuleManager.GetOrNewRuleGroup<R>();
				self.LoadGlobalNode();
			}
		}

		/// <summary>
		/// 填装全局节点
		/// </summary>
		public static void LoadGlobalNode<R>(this GlobalRuleActuator<R> self)
			where R : IRule
		{
			foreach (var item in self.ruleGroup)
			{
				if (!item.Key.HashCore64ToType().GetInterfaces().Contains(typeof(ICoreNode)))
				{
					if (self.Core.ReferencedPoolManager.TryGetPool(item.Key, out ReferencedPool pool))
					{
						foreach (var node in pool)
						{
							self.TryAdd(node.Value);
						}
					}
				}
			}
		}
	}
}
