/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:50

* 描述： 

*/

namespace WorldTree
{
    public static class NodeDomainStaticRule
    {
        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public static T GetDomain<T>(this Node self)
            where T : Node
        {
            self.TryGetDomain(out T domain);
            return domain;
        }

        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public static bool TryGetDomain<T>(this Node self, out T domain)
            where T : Node
        {

            if (self.Domains.TryGetValue(typeof(T), out Node node))
            {
                domain = node as T;
                return true;
            }
            else if (self.Domains.Count == 0)
            {
                node = self.Parent;
                while (node != null)
                {
                    self.Domains.TryAdd(node.GetType(), node);
                    node = node.Parent;
                }
                if (self.Domains.TryGetValue(typeof(T), out node))
                {
                    domain = node as T;
                    return true;
                }
            }

            domain = null;
            return false;
        }

        /// <summary>
        /// 释放域
        /// </summary>
        public static void DisposeDomain(this Node self)
        {
            if (self.m_Domains != null)
            {
                self.m_Domains.Clear();
                self.m_Domains.Dispose();
                self.m_Domains = null;
            }
        }

        /// <summary>
        /// 层序遍历释放域
        /// </summary>
        public static Node TraversalLevelDisposeDomain(this Node self)
        {
            UnitQueue<Node> queue = self.PoolGet<UnitQueue<Node>>();
            queue.Enqueue(self);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                current.DisposeDomain();

                if (current.m_Components != null)
                {
                    foreach (var item in current.m_Components)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
                if (current.m_Children != null)
                {
                    foreach (var item in current.m_Children)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
            }
            queue.Dispose();
            return self;
        }
    }
}
