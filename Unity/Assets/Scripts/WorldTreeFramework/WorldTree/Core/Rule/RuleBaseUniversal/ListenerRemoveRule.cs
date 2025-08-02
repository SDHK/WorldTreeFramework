/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:20

* 描述： 监听节点移除法则

*/

namespace WorldTree
{
	/// <summary>
	/// 监听节点法则移除法则
	/// </summary>
	public interface ListenerRemove : IListenerRule
	{ }


	/// <summary>
	/// 监听节点添加法则
	/// </summary>
	public abstract class ListenerNodeRemoveRule<LN, TN> : ListenerRule<LN, ListenerRemove, TN, IRule> where LN : class, INodeListener, AsRule<ListenerRemove> where TN : class, INode { }


	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public abstract class ListenerRuleRemvoeRule<LN, TR> : ListenerRule<LN, ListenerRemove, INode, TR> where LN : class, INodeListener, AsRule<ListenerRemove> where TR : IRule { }

}