/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/11 11:31

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 事件组件基类
    /// </summary>
    /// <typeparam name="N">事件挂载到节点下</typeparam>
    public abstract class EventNode<N> : Node, IAwake, ComponentOf<N>
        where N : class, INode
    {
        public N Node => this.Parent as N;
    }

    //假如没有被引用则回收
    class EventNodeReferencedRemoveRule<N> : ReferencedChildRemoveRule<EventNode<N>>
        where N : class, INode
    {
        public override void OnEvent(EventNode<N> self, INode arg1)
        {
            if (self.m_ReferencedParents.Count == 0)
            {
                self.Dispose();
            }
        }
    }
}
