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
		public static RuleBroadcast<R> GetRuleBroadcast<R>(this WorldLine self, out RuleBroadcast<R> ruleExecutor)
		where R : IGlobalRule
		{
			self.GlobalRuleExecutorManager.AddGeneric(self.TypeToCode<R>(), out RuleBroadcaster<R> globalRuleExecutor);
			ruleExecutor = globalRuleExecutor as RuleBroadcast<R>;
			return ruleExecutor;
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleBroadcaster GetRuleBroadcast(this WorldLine self, long genericTypeCpde)
		{
			INode node = NodeBranchHelper.GetBranch<GenericBranch<long>>(self.GlobalRuleExecutorManager)?.GetNode(genericTypeCpde);
			if (node != null) return node as RuleBroadcaster;

			if (!self.TryCodeToType(genericTypeCpde, out Type genericType)) return null;
			Type type = typeof(RuleBroadcaster<>).MakeGenericType(genericType);
			NodeBranchHelper.AddNode(self.GlobalRuleExecutorManager, default(GenericBranch<long>), genericTypeCpde, self.TypeToCode(type), out node);
			RuleBroadcaster executor = node as RuleBroadcaster;
			return executor;
		}


		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleBroadcast<R> GetRuleBroadcast<R>(this WorldLine self, long genericTypeCpde, out RuleBroadcast<R> ruleExecutor)
			where R : IGlobalRule
		{
			var executor = self.GetRuleBroadcast(genericTypeCpde);
			ruleExecutor = executor as RuleBroadcast<R>;
			return ruleExecutor;
		}
	}

	/// <summary>
	/// 全局法则执行器管理器
	/// </summary>
	public class RuleBroadcastManager : Node, CoreManagerOf<WorldLine>
		, AsGenericBranch<long>
		, AsRule<Awake>
	{ }
}
