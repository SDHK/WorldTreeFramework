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
			return self.GlobalRuleExecutorManager.AddTypeNode(self.TypeToCode<R>(), out globalRuleExecutor);
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static IRuleExecutor<R> GetGlobalRuleExecutor<R>(this WorldTreeCore self, out IRuleExecutor<R> ruleExecutor)
		where R : IGlobalRule
		{
			self.GlobalRuleExecutorManager.AddTypeNode(self.TypeToCode<R>(), out GlobalRuleExecutor<R> globalRuleExecutor);
			ruleExecutor = globalRuleExecutor as IRuleExecutor<R>;
			return ruleExecutor;
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleGroupExecutorBase GetGlobalRuleExecutor(this WorldTreeCore self, long genericTypeCpde)
		{
			INode node = NodeBranchHelper.GetBranch<TypeNodeBranch>(self.GlobalRuleExecutorManager)?.GetNode(genericTypeCpde);
			if (node != null) return node as RuleGroupExecutorBase;

			if (!self.TryCodeToType(genericTypeCpde, out Type genericType)) return null;
			Type type = typeof(GlobalRuleExecutor<>).MakeGenericType(genericType);
			NodeBranchHelper.AddNode<TypeNodeBranch, long>(self.GlobalRuleExecutorManager, genericTypeCpde, self.TypeToCode(type), out node);
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
			ruleExecutor = executor as IRuleExecutor<R>;
			return ruleExecutor;
		}
	}

	/// <summary>
	/// 全局法则执行器管理器
	/// </summary>
	public class GlobalRuleExecutorManager : Node, CoreManagerOf<WorldTreeCore>
		, AsTypeNodeBranch
		, AsAwake
	{ }
}
