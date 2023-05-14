
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/30 14:50

* 描述： 节点移除 动态全局监听器
* 
* 用于监听全局节点移除

*/

namespace WorldTree
{
    /// <summary>
    /// 节点移除 动态全局监听器
    /// </summary>
    public class NodeRemoveGlobalListener : Node, ComponentOf<DynamicNodeQueue>
        ,AsRule<IAwakeRule>
        ,AsRule<IAddRule>
        ,AsRule<IRemoveRule>
        ,AsRule<IListenerRemoveRule>
    {
        public DynamicNodeQueue nodeQueue;
    }

    class NodeRemoveGlobalListenerAddRule : AddRule<NodeRemoveGlobalListener>
    {
        public override void OnEvent(NodeRemoveGlobalListener self)
        {
            self.TryParentTo(out self.nodeQueue);
        }
    }
    class NodeRemoveGlobalListenerRemoveRule : RemoveRule<NodeRemoveGlobalListener>
    {
        public override void OnEvent(NodeRemoveGlobalListener self)
        {
            self.nodeQueue = null;
        }
    }
    class NodeRemoveGlobalListenerListenerRemoveRule : ListenerRemoveRule<NodeRemoveGlobalListener>
    {
        public override void OnEvent(NodeRemoveGlobalListener self, INode node)
        {
            self.nodeQueue?.Remove(node);
        }
    }
}
