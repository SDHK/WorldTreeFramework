using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    /// <summary>
    /// 对象池管理器基类
    /// </summary>
    public abstract class PoolManagerBase : CoreNode
     , ComponentOf<WorldTreeCore>
     , AsRule<IAwakeRule>

    {
        /// <summary>
        /// 忽略类型名单
        /// </summary>
        public TreeHashSet<Type> m_IgnoreTypeHashSet = new TreeHashSet<Type>();

        public override void Dispose()
        {
            this.IsRecycle = true;
            this.IsDisposed = true;
            this.SetActive(false);
            base.Dispose();
        }
    }

    /// <summary>
    /// 对象池管理器泛型基类
    /// </summary>
    public abstract class PoolManagerBase<T> : PoolManagerBase
        where T : PoolBase, ChildOf<PoolManagerBase<T>>
    {
        /// <summary>
        /// 池集合字典
        /// </summary>
        public TreeDictionary<Type, T> m_Pools = new TreeDictionary<Type, T>();


        /// <summary>
        /// 尝试新建或获取对象池
        /// </summary>
        public virtual bool TryNewOrGetPool(Type type, out T pool)
        {
            //忽略类型表检测
            if (!m_IgnoreTypeHashSet.Contains(type))
            {
                //不存在则新建
                if (!m_Pools.TryGetValue(type, out pool))
                {
                    pool = NewPool(type);
                }
                return true;
            }
            pool = null;
            return false;
        }

        /// <summary>
        /// 新建池
        /// </summary>
        private T NewPool(Type type)
        {
            this.Core.NewNodeLifecycle(out T pool);
            pool.ObjectType = type;
            this.m_Pools.Add(type, pool);
            this.AddChild(pool);
            return pool;
        }

        /// <summary>
        /// 尝试获取对象
        /// </summary>
        public bool TryGet(Type type, out object obj)
        {
            if (TryNewOrGetPool(type, out T pool))
            {
                obj = pool.GetObject();
                return true;
            }
            else
            {
                obj = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试回收对象
        /// </summary>
        public bool TryRecycle(object obj)
        {
            if (TryNewOrGetPool(obj.GetType(), out T pool))
            {
                pool.Recycle(obj);
                return true;
            }
            else if (obj is T Tpool)
            {
                m_Pools.Remove(Tpool.ObjectType);
            }
            return false;
        }



        /// <summary>
        /// 尝试获取对象池
        /// </summary>
        public virtual bool TryGetPool(Type type, out T pool)
        {
            return m_Pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public virtual void DisposePool(Type type)
        {
            if (m_Pools.TryGetValue(type, out T pool))
            {
                pool.Dispose();
            }
        }
    }

    //class AddRule : AddRule<NodePoolManager>
    //{
    //    public override void OnEvent(NodePoolManager self)
    //    {
    //        self.AddChild(out self.m_Pools);
    //    }
    //}

    //class RemoveRule : RemoveRule<NodePoolManager>
    //{
    //    public override void OnEvent(NodePoolManager self)
    //    {
    //        self.m_Pools = null;
    //    }
    //}



}
