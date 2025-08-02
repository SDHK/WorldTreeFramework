/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:17

* 描述： 监听节点添加法则

*/

namespace WorldTree
{

	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public interface ListenerAdd : IListenerRule { }


	/// <summary>
	/// 监听节点添加法则
	/// </summary>
	public abstract class ListenerNodeAddRule<LN, TN> : ListenerRule<LN, ListenerAdd, TN, IRule> where LN : class, INodeListener, AsRule<ListenerAdd> where TN : class, INode { }

	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public abstract class ListenerRuleAddRule<LN, TR> : ListenerRule<LN, ListenerAdd, INode, TR> where LN : class, INodeListener, AsRule<ListenerAdd> where TR : IRule { }

}
