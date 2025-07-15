/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 浮点四维向量
	/// </summary>
	public static class MathVector4Float
    {
        /// <summary>
        /// 返回最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Min(this Vector4Float lhs, Vector4Float rhs) => new Vector4Float(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y), Math.Min(lhs.Z, rhs.Z), Math.Min(lhs.W, rhs.W));

        /// <summary>
        /// 返回最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Max(this Vector4Float lhs, Vector4Float rhs) => new Vector4Float(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y), Math.Max(lhs.Z, rhs.Z), Math.Max(lhs.W, rhs.W));

        /// <summary>
        /// 返回a和b之间的距离。</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector4Float a, Vector4Float b) => (a - b).Magnitude;

        /// <summary>
        /// 两个向量的点积。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector4Float a, Vector4Float b) => (float)((double)a.X * (double)b.X + (double)a.Y * (double)b.Y + (double)a.Z * (double)b.Z + (double)a.W * (double)b.W);

        /// <summary>
        /// 将一个向量投影到另一个向量上。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float Project(this Vector4Float a, Vector4Float b) => b * (a.Dot(b) / b.Dot(b));
    }

}
