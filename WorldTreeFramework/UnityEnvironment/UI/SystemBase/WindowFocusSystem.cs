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
    public abstract class WindowFocusSystem<T> : SystemBase<T, IWindowFocusSystem>, IWindowFocusSystem
    where T : Entity
    {
        public void Invoke(Entity self) => OnFocus(self as T);
        public abstract void OnFocus(T self);
    }
}
