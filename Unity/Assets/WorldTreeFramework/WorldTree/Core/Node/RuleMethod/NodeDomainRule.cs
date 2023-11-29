/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:50

* 描述： 域节点
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
    public static class NodeDomainRule
    {
        ///// <summary>
        ///// 域节点
        ///// </summary>
        //public static UnitDictionary<Type, INode> DomainsDictionary(this INode self)
        //{
        //    if (self.m_Domains == null)
        //    {
        //        self.m_Domains = self.PoolGet<UnitDictionary<Type, INode>>();
        //    }
        //    return self.m_Domains;
        //}



        ///// <summary>
        ///// 获取所有上层节点并存入字典
        ///// </summary>
        //public static T GetDomain<T>(this INode self)
        //    where T : class, INode
        //{
        //    self.TryGetDomain(out T domain);
        //    return domain;
        //}

        ///// <summary>
        ///// 获取所有上层节点并存入字典
        ///// </summary>
        //public static bool TryGetDomain<T>(this INode self, out T domain)
        //    where T : class, INode
        //{

        //    if (self.DomainsDictionary().TryGetValue(typeof(T), out INode node))
        //    {
        //        domain = node as T;
        //        return true;
        //    }
        //    else if (self.DomainsDictionary().Count == 0)
        //    {
        //        node = self.Parent;
        //        while (node != null)
        //        {
        //            self.DomainsDictionary().TryAdd(node.GetType(), node);
        //            node = node.Parent;
        //        }
        //        if (self.DomainsDictionary().TryGetValue(typeof(T), out node))
        //        {
        //            domain = node as T;
        //            return true;
        //        }
        //    }

        //    domain = null;
        //    return false;
        //}

        ///// <summary>
        ///// 释放域
        ///// </summary>
        //public static void DisposeDomain(this INode self)
        //{
        //    if (self.m_Domains != null)
        //    {
        //        self.m_Domains.Clear();
        //        self.m_Domains.Dispose();
        //        self.m_Domains = null;
        //    }
        //}

        ///// <summary>
        ///// 层序遍历释放域
        ///// </summary>
        //public static INode TraversalLevelDisposeDomain(this INode self)
        //{
        //    UnitQueue<INode> queue = self.PoolGet<UnitQueue<INode>>();
        //    queue.Enqueue(self);

        //    while (queue.Count != 0)
        //    {
        //        var current = queue.Dequeue();

        //        current.DisposeDomain();

        //        if (current.m_Components != null)
        //        {
        //            foreach (var item in current.m_Components)
        //            {
        //                queue.Enqueue(item.Value);
        //            }
        //        }
        //        if (current.m_Children != null)
        //        {
        //            foreach (var item in current.m_Children)
        //            {
        //                queue.Enqueue(item.Value);
        //            }
        //        }
        //    }
        //    queue.Dispose();
        //    return self;
        //}
    }
}
