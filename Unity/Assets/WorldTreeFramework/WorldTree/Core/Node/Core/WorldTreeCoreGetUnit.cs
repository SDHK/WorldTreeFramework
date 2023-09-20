/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:45

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class WorldTreeCoreRule
    {
        /// <summary>
        /// 从池中获取单位对象
        /// </summary>
        public static T GetUnit<T>(this WorldTreeCore self)
        where T : class, IUnitPoolEventItem
        {
            if (self.IsActive)
            {
                if (self.UnitPoolManager.TryGet(TypeInfo<T>.HashCode64, out IUnitPoolEventItem unit))
                {
                    return unit as T;
                }
            }
            T obj = Activator.CreateInstance(TypeInfo<T>.Type, true) as T;
            obj.Type = TypeInfo<T>.HashCode64;
            obj.OnNew();
            obj.OnGet();
            return obj;
        }

        /// <summary>
        /// 回收单位
        /// </summary>
        public static void Recycle(this WorldTreeCore self, IUnitPoolEventItem obj)
        {
            if (self.IsActive && obj.IsFromPool)
            {
                if (self.UnitPoolManager.TryRecycle(obj)) return;
            }
            obj.IsRecycle = true;
            obj.OnRecycle();
            obj.IsDisposed = true;
            obj.OnDispose();
        }
    }
}
