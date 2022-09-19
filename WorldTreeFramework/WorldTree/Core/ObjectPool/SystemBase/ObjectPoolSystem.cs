
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 对象池的事件系统

*/

namespace WorldTree
{

    public interface INewSystem : ISendSystem{ }

    /// <summary>
    /// 新建事件系统
    /// </summary>
    public abstract class NewSystem<T> : SystemBase<T, INewSystem>, INewSystem
        where T : Entity
    {
        public void Invoke(Entity self) => OnNew(self as T);

        public abstract void OnNew(T self);
    }

    public interface IGetSystem : ISendSystem { }

    /// <summary>
    /// 获取事件系统
    /// </summary>
    public abstract class GetSystem<T> : SystemBase<T, IGetSystem>, IGetSystem
        where T : Entity
    {
        public void Invoke(Entity self) => OnGet(self as T);

        public abstract void OnGet(T self);
    }

    /// <summary>
    /// 回收事件系统接口
    /// </summary>
    public interface IRecycleSystem : ISendSystem { }

    /// <summary>
    /// 回收事件系统
    /// </summary>
    public abstract class RecycleSystem<T> : SystemBase<T, IRecycleSystem>, IRecycleSystem
        where T : Entity
    {
        public void Invoke(Entity self) => OnRecycle(self as T);

        public abstract void OnRecycle(T self);
    }

    /// <summary>
    /// 释放事件系统接口
    /// </summary>
    public interface IDestroySystem : ISendSystem { }
    /// <summary>
    /// 释放事件系统
    /// </summary>
    public abstract class DestroySystem<T> : SystemBase<T, IDestroySystem>, IDestroySystem
        where T : Entity
    {
        public void Invoke(Entity self) => OnDestroy(self as T);

        public abstract void OnDestroy(T self);

    }
}
