
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
    public class UnitPoolManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {
        public TreeDictionary<Type, UnitPool> m_Pools;

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
        /// 尝试回收对象
        /// </summary>
        public bool TryRecycle(IUnitPoolEventItem obj)
        {
            if (m_Pools != null)
                if (m_Pools.TryGetValue(obj.GetType(), out UnitPool unitPool))
                {
                    unitPool.Recycle(obj);
                    return true;
                }
            return false;
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
            if (!m_Pools.TryGetValue(type, out UnitPool pool))
            {
                this.AddChild(out pool,type);
                m_Pools.Add(type, pool);
            }

            return pool;
        }
        /// <summary>
        /// 尝试获取池
        /// </summary>
        public bool TryGetPool(Type type, out UnitPool pool)
        {
            return m_Pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool<T>()
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            DisposePool(type);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool(Type type)
        {
            if (m_Pools.TryGetValue(type, out UnitPool pool))
            {
                m_Pools.Remove(type);
                pool.Dispose();
            }
        }
    }

    class UnitPoolManagerAddRule : AddRule<UnitPoolManager>
    {
        public override void OnEvent(UnitPoolManager self)
        {
            self.AddChild(out self.m_Pools);
        }
    }
    class UnitPoolManagerRemoveRule : RemoveRule<UnitPoolManager>
    {
        public override void OnEvent(UnitPoolManager self)
        {
            self.m_Pools = null;
        }
    }
}
