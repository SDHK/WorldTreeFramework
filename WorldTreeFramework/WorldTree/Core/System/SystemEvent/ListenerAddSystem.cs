/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/28 10:17

* 描述： 监听实体添加

*/

namespace WorldTree
{
    /// <summary>
    /// 监听实体添加
    /// </summary>
    public interface IListenerAddSystem : IListenerSystem { }

    /// <summary>
    /// 监听实体添加系统事件
    /// </summary>
    public abstract class ListenerAddSystem<LE, TE, TS> : ListenerSystemBase<LE, IListenerAddSystem, TE, TS> where TE : Entity where LE : Entity where TS : IEntitySystem { }

    /// <summary>
    /// 【动态】监听实体添加系统事件
    /// </summary>
    public abstract class ListenerAddSystem<LE> : ListenerSystemBase<LE, IListenerAddSystem, Entity, IEntitySystem> where LE : Entity { }

}
