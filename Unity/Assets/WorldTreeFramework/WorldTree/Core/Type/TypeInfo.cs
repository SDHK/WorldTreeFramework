
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 14:57

* 描述： 类型信息：用于类型明确时的信息获取

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 类型信息：用于类型明确时的信息获取
    /// </summary>
    public static class TypeInfo<T>
    {
        /// <summary>
        /// 类型
        /// </summary>
        public static readonly Type Type = typeof(T);
        /// <summary>
        /// 类型哈希码
        /// </summary>
        public static readonly int HashCode = typeof(T).GetHashCode();
        /// <summary>
        /// 类型FullName 的 RCR64码
        /// </summary>
        public static readonly long CRC64Code = typeof(T).FullName.GetCRC64();
        /// <summary>
        /// 类型名称
        /// </summary>
        public static readonly string TypeName = typeof(T).Name;
    }

}
