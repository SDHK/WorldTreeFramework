
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/12 19:00

* 描述： 数组对象池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 数组对象池管理器
    /// </summary>
    public class ArrayPoolManager : Node, IAwake, ComponentOf<WorldTreeCore>
    {
        public TreeDictionary<Type, ArrayPoolGroup> PoolGroups;
    }

    class ArrayPoolManagerAddRule : AddRule<ArrayPoolManager>
    {
        public override void OnEvent(ArrayPoolManager self)
        {
            self.AddChild(out self.PoolGroups);
        }
    }

    class ArrayPoolManagerRemoveRule : RemoveRule<ArrayPoolManager>
    {
        public override void OnEvent(ArrayPoolManager self)
        {
            self.PoolGroups = null;
        }
    }

    public static class ArrayPoolManagerRule
    {

        /// <summary>
        /// 获取数组
        /// </summary>
        public static T[] Get<T>(this ArrayPoolManager self,  int Length)
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
