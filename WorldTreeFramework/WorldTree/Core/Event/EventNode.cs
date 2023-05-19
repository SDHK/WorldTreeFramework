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
    public abstract class EventNode<N> : Node, ComponentOf<N>
        where N : class, INode
    {
        public N Node => this.Parent as N;

    }

    class EventNodeDeReferencedParentRule<N> : DeReferencedParentRule<EventNode<N>>
        where N : class, INode
    {
        public override void OnEvent(EventNode<N> self, INode referencedParent)
        {
            if (self.m_ReferencedParents is null)
            {
                self.Dispose();
            }
        }
    }

    class EventNodeReferencedParentRemoveRule<N> : ReferencedParentRemoveRule<EventNode<N>>
        where N : class, INode
    {
        public override void OnEvent(EventNode<N> self, INode referencedParent)
        {
            if (self.m_ReferencedParents is null)
            {
                self.Dispose();
            }
        }
    }
}
