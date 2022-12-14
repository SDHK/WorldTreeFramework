
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:31

* 描述： 活跃禁用事件系统
* 
* 同时会在移除节点前触发事件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 活跃禁用事件系统接口
    /// </summary>
    public interface IDisableSystem : ISendSystem { }

    /// <summary>
    /// 活跃禁用事件系统
    /// </summary>
    public abstract class DisableSystem<T> : SystemBase<T, IDisableSystem>, IDisableSystem
        where T : Entity
    {
        public void Invoke(Entity self)
        {
            if (self.IsActive != self.activeEventMark)
            {
                if (!self.IsActive)
                {
                    OnDisable(self as T);
                }
                self.activeEventMark = self.IsActive;
            }
        }
        public abstract void OnDisable(T self);
    }
}
