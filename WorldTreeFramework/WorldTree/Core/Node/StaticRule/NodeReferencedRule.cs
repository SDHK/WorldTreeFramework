
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
            node.m_Referenceds.TryAdd(self.Id, self);
        }

        /// <summary>
        /// 解除引用关系
        /// </summary>
        public static void DeReferencedAll(this INode self)
        {
            if (self.m_Referenceds != null)//移除父级
            {
                foreach (var item in self.m_Referenceds)
                {
                    item.Value.m_ReferencedsBy.Remove(self.Id);
                    if (item.Value.m_ReferencedsBy.Count == 0)
                    {
                        item.Value.m_ReferencedsBy.Dispose();
                        item.Value.m_ReferencedsBy = null;
                    }
                }
                self.m_Referenceds.Dispose();
                self.m_Referenceds = null;
            }

            if (self.m_ReferencedsBy != null)//移除子级
            {
                foreach (var item in self.m_ReferencedsBy)
                {
                    item.Value.m_Referenceds.Remove(self.Id);
                    if (item.Value.m_Referenceds.Count == 0)
                    {
                        item.Value.m_Referenceds.Dispose();
                        item.Value.m_Referenceds = null;
                    }
                }
                self.m_ReferencedsBy.Dispose();
                self.m_ReferencedsBy = null;
            }
        }
    }
}
