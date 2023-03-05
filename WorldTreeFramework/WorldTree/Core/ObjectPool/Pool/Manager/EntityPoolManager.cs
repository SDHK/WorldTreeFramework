/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池管理器。
* 
*/
using System;

namespace WorldTree
{

    public static class EntityPoolManagerExtension
    {
        /// <summary>
        /// 获取对象池管理器
        /// </summary>
        public static EntityPoolManager PoolManager(this Node self)
        {
            return self.Root.EntityPoolManager;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static T PoolGet<T>(this Node self)
        where T : Node
        {
            return self.Root.EntityPoolManager.Get<T>();
        }


        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static Node PoolGet(this Node self, Type type)
        {
            return self.Root.EntityPoolManager.Get(type) as Node;
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public static void PoolRecycle(this Node self, Node obj)
        {
            self.Root.EntityPoolManager.Recycle(obj);
        }

    }

    /// <summary>
    /// 实体对象池管理器
    /// </summary>
    public class EntityPoolManager : Node
    {

       public UnitDictionary<Type, EntityPool> pools = new UnitDictionary<Type, EntityPool>();

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
        /// 获取实体
        /// </summary>
        public T Get<T>()
        where T : Node
        {
            Type type = typeof(T);
            return GetPool(type).Get<T>();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        public object Get(Type type)
        {
            return GetPool(type).Get();
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(Node obj)
        {
            if (obj != this && !(obj is EntityPool))//禁止回收自己和对象池
            {
                GetPool(obj.GetType()).Recycle(obj);
            }
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public EntityPool GetPool<T>()
            where T: Node
        {
            Type type = typeof(T);
            return GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public EntityPool GetPool(Type type)
        {
            if (!pools.TryGetValue(type, out EntityPool pool))
            {
                pool = new EntityPool(type);
                pool.id = Root.IdManager.GetId();
                pool.Root = Root;
                pools.Add(type, pool);
                AddChildren(pool);
            }

            return pool;
        }

        /// <summary>
        /// 尝试获取池
        /// </summary>
        public bool TryGetPool(Type type,out EntityPool pool)
        {
            return pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool<T>()
        {
            Type type = typeof(T);
            if (pools.TryGetValue(type, out EntityPool pool))
            {
                pool.Dispose();
                pools.Remove(type);
            }
        }
    }
}
