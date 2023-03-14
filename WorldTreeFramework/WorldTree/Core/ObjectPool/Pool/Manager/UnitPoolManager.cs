
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 11:05

* 描述： 单位对象池管理器

*/

using System;

namespace WorldTree
{

    public static class UnitPoolManagerExtension
    {

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static T PoolGet<T>(this INode self)
        where T : class, IUnitPoolEventItem
        {
            return self.Core.GetUnit<T>();
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public static T PoolGet<T>(this INode self, out T unit)
       where T : class, IUnitPoolEventItem
        {
            return unit = self.Core.GetUnit<T>();
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public static void PoolRecycle(this INode self, IUnitPoolEventItem obj)
        {
            self.Core.Recycle(obj);
        }

    }

    /// <summary>
    /// 单位对象池管理器
    /// </summary>
    public class UnitPoolManager : Node
    {
        UnitDictionary<Type, UnitPool> pools = new UnitDictionary<Type, UnitPool>();
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
        /// 获取单位
        /// </summary>
        public T Get<T>()
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            return GetPool(type).Get() as T;
        }
        /// <summary>
        /// 获取单位
        /// </summary>
        public object Get(Type type)
        {
            return GetPool(type).Get();
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle(IUnitPoolEventItem obj)
        {
            if (obj != this && !(obj is UnitPool))//禁止回收自己和对象池
            {
                GetPool(obj.GetType()).Recycle(obj);
            }
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public UnitPool GetPool<T>()
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            return GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public UnitPool GetPool(Type type)
        {
            if (!pools.TryGetValue(type, out UnitPool pool))
            {
                pool = new UnitPool(type);
                pool.Id = Core.IdManager.GetId();
                pool.Core = Core;
                pool.Branch = Branch;
                pool.Type = pool.GetType();
                pools.Add(type, pool);
                this.AddChildren( pool);
            }

            return pool;
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool<T>()
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            if (pools.TryGetValue(type, out UnitPool pool))
            {
                pool.Dispose();
                pools.Remove(type);
            }
        }
    }
}
