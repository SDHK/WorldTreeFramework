/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 11:48

* 描述： UI窗口更新系统

*/

namespace WorldTree
{
    /// <summary>
    /// UI窗口更新系统接口
    /// </summary>
    public interface IWindowUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// UI窗口更新系统
    /// </summary>
    public abstract class WIndowUpdateSystem<E> : SendSystemBase<IWindowUpdateSystem, E, float> where E : Entity { }
}
