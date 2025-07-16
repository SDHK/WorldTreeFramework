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
	public interface AsListenerRemoveRule : AsRule<ListenerRemove>
	{ }


	/// <summary>
	/// 监听节点法则移除法则
	/// </summary>
	public interface ListenerRemove : IListenerRule
	{ }


	/// <summary>
	/// 监听节点添加法则
	/// </summary>
	public abstract class NodeListenerRemoveRule<LN, TN> : NodeListenerRule<LN, ListenerRemove, TN> where LN : class, INodeListener, AsRule<ListenerRemove> where TN : class, INode { }


	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public abstract class RuleListenerRemvoeRule<LN, TR> : RuleListenerRule<LN, ListenerRemove, TR> where LN : class, INodeListener, AsRule<ListenerRemove> where TR : IRule { }

}