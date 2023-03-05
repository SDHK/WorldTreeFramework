
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/30 14:50

* 描述： 全局实体移除事件动态监听器
* 
* 用于广播监听全局实体移除

*/

namespace WorldTree
{
    /// <summary>
    /// 全局实体移除事件动态监听器
    /// </summary>
    public class GlobalEntityRemoveListener : Node
    {
        public SystemBroadcast systemBroadcast;
    }

    class GlobalEntityRemoveListenerAddSystem : AddRule<GlobalEntityRemoveListener>
    {
        public override void OnEvent(GlobalEntityRemoveListener self)
        {
            self.TryParentTo(out self.systemBroadcast);
        }
    }
    class GlobalEntityRemoveListenerRemoveSystem : RemoveRule<GlobalEntityRemoveListener>
    {
        public override void OnEvent(GlobalEntityRemoveListener self)
        {
            self.systemBroadcast = null;
        }
    }
    class GlobalEntityRemoveListenerListenerRemoveSystem : ListenerRemoveSystem<GlobalEntityRemoveListener>
    {
        public override void OnEvent(GlobalEntityRemoveListener self, Node entity)
        {
            self.systemBroadcast?.Remove(entity);
        }
    }
}
