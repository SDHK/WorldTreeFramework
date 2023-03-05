/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 15:27

* 描述： UI窗口关闭系统

*/

namespace WorldTree
{
    /// <summary>
    /// UI窗口关闭系统接口
    /// </summary>
    public interface IWindowCloseSystem : ISendRule { }
    /// <summary>
    /// UI窗口关闭系统
    /// </summary>
    public abstract class WindowCloseSystem<E> : SendRuleBase<IWindowCloseSystem, E> where E : Node { }
}
