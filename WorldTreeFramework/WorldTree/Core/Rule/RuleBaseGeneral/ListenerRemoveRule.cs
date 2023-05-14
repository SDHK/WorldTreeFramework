/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:20

* 描述： 监听节点移除法则

*/

namespace WorldTree
{
    /// <summary>
    /// 监听节点移除法则接口
    /// </summary>
    public interface IListenerRemoveRule : IListenerRule { }

    /// <summary>
    /// 监听节点移除法则
    /// </summary>
    public abstract class ListenerRemoveRule<LN, TN, TR> : ListenerRuleBase<LN, IListenerRemoveRule, TN, TR> where TN : class, INode where LN : class, INode, AsRule<IListenerRemoveRule> where TR : IRule { }

    /// <summary>
    /// 【动态】监听节点移除法则
    /// </summary>
    public abstract class ListenerRemoveRule<LN> : ListenerRuleBase<LN, IListenerRemoveRule, INode, IRule> where LN : class, INode, AsRule<IListenerRemoveRule> { }
}
