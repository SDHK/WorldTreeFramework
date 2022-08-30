/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 实体对象池管理器。
* 为所有实体对象池的管理器。

*/
using System;

namespace WorldTree
{

    public static class ObjectPoolManagerExtension
    {
        public static ObjectPoolManager EntityPoolManager(this Entity self)
        {
            return self.Root.ObjectPoolManager;
        }
    }

    class ObjectPoolManagerRemove : RemoveSystem<ObjectPoolManager>
    {
        public override void OnRemove(ObjectPoolManager self)
        {
            self.Dispose();//全部释放
        }
    }

    /// <summary>
    /// 实体对象池管理器
    /// </summary>
    public class ObjectPoolManager : Entity
    {

        UnitDictionary<Type, ObjectPool> pools = new UnitDictionary<Type, ObjectPool>();

        public override void OnDispose()
        {
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
                GetPool( obj.GetType()).Recycle(obj);
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
                pools.Remove(type);
            }
        }
    }
}
