/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 节点对象池管理器。
* 
*/
using System;

namespace WorldTree
{

    public static partial class NodeRule
    {
        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static T PoolGet<T>(this INode self)
        where T : class, INode
        {
            return self.Core.GetNode<T>();
        }

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static INode PoolGet(this INode self, Type type)
        {
            return self.Core.GetNode(type);
        }
    }


    /// <summary>
    /// 节点对象池管理器
    /// </summary>
    public class NodePoolManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {
        public TreeDictionary<Type, NodePool> m_Pools;
    }

    class NodePoolManagerAddRule : AddRule<NodePoolManager>
    {
        public override void OnEvent(NodePoolManager self)
        {
            self.AddChild(out self.m_Pools);
        }
    }

    class NodePoolManagerRemoveRule : RemoveRule<NodePoolManager>
    {
        public override void OnEvent(NodePoolManager self)
        {
            self.m_Pools = null;
        }
    }


    public static class NodePoolManagerRule
    {
        /// <summary>
        /// 获取节点
        /// </summary>
        public static T Get<T>(this NodePoolManager self)
        where T : class, INode
        {
            Type type = typeof(T);
            return self.GetPool(type).Get<T>();
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public static object Get(this NodePoolManager self, Type type)
        {
            return self.GetPool(type).Get();
        }


        /// <summary>
        /// 尝试回收对象
        /// </summary>
        public static bool TryRecycle(this NodePoolManager self, INode obj)
        {
            if (self.m_Pools != null)
                if (self.m_Pools.TryGetValue(obj.GetType(), out NodePool nodePool))
                {
                    nodePool.Recycle(obj);
                    return true;

                }
            return false;
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public static NodePool GetPool<T>(this NodePoolManager self)
            where T : class, INode
        {
            Type type = typeof(T);
            return self.GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public static NodePool GetPool(this NodePoolManager self, Type type)
        {
            if (!self.m_Pools.TryGetValue(type, out NodePool pool))
            {
                pool = new NodePool(type);
                pool.Id = self.Core.IdManager.GetId();
                pool.Core = self.Core;
                pool.Root = self.Root;
                pool.Branch = self.Branch;
                pool.Type = pool.GetType();
                self.m_Pools.Add(type, pool);
                self.AddChild(pool);
            }
            return pool;
        }

        /// <summary>
        /// 尝试获取池
        /// </summary>
        public static bool TryGetPool(this NodePoolManager self, Type type, out NodePool pool)
        {
            return self.m_Pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public static void DisposePool<T>(this NodePoolManager self)
        {
            Type type = typeof(T);
            self.DisposePool(type);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public static void DisposePool(this NodePoolManager self, Type type)
        {
            if (self.m_Pools.TryGetValue(type, out NodePool pool))
            {
                self.m_Pools.Remove(type);
                pool.Dispose();
            }
        }
    }

}
