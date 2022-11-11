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
    public interface IWindowCloseSystem : ISendSystem { }
    /// <summary>
    /// UI窗口关闭系统
    /// </summary>
    public abstract class WindowCloseSystem<T> : SystemBase<T, IWindowCloseSystem>, IWindowCloseSystem
    where T : Entity
    {
        public void Invoke(Entity self) => OnClose(self as T);
        public abstract void OnClose(T self);
    }
}
