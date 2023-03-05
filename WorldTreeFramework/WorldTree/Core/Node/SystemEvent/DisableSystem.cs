
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
    public abstract class DisableSystem<T> : SendSystemBase<IDisableSystem, T>
        where T : Node
    {
        public override void Invoke(Node self)
        {
            if (self.IsActive != self.activeEventMark)
            {
                if (!self.IsActive)
                {
                    OnEvent(self as T);
                }
                self.activeEventMark = self.IsActive;
            }
        }
    }
}
