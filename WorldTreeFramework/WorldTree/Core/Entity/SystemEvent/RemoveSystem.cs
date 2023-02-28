
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
    public interface IRemoveSystem : ISendSystem { }

    /// <summary>
    /// 移除事件系统
    /// </summary>
    public abstract class RemoveSystem<E> : SendSystemBase<IRemoveSystem, E> where E : Entity { }
}
