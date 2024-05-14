/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:17

* 描述： 监听节点添加法则

*/

namespace WorldTree
{
	/// <summary>
	/// 法则约束 : 监听节点添加法则
	/// </summary>
	public interface AsListenerAddRule : AsRule<IListenerAddRule> { }
	/// <summary>
	/// 监听节点添加法则接口
	/// </summary>
	public interface IListenerAddRule : IListenerRule { }

	public static class ListenerAddRule
	{
		/// <summary>
		/// 【动态】监听节点添加法则
		/// </summary>
		public abstract class NodeRule<LN> : NodeRuleListenerRuleBase<LN, IListenerAddRule> where LN : class, IDynamicNodeListener, AsRule<IListenerAddRule> { }
		/// <summary>
		/// 【静态】监听节点添加法则
		/// </summary>
		public abstract class Node<LN, TN> : NodeListenerRuleBase<LN, IListenerAddRule, TN> where LN : class, INodeListener, AsRule<IListenerAddRule> where TN : class, INode { }
		/// <summary>
		/// 【静态】监听节点添加法则
		/// </summary>
		public abstract class Rule<LN, TR> : RuleListenerRuleBase<LN, IListenerAddRule, TR> where LN : class, INodeListener, AsRule<IListenerAddRule> where TR : IRule { }
	}
}
