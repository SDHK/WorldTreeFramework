/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局单法则执行器

*/

using System;

namespace WorldTree
{

	/// <summary>
	/// 全局法则执行器
	/// </summary>
	public class GlobalRuleActuator<R> : RuleGroupActuatorBase, INodeListener, IRuleActuator<R>
		, ComponentOf<GlobalRuleActuatorManager>
		, AsAwake
		where R : IRule
	{
		public override string ToString()
		{
			return GetType().ToString();
		}
	}

	public static class GlobalRuleActuatorRule
	{
		class ListenerAddRule<R> : ListenerAddRule.Rule<GlobalRuleActuator<R>, R>
			where R : IRule
		{
			protected override void Execute(GlobalRuleActuator<R> self, INode node)
			{
				self.TryAdd(node);
			}
		}

		class Add<R> : AddRule<GlobalRuleActuator<R>>
			where R : IRule
		{
			protected override void Execute(GlobalRuleActuator<R> self)
			{
				self.ruleGroupDict = self.Core.RuleManager.GetOrNewRuleGroup<R>();
				self.LoadGlobalNode();
			}
		}

		/// <summary>
		/// 填装全局节点
		/// </summary>
		public static void LoadGlobalNode<R>(this GlobalRuleActuator<R> self)
			where R : IRule
		{
			foreach (var item in self.ruleGroupDict)
			{
				bool isListenerIgnorer = false;
				foreach (Type typeItem in self.CodeToType(item.Key).GetInterfaces())
				{
					if (typeItem == typeof(IListenerIgnorer))
					{
						isListenerIgnorer = true;
						break;
					}
				}

				if (!isListenerIgnorer)
				{
					if (self.Core.ReferencedPoolManager.TryGetPool(item.Key, out ReferencedPool pool))
					{
						foreach (var node in pool.NodeDict) self.TryAdd(node.Value);
					}
				}
			}
		}
	}
}
