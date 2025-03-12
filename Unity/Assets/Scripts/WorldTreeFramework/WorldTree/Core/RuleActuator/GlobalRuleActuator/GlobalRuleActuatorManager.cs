/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 全局法则执行器集合

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 核心扩展类型
	/// </summary>
	public static partial class WorldTreeCoreExtension
	{
		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static GlobalRuleActuator<R> GetGlobalRuleActuator<R>(this WorldTreeCore self, out GlobalRuleActuator<R> globalRuleActuator)
		where R : IGlobalRule
		{
			return self.AddComponent(out GlobalRuleActuatorManager _).AddTypeNode(self.TypeToCode<R>(), out globalRuleActuator);
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static IRuleActuator<R> GetGlobalRuleActuator<R>(this WorldTreeCore self, out IRuleActuator<R> ruleActuator)
		where R : IGlobalRule
		{
			self.AddComponent(out GlobalRuleActuatorManager _).AddTypeNode(self.TypeToCode<R>(), out GlobalRuleActuator<R> globalRuleActuator);
			ruleActuator = (IRuleActuator<R>)globalRuleActuator;
			return ruleActuator;
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleGroupActuatorBase GetGlobalRuleActuator(this WorldTreeCore self, long genericTypeCpde)
		{
			self.AddComponent(out GlobalRuleActuatorManager manager);
			INode node = NodeBranchHelper.GetBranch<TypeNodeBranch>(manager)?.GetNode(genericTypeCpde);
			if (node != null) return node as RuleGroupActuatorBase;

			Type type = typeof(GlobalRuleActuator<>).MakeGenericType(self.CodeToType(genericTypeCpde));
			NodeBranchHelper.AddNode<TypeNodeBranch, long>(manager, genericTypeCpde, self.TypeToCode(type), out node);
			RuleGroupActuatorBase actuator = node as RuleGroupActuatorBase;
			return actuator;
		}


		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static IRuleActuator<R> GetGlobalRuleActuator<R>(this WorldTreeCore self, long genericTypeCpde, out IRuleActuator<R> ruleActuator)
			where R : IGlobalRule
		{
			var actuator = self.GetGlobalRuleActuator(genericTypeCpde);
			ruleActuator = (IRuleActuator<R>)actuator;
			return ruleActuator;
		}
	}

	/// <summary>
	/// 全局法则执行器管理器
	/// </summary>
	public class GlobalRuleActuatorManager : Node, ComponentOf<WorldTreeCore>
		, AsTypeNodeBranch
		, AsAwake
	{ }
}
