/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/8 10:33

* 描述： 

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    public static class MathVector4Float
    {
        /// <summary>
        /// 返回最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Min(this Vector4Float lhs, Vector4Float rhs) => new Vector4Float(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));

        /// <summary>
        /// 返回最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Max(this Vector4Float lhs, Vector4Float rhs) => new Vector4Float(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));

        /// <summary>
        /// 返回a和b之间的距离。</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector4Float a, Vector4Float b) => (a - b).magnitude;

        /// <summary>
        /// 两个向量的点积。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector4Float a, Vector4Float b) => (float)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z + (double)a.w * (double)b.w);

        /// <summary>
        /// 将一个向量投影到另一个向量上。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Project(this Vector4Float a, Vector4Float b) => b * (a.Dot(b) / b.Dot(b));
    }

}
