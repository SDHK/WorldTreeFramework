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
                if (self.UnitPoolManager.TryGet(TypeInfo<T>.TypeCode, out IUnitPoolEventItem unit))
                {
                    return unit as T;
                }
            }
            T unitObj = Activator.CreateInstance(TypeInfo<T>.Type, true) as T;
            unitObj.Type = TypeInfo<T>.TypeCode;
			unitObj.Core = self;
			unitObj.OnNew();
            unitObj.OnGet();
            return unitObj;
        }

		/// <summary>
		/// 从池中获取单位对象
		/// </summary>
		public static IUnitPoolEventItem GetUnit(this WorldTreeCore self, long type)
        {
			if (self.IsActive)
			{
				if (self.UnitPoolManager.TryGet(type, out IUnitPoolEventItem unit))
				{
					return unit;
				}
			}
			IUnitPoolEventItem unitObj = Activator.CreateInstance(type.HashCore64ToType(), true) as IUnitPoolEventItem;
			unitObj.Type = type;
			unitObj.Core = self;
			unitObj.OnNew();
			unitObj.OnGet();
			return unitObj;
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
