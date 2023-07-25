
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 11:05

* 描述： 单位对象池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 单位对象池管理器
    /// </summary>
    public class UnitPoolManager : CoreNode, ComponentOf<WorldTreeCore>
        , AsRule<IAwakeRule>
    {
        public TreeDictionary<Type, UnitPool> m_Pools;

        public override void Dispose()
        {
            this.IsRecycle = true;
            this.IsDisposed = true;
            base.Dispose();
        }

    }

    public static partial class NodeUnitRule
    {
        /// <summary>
        /// 从池中获取单位对象
        /// </summary>
        public static T PoolGet<T>(this INode self)
        where T : class, IUnitPoolEventItem
        {
            return self.Core.GetUnit<T>();
        }

        /// <summary>
        /// 从池中获取单位对象
        /// </summary>
        public static T PoolGet<T>(this INode self, out T unit)
        where T : class, IUnitPoolEventItem
        {
            return unit = self.Core.GetUnit<T>();
        }
    }
    public static partial class UnitPoolManagerRule
    {

        class AddRule : AddRule<UnitPoolManager>
        {
            public override void OnEvent(UnitPoolManager self)
            {
                self.AddChild(out self.m_Pools);
            }
        }

        class RemoveRule : RemoveRule<UnitPoolManager>
        {
            public override void OnEvent(UnitPoolManager self)
            {
                self.m_Pools = null;
            }
        }


        /// <summary>
        /// 获取单位
        /// </summary>
        public static T Get<T>(this UnitPoolManager self)
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            return self.GetPool(type).Get() as T;
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        public static object Get(this UnitPoolManager self, Type type)
        {
            return self.GetPool(type).Get();
        }


        /// <summary>
        /// 尝试回收对象
        /// </summary>
        public static bool TryRecycle(this UnitPoolManager self, IUnitPoolEventItem obj)
        {
            if (self.m_Pools != null)
                if (self.m_Pools.TryGetValue(obj.GetType(), out UnitPool unitPool))
                {
                    unitPool.Recycle(obj);
                    return true;
                }
            return false;
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public static UnitPool GetPool<T>(this UnitPoolManager self)
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            return self.GetPool(type);
        }
        /// <summary>
        /// 获取池
        /// </summary>
        public static UnitPool GetPool(this UnitPoolManager self, Type type)
        {
            if (!self.m_Pools.TryGetValue(type, out UnitPool pool))
            {
                self.AddChild(out pool, type);
                self.m_Pools.Add(type, pool);
            }

            return pool;
        }
        /// <summary>
        /// 尝试获取池
        /// </summary>
        public static bool TryGetPool(this UnitPoolManager self, Type type, out UnitPool pool)
        {
            return self.m_Pools.TryGetValue(type, out pool);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public static void DisposePool<T>(this UnitPoolManager self)
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            self.DisposePool(type);
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public static void DisposePool(this UnitPoolManager self, Type type)
        {
            if (self.m_Pools.TryGetValue(type, out UnitPool pool))
            {
                self.m_Pools.Remove(type);
                pool.Dispose();
            }
        }
    }
}
