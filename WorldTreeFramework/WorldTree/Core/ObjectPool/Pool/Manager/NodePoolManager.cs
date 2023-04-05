/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 节点对象池管理器。
* 
*/
using System;

namespace WorldTree
{

    public static class NodePoolManagerExtension
    {
        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static T PoolGet<T>(this INode self)
        where T : class, INode
        {
            return self.Core.GetNode<T>();
        }



        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static INode PoolGet(this INode self, Type type)
        {
            return self.Core.GetNode(type);
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public static void PoolRecycle(this INode self, INode obj)
        {
            self.Core.Recycle(obj);
        }

    }

    public static class NodePoolManagerStaticRule
    {

    }

    /// <summary>
    /// 节点对象池管理器
    /// </summary>
    public class NodePoolManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {

        public UnitDictionary<Type, NodePool> pools = new UnitDictionary<Type, NodePool>();

        /// <summary>
        /// 释放后
        /// </summary>
        public override void OnDispose()
        {
            IsRecycle = true;
            IsDisposed = true;
            pools.Clear();
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public T Get<T>()
        where T : class, INode
        {
            Type type = typeof(T);
            return GetPool(type).Get<T>();
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public object Get(Type type)
        {
            return GetPool(type).Get();
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(INode obj)
        {
            if (obj != this && !(obj is NodePool))//禁止回收自己和对象池
            {
                GetPool(obj.GetType()).Recycle(obj);
            }
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public NodePool GetPool<T>()
            where T : class, INode
        {
            Type type = typeof(T);
            return GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public NodePool GetPool(Type type)
        {
            if (!pools.TryGetValue(type, out NodePool pool))
            {
                pool = new NodePool(type);
                pool.Id = Core.IdManager.GetId();
                pool.Core = Core;
                pool.Root = Root;
                pool.Branch = Branch;
                pool.Type = pool.GetType();
                pools.Add(type, pool);
                this.AddChild(pool);
            }
            return pool;
        }

        /// <summary>
        /// 尝试获取池
        /// </summary>
        public bool TryGetPool(Type type, out NodePool pool)
        {
            return pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool<T>()
        {
            Type type = typeof(T);
            if (pools.TryGetValue(type, out NodePool pool))
            {
                pool.Dispose();
                pools.Remove(type);
            }
        }
    }
}
