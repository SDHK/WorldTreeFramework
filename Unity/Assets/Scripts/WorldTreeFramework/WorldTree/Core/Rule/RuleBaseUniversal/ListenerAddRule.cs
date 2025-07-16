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
	public interface AsListenerAddRule : AsRule<ListenerAdd> { }


	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public interface ListenerAdd : IListenerRule { }


	/// <summary>
	/// 监听节点添加法则
	/// </summary>
	public abstract class NodeListenerAddRule<LN, TN> : NodeListenerRule<LN, ListenerAdd, TN> where LN : class, INodeListener, AsRule<ListenerAdd> where TN : class, INode { }

	/// <summary>
	/// 监听节点法则添加法则
	/// </summary>
	public abstract class RuleListenerAddRule<LN, TR> : RuleListenerRule<LN, ListenerAdd, TR> where LN : class, INodeListener, AsRule<ListenerAdd> where TR : IRule { }

}
