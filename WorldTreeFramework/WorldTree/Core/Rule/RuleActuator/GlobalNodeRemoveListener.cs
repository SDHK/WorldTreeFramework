
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/30 14:50

* 描述： 全局节点移除事件动态监听器
* 
* 用于监听全局节点移除

*/

namespace WorldTree
{
    /// <summary>
    /// 全局节点移除事件动态监听器
    /// </summary>
    public class GlobalNodeRemoveListener : Node
    {
        public DynamicNodeQueue nodeQueue;
    }

    class GlobalNodeRemoveListenerAddSystem : AddRule<GlobalNodeRemoveListener>
    {
        public override void OnEvent(GlobalNodeRemoveListener self)
        {
            self.TryParentTo(out self.nodeQueue);
        }
    }
    class GlobalNodeRemoveListenerRemoveSystem : RemoveRule<GlobalNodeRemoveListener>
    {
        public override void OnEvent(GlobalNodeRemoveListener self)
        {
            self.nodeQueue = null;
        }
    }
    class GlobalNodeRemoveListenerListenerRemoveSystem : ListenerRemoveRule<GlobalNodeRemoveListener>
    {
        public override void OnEvent(GlobalNodeRemoveListener self, Node node)
        {
            self.nodeQueue?.Remove(node);
        }
    }
}
