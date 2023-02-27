
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
    public interface INewSystem : ISendSystem { }

    /// <summary>
    /// 新建事件系统
    /// </summary>
    public abstract class NewSystem<E> : SendSystemBase<INewSystem, E> where E : Entity { }

    /// <summary>
    /// 获取事件系统接口
    /// </summary>
    public interface IGetSystem : ISendSystem { }

    /// <summary>
    /// 获取事件系统
    /// </summary>
    public abstract class GetSystem<E> : SendSystemBase<IGetSystem, E> where E : Entity { }

    /// <summary>
    /// 回收事件系统接口
    /// </summary>
    public interface IRecycleSystem : ISendSystem { }

    /// <summary>
    /// 回收事件系统
    /// </summary>
    public abstract class RecycleSystem<E> : SendSystemBase<IRecycleSystem, E> where E : Entity { }

    /// <summary>
    /// 释放事件系统接口
    /// </summary>
    public interface IDestroySystem : ISendSystem { }
    /// <summary>
    /// 释放事件系统
    /// </summary>
    public abstract class DestroySystem<E> : SendSystemBase<IDestroySystem, E> where E : Entity { }
}
