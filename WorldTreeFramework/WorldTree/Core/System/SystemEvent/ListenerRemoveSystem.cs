/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:20

* 描述： 监听实体移除

*/

namespace WorldTree
{
    /// <summary>
    /// 监听实体移除
    /// </summary>
    public interface IListenerRemoveSystem : IListenerRule { }

    /// <summary>
    /// 监听实体移除系统事件
    /// </summary>
    public abstract class ListenerRemoveSystem<LE, TE, TS> : ListenerRuleBase<LE, IListenerRemoveSystem, TE, TS> where TE : Node where LE : Node where TS : IRule { }

    /// <summary>
    /// 【动态】监听实体移除系统事件
    /// </summary>
    public abstract class ListenerRemoveSystem<LE> : ListenerRuleBase<LE, IListenerRemoveSystem, Node, IRule> where LE : Node { }
}
