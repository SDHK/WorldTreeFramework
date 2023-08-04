
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 11:05

* 描述： 单位对象池管理器

*/

using System;

namespace WorldTree
{
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


    /// <summary>
    /// 单位对象池管理器
    /// </summary>
    public class UnitPoolManager : PoolManagerBase<UnitPool>, ComponentOf<WorldTreeCore>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 尝试获取单位
        /// </summary>
        public bool TryGet(Type type, out IUnitPoolEventItem unit)
        {
            if (TryGet(type, out object obj))
            {
                unit = obj as IUnitPoolEventItem;
                return true;
            }
            else
            {
                unit = null;
                return false;
            }
        }
    }
}
