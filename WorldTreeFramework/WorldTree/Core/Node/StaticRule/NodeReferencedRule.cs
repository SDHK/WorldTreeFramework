
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/31 17:56

* 描述： 节点互相引用关系

*/

namespace WorldTree
{
    public static class NodeReferencedRule
    {

        /// <summary>
        /// 建立引用关系
        /// </summary>
        public static void Referenced(this INode self, INode node)
        {
            if (self.m_ReferencedChilden is null) self.m_ReferencedChilden = self.PoolGet<UnitDictionary<long, INode>>();
            if (node.m_ReferencedParents is null) node.m_ReferencedParents = node.PoolGet<UnitDictionary<long, INode>>();

            self.m_ReferencedChilden.TryAdd(node.Id, node);
            node.m_ReferencedParents.TryAdd(self.Id, self);
        }

        /// <summary>
        /// 解除引用关系
        /// </summary>
        public static void DeReferencedAll(this INode self)
        {
            if (self.m_ReferencedParents != null)//移除父级
            {
                foreach (var item in self.m_ReferencedParents)
                {
                    item.Value.m_ReferencedChilden.Remove(self.Id);
                    if (item.Value.m_ReferencedChilden.Count == 0)
                    {
                        item.Value.m_ReferencedChilden.Dispose();
                        item.Value.m_ReferencedChilden = null;
                    }
                }
                self.m_ReferencedParents.Dispose();
                self.m_ReferencedParents = null;
            }

            if (self.m_ReferencedChilden != null)//移除子级
            {
                foreach (var item in self.m_ReferencedChilden)
                {
                    item.Value.m_ReferencedParents.Remove(self.Id);
                    if (item.Value.m_ReferencedParents.Count == 0)
                    {
                        item.Value.m_ReferencedParents.Dispose();
                        item.Value.m_ReferencedParents = null;
                    }
                }
                self.m_ReferencedChilden.Dispose();
                self.m_ReferencedChilden = null;
            }
        }
    }
}
