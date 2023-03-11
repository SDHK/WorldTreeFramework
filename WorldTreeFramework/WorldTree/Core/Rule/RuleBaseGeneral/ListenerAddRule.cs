/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:17

* 描述： 监听节点添加法则

*/

namespace WorldTree
{
    /// <summary>
    /// 监听节点添加法则接口
    /// </summary>
    public interface IListenerAddRule : IListenerRule { }

    /// <summary>
    /// 监听节点添加法则
    /// </summary>
    public abstract class ListenerAddRule<LN, TN, TR> : ListenerRuleBase<LN, IListenerAddRule, TN, TR> where TN : class,INode where LN : class,INode where TR : IRule { }

    /// <summary>
    /// 【动态】监听节点添加法则
    /// </summary>
    public abstract class ListenerAddRule<LN> : ListenerRuleBase<LN, IListenerAddRule, INode, IRule> where LN : class,INode { }

}
