
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/3 14:51

* 描述： 域节点
* 
* 用于分组，标签，获取上层节点。
* 
* 从字典查询节点是否存在，不存在则，
* 从父节点开始反向向上查询节点是否存在。
* 存在则存入字典。
* 
*/

using System;

namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 域节点
        /// </summary>
        private UnitDictionary<Type, Node> domains;

        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, Node> Domains
        {
            get
            {
                if (domains == null)
                {
                    domains = this.PoolGet<UnitDictionary<Type, Node>>();
                }
                return domains;
            }
            set { domains = value; }
        }

        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public T GetDomain<T>()
            where T : Node
        {
            TryGetDomain(out T domain);
            return domain;
        }

        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public bool TryGetDomain<T>(out T domain)
            where T : Node
        {

            if (Domains.TryGetValue(typeof(T), out Node node))
            {
                domain = node as T;
                return true;
            }
            else if (Domains.Count == 0)
            {
                node = Parent;
                while (node != null)
                {
                    Domains.TryAdd(node.GetType(), node);
                    node = node.Parent;
                }
                if (Domains.TryGetValue(typeof(T), out node))
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
        public void DisposeDomain()
        {
            if (domains != null)
            {
                domains.Clear();
                domains.Dispose();
                domains = null;
            }
        }

        /// <summary>
        /// 层序遍历释放域
        /// </summary>
        public Node TraversalLevelDisposeDomain()
        {
            UnitQueue<Node> queue = this.PoolGet<UnitQueue<Node>>();
            queue.Enqueue(this);

            while (queue.Count != 0)
            {
                var current = queue.Dequeue();

                current.DisposeDomain();

                if (current.components != null)
                {
                    foreach (var item in current.components)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
                if (current.children != null)
                {
                    foreach (var item in current.children)
                    {
                        queue.Enqueue(item.Value);
                    }
                }
            }
            queue.Dispose();
            return this;
        }

    }
}
