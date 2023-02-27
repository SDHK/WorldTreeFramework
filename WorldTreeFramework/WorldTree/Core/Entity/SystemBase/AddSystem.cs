
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 添加事件系统
* 
* 在加入节点时触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 添加事件系统接口
    /// </summary>
    public interface IAddSystem : ISendSystem { }

    /// <summary>
    /// 添加事件系统
    /// </summary>
    public abstract class AddSystem<E> : SendSystemBase<IAddSystem, E> where E : Entity { }
}
