/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 混合类型对象池管理器。
* 

*/
using System;

namespace WorldTree
{

    public static class ObjectPoolManagerExtension
    {
        /// <summary>
        /// 获取对象池管理器
        /// </summary>
        public static ObjectPoolManager PoolManager(this Entity self)
        {
            return self.Root.ObjectPoolManager;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static T PoolGet<T>(this Entity self)
        where T : class
        {
            return self.Root.ObjectPoolManager.Get<T>();
        }


        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static object PoolGet(this Entity self, Type type)
        {
            return self.Root.ObjectPoolManager.Get(type);
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public static void PoolRecycle(this Entity self, object obj)
        {
            self.Root.ObjectPoolManager.Recycle(obj);
        }

    }

    /// <summary>
    /// 实体对象池管理器
    /// </summary>
    public class ObjectPoolManager : Entity
    {

        UnitDictionary<Type, ObjectPool> pools = new UnitDictionary<Type, ObjectPool>();

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
        where T : class
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
        public void Recycle(object obj)
        {
            if (obj != this && !(obj is ObjectPool))//禁止回收自己和对象池
            {
                GetPool(obj.GetType()).Recycle(obj);
            }
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public ObjectPool GetPool<T>()
        {
            Type type = typeof(T);
            return GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public ObjectPool GetPool(Type type)
        {
            if (!pools.TryGetValue(type, out ObjectPool pool))
            {
                pool = new ObjectPool(type);
                pool.id = Root.IdManager.GetId();
                pool.Root = Root;
                pools.Add(type, pool);
                AddChildren(pool);
            }

            return pool;
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool<T>()
        {
            Type type = typeof(T);
            if (pools.TryGetValue(type, out ObjectPool pool))
            {
                pool.Dispose();
                pools.Remove(type);
            }
        }
    }
}
