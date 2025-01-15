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
		public static GlobalRuleActuator<R> GetOrNewGlobalRuleActuator<R>(this WorldTreeCore self, out GlobalRuleActuator<R> globalRuleActuator)
		where R : IGlobalRule
		{
			return self.AddComponent(out GlobalRuleActuatorManager _).AddTypeNode(self.TypeToCode<R>(), out globalRuleActuator);
		}

		/// <summary>
		/// 强制获取全局节点法则执行器
		/// </summary>
		public static RuleGroupActuatorBase GetOrNewGlobalRuleActuator(this WorldTreeCore self, long typeCpde)
		{
			Type type = self.CodeToType(typeCpde);
			Type type1 = typeof(GlobalRuleActuator<>);
			Type type2 = type1.MakeGenericType(type);
			long typeCpde2 = self.TypeToCode(type2);
			self.AddComponent(out GlobalRuleActuatorManager manager);
			NodeBranchHelper.AddNode<TypeNodeBranch, long>(manager, typeCpde, typeCpde2, out INode node);
			return node as RuleGroupActuatorBase;
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
