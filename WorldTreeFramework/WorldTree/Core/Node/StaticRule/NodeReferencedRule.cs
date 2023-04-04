
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
            if (self.m_ReferencedsBy is null) self.m_ReferencedsBy = self.PoolGet<UnitDictionary<long, INode>>();
            if (node.m_Referenceds is null) node.m_Referenceds = node.PoolGet<UnitDictionary<long, INode>>();

            self.m_ReferencedsBy.TryAdd(node.Id, node);
            node.m_Referenceds.TryAdd(self.Id, node);
        }

        /// <summary>
        /// 解除引用关系
        /// </summary>
        public static void DeReferenced(this INode self, INode node)
        {
            if (self.m_ReferencedsBy != null)//移除子级
            {
                self.m_ReferencedsBy.Remove(node.Id);
                if (self.m_ReferencedsBy.Count == 0)
                {
                    self.m_ReferencedsBy.Dispose();
                    self.m_ReferencedsBy = null;
                }
            }

            if (node.m_Referenceds != null)//移除父级
            {
                node.m_Referenceds.Remove(self.Id);
                if (node.m_Referenceds.Count == 0)
                {
                    node.m_Referenceds.Dispose();
                    node.m_Referenceds = null;
                }
            }
        }
    }
}
