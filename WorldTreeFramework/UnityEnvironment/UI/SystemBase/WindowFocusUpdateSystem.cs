/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 11:17

* 描述： UI窗口焦点更新系统

*/

namespace WorldTree
{
    /// <summary>
    /// UI窗口焦点更新系统接口
    /// </summary>
    public interface IWindowFocusUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// UI窗口焦点更新系统
    /// </summary>
    public abstract class WindowFocusUpdateSystem<T> : SystemBase<T, IWindowFocusUpdateSystem>, IWindowFocusUpdateSystem
        where T : Entity
    {
        public void Invoke(Entity self,float deltaTime) => OnFocusUpdate(self as T, deltaTime);
        public abstract void OnFocusUpdate(T self, float deltaTime);
    }
}
