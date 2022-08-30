
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:30

* 描述： 活跃启用事件系统
* 
* 会在加入节点后触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 活跃启用事件系统接口
    /// </summary>
    public interface IEnableSystem : ISystem
    {
        void Enable(Entity self);
    }

    /// <summary>
    /// 活跃启用事件系统
    /// </summary>
    public abstract class EnableSystem<T> : SystemBase<T, IEnableSystem>, IEnableSystem
        where T : Entity
    {
        public void Enable(Entity self) => OnEnable(self as T);
        public abstract void OnEnable(T self);
    }
}
