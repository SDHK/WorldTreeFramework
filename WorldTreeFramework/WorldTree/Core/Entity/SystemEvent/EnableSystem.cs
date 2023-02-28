
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
    public interface IEnableSystem : ISendSystem { }

    /// <summary>
    /// 活跃启用事件系统
    /// </summary>
    public abstract class EnableSystem<T> : SendSystemBase<IEnableSystem, T>
    where T : Entity
    {
        public override void Invoke(Entity self)
        {
            if (self.IsActive != self.activeEventMark)
            {
                if (self.IsActive)
                {
                    OnEvent(self as T);
                }
                self.activeEventMark = self.IsActive;
            }
        }
    }
}
