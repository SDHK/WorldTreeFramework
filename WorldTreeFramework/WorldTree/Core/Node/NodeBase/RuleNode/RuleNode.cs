/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/11 11:31

* 描述： 法则节点基类

*/

namespace WorldTree
{

    /// <summary>
    /// 法则节点基类
    /// </summary>
    public abstract class RuleNode<N> : Node, ComponentOf<N>
        where N : class, INode
    {
        public N Node => this.Parent as N;

    }

    class RuleNodeDeReferencedParentRule<N> : DeReferencedParentRule<RuleNode<N>>
        where N : class, INode
    {
        public override void OnEvent(RuleNode<N> self, INode referencedParent)
        {
            if (self.m_ReferencedParents is null)
            {
                self.Dispose();
            }
        }
    }

    class RuleNodeReferencedParentRemoveRule<N> : ReferencedParentRemoveRule<RuleNode<N>>
        where N : class, INode
    {
        public override void OnEvent(RuleNode<N> self, INode referencedParent)
        {
            if (self.m_ReferencedParents is null)
            {
                self.Dispose();
            }
        }
    }
}
