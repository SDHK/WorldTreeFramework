/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 10:37

* 描述： 数组对象池集合

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 数组对象池集合
    /// </summary>
    public class ArrayPoolGroup : Node, IAwake<Type>, ChildOf<ArrayPoolManager>
    {
        public Type objectType;
        public TreeDictionary<int, ArrayPool> Pools = new TreeDictionary<int, ArrayPool>();
    }

    class ArrayPoolGroupAwakeRule : AwakeRule<ArrayPoolGroup, Type>
    {
        public override void OnEvent(ArrayPoolGroup self, Type type)
        {
            self.objectType = type;
        }
    }


    class ArrayPoolGroupAddRule : AddRule<ArrayPoolGroup>
    {
        public override void OnEvent(ArrayPoolGroup self)
        {
            self.AddChild(out self.Pools);
        }
    }

    class ArrayPoolGroupRemoveRule : RemoveRule<ArrayPoolGroup>
    {
        public override void OnEvent(ArrayPoolGroup self)
        {
            self.Pools = null;
        }
    }


    public static class ArrayPoolGroupRule
    {
        /// <summary>
        /// 获取数组对象池
        /// </summary>
        public static ArrayPool GetPool(this ArrayPoolGroup self, int Length)
        {
            if (!self.Pools.TryGetValue(Length, out ArrayPool arrayPool))
            {
                self.AddChild(out arrayPool, self.objectType, Length);
                self.Pools.Add(Length, arrayPool);
            }
            return arrayPool;
        }

        /// <summary>
        /// 尝试获取数组对象池
        /// </summary>
        public static bool TryGetPool(this ArrayPoolGroup self, int Length,out ArrayPool arrayPool)
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
