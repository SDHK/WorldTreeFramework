
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 10:13

* 描述： 转换扩展

*/

using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace WorldTree
{
    public partial struct Vector3Float
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(Vector3Float vector3Float) => new Vector3(vector3Float.x, vector3Float.y, vector3Float.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Float(Vector3 vector3) => new Vector3Float(vector3.x, vector3.y, vector3.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator +(Vector3Float a, Vector3 b) => new Vector3Float(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator +(Vector3 a, Vector3Float b) => new Vector3Float(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3Float a, Vector3 b) => new Vector3Float(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3 a, Vector3Float b) => new Vector3Float(a.x - b.x, a.y - b.y, a.z - b.z);

        /// <summary>
        /// 极小误差相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Float lhs, Vector3 rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 < 9.999999439624929E-11;
        }

        /// <summary>
        /// 极小误差相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 lhs, Vector3Float rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 < 9.999999439624929E-11;
        }

        /// <summary>
        /// 极小误差不相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Float lhs, Vector3 rhs) => !(lhs == rhs);
        /// <summary>
        /// 极小误差不相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 lhs, Vector3Float rhs) => !(lhs == rhs);


    }

    public static partial class Vector3FloatRule
    {
        public static Vector3 ToVector3(this Vector3Float self) => self;
        public static Vector3Float ToVector3Float(this Vector3 self) => self;
    }
}
