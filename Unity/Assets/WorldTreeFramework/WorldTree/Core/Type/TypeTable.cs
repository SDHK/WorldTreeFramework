
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 14:58

* 描述： 类型信息表，用于不明确时类型的信息查表

*/

using System;
using System.Collections.Concurrent;

namespace WorldTree
{
    /// <summary>
    /// 类型信息表，用于不明确时类型的信息查表
    /// </summary>
    public static class TypeTable
    {
        private static readonly ConcurrentDictionary<Type, int> TypeHashCode = new();
        private static readonly ConcurrentDictionary<int, Type> HashCodeType = new();


        private static readonly ConcurrentDictionary<Type, long> TypeHash64 = new();
        private static readonly ConcurrentDictionary<long, Type> Hash64Type = new();

        /// <summary>
        /// 类型注册到信息表
        /// </summary>
        public static Type Add(this Type type)
        {
            if (!TypeHash64.ContainsKey(type))
            {
                long hash64 = type.FullName.GetHash64();
                TypeHash64.GetOrAdd(type, hash64);
                Hash64Type.GetOrAdd(hash64, type);

                int hash = type.GetHashCode();
                TypeHashCode.GetOrAdd(type, hash);
                HashCodeType.GetOrAdd(hash, type);
            }
            return type;
        }

        /// <summary>
        /// 类型获取64位哈希码
        /// </summary>
        public static long GetTableHash64(this Type type)
        {
            if (!TypeHash64.TryGetValue(type, out long hash64))
            {
                TypeHash64.GetOrAdd(type, hash64 = type.FullName.GetHash64());
                Hash64Type.GetOrAdd(hash64, type);
            }
            return hash64;
        }

        /// <summary>
        /// 类型获取哈希码
        /// </summary>
        public static int GetTableHashCode(this Type type)
        {
            if (!TypeHashCode.TryGetValue(type, out int hash))
            {
                TypeHashCode.GetOrAdd(type, hash = type.GetHashCode());
                HashCodeType.GetOrAdd(hash, type);
            }
            return hash;
        }

        /// <summary>
        /// 哈希码64码获取类型
        /// </summary>
        public static bool TryGetTypeHash64(this long rcr, out Type type) => Hash64Type.TryGetValue(rcr, out type);
        /// <summary>
        /// 哈希码获取类型
        /// </summary>
        public static bool TryGetTypeHashCore(this int hash, out Type type) => HashCodeType.TryGetValue(hash, out type);
    }

}
