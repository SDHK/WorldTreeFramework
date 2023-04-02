
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
        public static void DeReferenced(this INode self)
        {
            if (self.m_ReferencedsBy != null)//从绑定子级移除自己
            {
                foreach (var nodeKV in self.m_ReferencedsBy)
                {
                    nodeKV.Value.m_Referenceds?.Remove(self.Id);
                    if (nodeKV.Value.m_Referenceds.Count == 0)
                    {
                        nodeKV.Value.m_Referenceds.Dispose();
                        nodeKV.Value.m_Referenceds = null;
                    }
                }
            }

            if (self.m_Referenceds != null)//从绑定父级移除自己
            {
                foreach (var nodeKV in self.m_Referenceds)
                {
                    nodeKV.Value.m_ReferencedsBy?.Remove(self.Id);
                    if (nodeKV.Value.m_ReferencedsBy.Count == 0)
                    {
                        nodeKV.Value.m_ReferencedsBy.Dispose();
                        nodeKV.Value.m_ReferencedsBy = null;
                    }
                }
            }
        }
    }
}
