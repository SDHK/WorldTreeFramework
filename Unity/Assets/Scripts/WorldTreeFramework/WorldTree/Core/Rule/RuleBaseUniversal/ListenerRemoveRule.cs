/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:20

* 描述： 监听节点移除法则

*/

namespace WorldTree
{
	/// <summary>
	/// 法则约束 : 监听节点移除法则
	/// </summary>
	public interface AsListenerRemoveRule : AsRule<IListenerRemoveRule>
	{ }

	/// <summary>
	/// 监听节点移除法则接口
	/// </summary>
	public interface IListenerRemoveRule : IListenerRule
	{ }

	/// <summary>
	/// 监听节点移除法则
	/// </summary>
	/// <remarks>目标为INode和IRule时为动态监听</remarks>
	public static class ListenerRemoveRule
	{
		/// <summary>
		/// 【动态】监听节点移除法则
		/// </summary>
		public abstract class NodeRule<LN> : NodeRuleListenerRule<LN, IListenerRemoveRule> where LN : class, IDynamicNodeListener, AsRule<IListenerRemoveRule>
		{ }

		/// <summary>
		/// 【静态】监听节点移除法则
		/// </summary>
		public abstract class Node<LN, TN> : NodeListenerRuleBase<LN, IListenerRemoveRule, TN> where LN : class, INodeListener, AsRule<IListenerRemoveRule> where TN : class, INode
		{ }

		/// <summary>
		/// 【静态】监听节点移除法则
		/// </summary>
		public abstract class Rule<LN, TR> : RuleListenerRuleBase<LN, IListenerRemoveRule, TR> where LN : class, INodeListener, AsRule<IListenerRemoveRule> where TR : IRule
		{ }
	}
}