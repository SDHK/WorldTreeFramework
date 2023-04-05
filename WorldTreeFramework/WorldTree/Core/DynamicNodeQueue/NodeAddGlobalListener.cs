
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/23 18:17

* 描述：节点添加 全局动态监听器
* 
* 用于监听全局节点添加

*/


namespace WorldTree
{
    /// <summary>
    ///节点添加 全局动态监听器
    /// </summary>
    public class NodeAddGlobalListener : Node, IAwake, ComponentOf<DynamicNodeQueue>
    {
        public DynamicNodeQueue nodeQueue;
    }
    class NodeAddGlobalListenerAddRule : AddRule<NodeAddGlobalListener>
    {
        public override void OnEvent(NodeAddGlobalListener self)
        {
            self.TryParentTo(out self.nodeQueue);
        }
    }
    class NodeAddGlobalListenerRemoveRule : RemoveRule<NodeAddGlobalListener>
    {
        public override void OnEvent(NodeAddGlobalListener self)
        {
            self.nodeQueue = null;
        }
    }
    class NodeAddGlobalListenerListenerAddRule : ListenerAddRule<NodeAddGlobalListener>
    {
        public override void OnEvent(NodeAddGlobalListener self, INode node)
        {
            self.nodeQueue?.Enqueue(node);
        }
    }
}
