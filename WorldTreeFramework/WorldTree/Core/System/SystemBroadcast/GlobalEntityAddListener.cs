
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/23 18:17

* 描述：全局实体添加事件动态监听器
* 
* 用于广播监听全局实体添加

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 全局实体添加事件动态监听器
    /// </summary>
    public class GlobalEntityAddListener : Entity
    {
        public SystemBroadcast systemBroadcast;
    }
    class GlobalEntityAddListenerAddSystem : AddSystem<GlobalEntityAddListener>
    {
        public override void OnAdd(GlobalEntityAddListener self)
        {
            self.TryGetParent(out self.systemBroadcast);
        }
    }
    class GlobalEntityAddListenerRemoveSystem : RemoveSystem<GlobalEntityAddListener>
    {
        public override void OnRemove(GlobalEntityAddListener self)
        {
            self.systemBroadcast = null;
        }
    }
    class GlobalEntityAddListenerListenerAddSystem : ListenerAddSystem<GlobalEntityAddListener>
    {
        public override void OnAdd(GlobalEntityAddListener self, Entity entity)
        {
            self.systemBroadcast?.AddEntity(entity);
        }
    }
}
