/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 10:56

* 描述： UI窗口焦点系统

*/

namespace WorldTree
{

    /// <summary>
    /// UI窗口焦点系统接口
    /// </summary>
    public interface IWindowFocusSystem : ISendSystem { }

    /// <summary>
    /// UI窗口焦点系统
    /// </summary>
    public abstract class WindowFocusSystem<E> : SendSystemBase<IWindowFocusSystem, E> where E : Node { }
}
