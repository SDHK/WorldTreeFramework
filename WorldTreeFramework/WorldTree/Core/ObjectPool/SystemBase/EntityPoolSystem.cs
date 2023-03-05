
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 实体对象池的事件系统

*/

namespace WorldTree
{
    /// <summary>
    /// 新建事件系统接口
    /// </summary>
    public interface INewSystem : ISendRule { }

    /// <summary>
    /// 新建事件系统
    /// </summary>
    public abstract class NewSystem<E> : SendRuleBase<INewSystem, E> where E : Node { }

    /// <summary>
    /// 获取事件系统接口
    /// </summary>
    public interface IGetSystem : ISendRule { }

    /// <summary>
    /// 获取事件系统
    /// </summary>
    public abstract class GetSystem<E> : SendRuleBase<IGetSystem, E> where E : Node { }

    /// <summary>
    /// 回收事件系统接口
    /// </summary>
    public interface IRecycleSystem : ISendRule { }

    /// <summary>
    /// 回收事件系统
    /// </summary>
    public abstract class RecycleSystem<E> : SendRuleBase<IRecycleSystem, E> where E : Node { }

    /// <summary>
    /// 释放事件系统接口
    /// </summary>
    public interface IDestroySystem : ISendRule { }
    /// <summary>
    /// 释放事件系统
    /// </summary>
    public abstract class DestroySystem<E> : SendRuleBase<IDestroySystem, E> where E : Node { }
}
