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
		public static GlobalRuleExecutor<R> GetGlobalRuleExecutor<R>(this WorldTreeCore self, out GlobalRuleExecutor<R> globalRuleExecutor)
		where R : IGlobalRule
		{
			return self.AddComponent(out GlobalRuleExecutorManager _).AddTypeNode(self.TypeToCode<R>(), out globalRuleExecutor);
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static IRuleExecutor<R> GetGlobalRuleExecutor<R>(this WorldTreeCore self, out IRuleExecutor<R> ruleExecutor)
		where R : IGlobalRule
		{
			self.AddComponent(out GlobalRuleExecutorManager _).AddTypeNode(self.TypeToCode<R>(), out GlobalRuleExecutor<R> globalRuleExecutor);
			ruleExecutor = (IRuleExecutor<R>)globalRuleExecutor;
			return ruleExecutor;
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleGroupExecutorBase GetGlobalRuleExecutor(this WorldTreeCore self, long genericTypeCpde)
		{
			self.AddComponent(out GlobalRuleExecutorManager manager);
			INode node = NodeBranchHelper.GetBranch<TypeNodeBranch>(manager)?.GetNode(genericTypeCpde);
			if (node != null) return node as RuleGroupExecutorBase;

			Type type = typeof(GlobalRuleExecutor<>).MakeGenericType(self.CodeToType(genericTypeCpde));
			NodeBranchHelper.AddNode<TypeNodeBranch, long>(manager, genericTypeCpde, self.TypeToCode(type), out node);
			RuleGroupExecutorBase executor = node as RuleGroupExecutorBase;
			return executor;
		}


		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static IRuleExecutor<R> GetGlobalRuleExecutor<R>(this WorldTreeCore self, long genericTypeCpde, out IRuleExecutor<R> ruleExecutor)
			where R : IGlobalRule
		{
			var executor = self.GetGlobalRuleExecutor(genericTypeCpde);
			ruleExecutor = (IRuleExecutor<R>)executor;
			return ruleExecutor;
		}
	}

	/// <summary>
	/// 全局法则执行器管理器
	/// </summary>
	public class GlobalRuleExecutorManager : Node, ComponentOf<WorldTreeCore>
		, AsTypeNodeBranch
		, AsAwake
	{ }
}
