
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
    public class GlobalEntityAddListener : Node
    {
        public SystemBroadcast systemBroadcast;
    }
    class GlobalEntityAddListenerAddSystem : AddSystem<GlobalEntityAddListener>
    {
        public override void OnEvent(GlobalEntityAddListener self)
        {
            self.TryParentTo(out self.systemBroadcast);
        }
    }
    class GlobalEntityAddListenerRemoveSystem : RemoveSystem<GlobalEntityAddListener>
    {
        public override void OnEvent(GlobalEntityAddListener self)
        {
            self.systemBroadcast = null;
        }
    }
    class GlobalEntityAddListenerListenerAddSystem : ListenerAddSystem<GlobalEntityAddListener>
    {
        public override void OnEvent(GlobalEntityAddListener self, Node entity)
        {
            self.systemBroadcast?.Enqueue(entity);
        }
    }
}
