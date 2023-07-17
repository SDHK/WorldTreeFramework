
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:15

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class ArrayPoolManagerRule
    {

        /// <summary>
        /// 获取数组
        /// </summary>
        public static T[] Get<T>(this ArrayPoolManager self, int Length)
        {
            return self.GetGroup(typeof(T)).GetPool(Length).Get() as T[];
        }


        /// <summary>
        /// 获取数组
        /// </summary>
        public static Array Get(this ArrayPoolManager self, Type type, int Length)
        {
            return self.GetGroup(type).GetPool(Length).Get();
        }

        /// <summary>
        /// 回收数组
        /// </summary>
        public static void Recycle(this ArrayPoolManager self, Array obj)
        {
            if (self.TryGetGroup(obj.GetType().GetElementType(), out ArrayPoolGroup arrayPoolGroup))
            {
                if (arrayPoolGroup.TryGetPool(obj.Length, out ArrayPool arrayPool))
                {
                    arrayPool.Recycle(obj);
                }
            }
        }

        /// <summary>
        /// 获取对象池集合
        /// </summary>
        public static ArrayPoolGroup GetGroup(this ArrayPoolManager self, Type type)
        {
            if (!self.PoolGroups.TryGetValue(type, out ArrayPoolGroup arrayPoolGroup))
            {
                self.AddChild(out arrayPoolGroup, type);
                self.PoolGroups.Add(type, arrayPoolGroup);
            }
            return arrayPoolGroup;
        }

        /// <summary>
        /// 尝试获取对象池集合
        /// </summary>
        public static bool TryGetGroup(this ArrayPoolManager self, Type type, out ArrayPoolGroup arrayPoolGroup)
        {
            return self.PoolGroups.TryGetValue(type, out arrayPoolGroup);
        }

        /// <summary>
        /// 释放对象池集合
        /// </summary>
        public static void DisposeGroup(this ArrayPoolManager self, Type type)
        {
            if (!self.PoolGroups.TryGetValue(type, out ArrayPoolGroup arrayPoolGroup))
            {
                self.PoolGroups.Remove(type);
                arrayPoolGroup.Dispose();
            }
        }
    }
}
