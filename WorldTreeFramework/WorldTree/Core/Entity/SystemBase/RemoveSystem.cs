
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 移除事件系统
* 
* 从节点移除时触发

*/

namespace WorldTree
{
    /// <summary>
    /// 移除事件系统接口
    /// </summary>
    public interface IRemoveSystem : ISystem
    { 
        void Remove(Entity self);
    }
    /// <summary>
    /// 移除事件系统
    /// </summary>
    public abstract class RemoveSystem<T> : SystemBase<T, IRemoveSystem>, IRemoveSystem
        where T :Entity
    {
        public void Remove(Entity self) => OnRemove(self as T);
        public abstract void OnRemove(T self);
    }
}
