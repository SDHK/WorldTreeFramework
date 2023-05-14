
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
            self.m_ReferencedChilden ??= self.PoolGet<UnitDictionary<long, INode>>();
            node.m_ReferencedParents ??= node.PoolGet<UnitDictionary<long, INode>>();

            self.m_ReferencedChilden.TryAdd(node.Id, node);
            node.m_ReferencedParents.TryAdd(self.Id, self);
        }

        /// <summary>
        /// 解除引用关系
        /// </summary>
        public static void DeReferenced(this INode self, INode node)
        {
            if (self.m_ReferencedChilden != null)
            {
                if (self.m_ReferencedChilden.ContainsKey(node.Id))
                {
                    self.m_ReferencedChilden.Remove(node.Id);

                    if (self.m_ReferencedChilden.Count == 0)
                    {
                        self.m_ReferencedChilden.Dispose();
                        self.m_ReferencedChilden = null;
                    }
                    self.TrySendRule(default(IDeReferencedChildRule), node);
                }
            }

            if (node.m_ReferencedParents != null)
            {
                if (node.m_ReferencedParents.ContainsKey(self.Id))
                {
                    node.m_ReferencedParents.Remove(self.Id);
                    if (node.m_ReferencedParents.Count == 0)
                    {
                        node.m_ReferencedParents.Dispose();
                        node.m_ReferencedParents = null;
                    }
                    node.TrySendRule(default(IDeReferencedParentRule), self);
                }
            }
        }

        /// <summary>
        /// 解除所有引用关系
        /// </summary>
        public static void DeReferencedAll(this INode self)
        {
            if (self.m_ReferencedParents != null)//移除父级
            {
                using (self.PoolGet(out UnitQueue<INode> nodeIdQueue))
                {
                    foreach (var item in self.m_ReferencedParents)
                    {
                        nodeIdQueue.Enqueue(item.Value);
                    }

                    while (nodeIdQueue.Count != 0)
                    {
                        nodeIdQueue.Dequeue().DeReferenced(self);
                    }
                }
            }

            if (self.m_ReferencedChilden != null)//移除子级
            {
                using (self.PoolGet(out UnitQueue<INode> nodeIdQueue))
                {
                    foreach (var item in self.m_ReferencedChilden)
                    {
                        nodeIdQueue.Enqueue(item.Value);
                    }
                    while (nodeIdQueue.Count != 0)
                    {
                        self.DeReferenced(nodeIdQueue.Dequeue());
                    }
                }
            }
        }

        /// <summary>
        /// 解除所有引用关系, 通知自己的移除生命周期事件
        /// </summary>
        public static void SendAllReferencedNodeRemove(this INode self)
        {
            if (self.m_ReferencedParents != null)//移除父级
            {
                using (self.PoolGet(out UnitQueue<INode> nodeIdQueue))
                {
                    foreach (var item in self.m_ReferencedParents)
                    {
                        nodeIdQueue.Enqueue(item.Value);
                    }

                    while (nodeIdQueue.Count != 0)
                    {
                        var node = nodeIdQueue.Dequeue();
                        //node.DeReferenced(self);

                        if (node.m_ReferencedChilden != null)
                        {
                            if (node.m_ReferencedChilden.ContainsKey(self.Id))
                            {
                                node.m_ReferencedChilden.Remove(self.Id);

                                if (node.m_ReferencedChilden.Count == 0)
                                {
                                    node.m_ReferencedChilden.Dispose();
                                    node.m_ReferencedChilden = null;
                                }
                            }
                        }

                        self.m_ReferencedParents.Remove(node.Id);
                        if (self.m_ReferencedParents.Count == 0)
                        {
                            self.m_ReferencedParents.Dispose();
                            self.m_ReferencedParents = null;
                        }

                        node.TrySendRule(default(IReferencedChildRemoveRule), self);
                    }
                }
            }

            if (self.m_ReferencedChilden != null)//移除子级
            {
                using (self.PoolGet(out UnitQueue<INode> nodeIdQueue))
                {
                    foreach (var item in self.m_ReferencedChilden)
                    {
                        nodeIdQueue.Enqueue(item.Value);
                    }

                    while (nodeIdQueue.Count != 0)
                    {
                        var node = nodeIdQueue.Dequeue();
                        //self.DeReferenced(node);

                        self.m_ReferencedChilden.Remove(node.Id);
                        if (self.m_ReferencedChilden.Count == 0)
                        {
                            self.m_ReferencedChilden.Dispose();
                            self.m_ReferencedChilden = null;
                        }

                        if (node.m_ReferencedParents != null)
                        {
                            if (node.m_ReferencedParents.ContainsKey(self.Id))
                            {
                                node.m_ReferencedParents.Remove(self.Id);
                                if (node.m_ReferencedParents.Count == 0)
                                {
                                    node.m_ReferencedParents.Dispose();
                                    node.m_ReferencedParents = null;
                                }
                            }
                        }
                        node.TrySendRule(default(IReferencedParentRemoveRule), self);
                    }
                }
            }
        }
    }
}
