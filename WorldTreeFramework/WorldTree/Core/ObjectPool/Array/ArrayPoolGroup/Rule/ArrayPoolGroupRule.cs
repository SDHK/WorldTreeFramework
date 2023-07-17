/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 12:13

* 描述： 

*/

namespace WorldTree
{
    public static partial class ArrayPoolGroupRule
    {
        /// <summary>
        /// 获取数组对象池
        /// </summary>
        public static ArrayPool GetPool(this ArrayPoolGroup self, int Length)
        {
            if (!self.Pools.TryGetValue(Length, out ArrayPool arrayPool))
            {
                self.AddChild(out arrayPool, self.ArrayType, Length);
                self.Pools.Add(Length, arrayPool);
            }
            return arrayPool;
        }

        /// <summary>
        /// 尝试获取数组对象池
        /// </summary>
        public static bool TryGetPool(this ArrayPoolGroup self, int Length, out ArrayPool arrayPool)
        {
            return self.Pools.TryGetValue(Length, out arrayPool);
        }

        /// <summary>
        /// 释放数组对象池
        /// </summary>
        public static void DisposePool(this ArrayPoolGroup self, int Length)
        {
            if (self.Pools.TryGetValue(Length, out ArrayPool arrayPool))
            {
                self.Pools.Remove(Length);
                arrayPool.Dispose();
            }
        }
    }
}
