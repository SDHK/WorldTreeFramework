/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 11:45

* 描述： 框架内置的浮点向量结构体

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 浮点三维向量
    /// </summary>
    public struct Vector3Float : IEquatable<Vector3Float>
    {
        public const float kEpsilon = 1E-05f;//?
        public const float kEpsilonNormalSqrt = 1E-15f;//?

        public float x;
        public float y;
        public float z;

        private static readonly Vector3Float zeroVector = new Vector3Float(0.0f, 0.0f, 0.0f);
        private static readonly Vector3Float oneVector = new Vector3Float(1f, 1f, 1f);
        private static readonly Vector3Float upVector = new Vector3Float(0.0f, 1f, 0.0f);
        private static readonly Vector3Float downVector = new Vector3Float(0.0f, -1f, 0.0f);
        private static readonly Vector3Float leftVector = new Vector3Float(-1f, 0.0f, 0.0f);
        private static readonly Vector3Float rightVector = new Vector3Float(1f, 0.0f, 0.0f);
        private static readonly Vector3Float forwardVector = new Vector3Float(0.0f, 0.0f, 1f);
        private static readonly Vector3Float backVector = new Vector3Float(0.0f, 0.0f, -1f);
        private static readonly Vector3Float positiveInfinityVector = new Vector3Float(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        private static readonly Vector3Float negativeInfinityVector = new Vector3Float(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        /// <summary>
        /// (0, 0, 0)
        /// </summary>
        public static Vector3Float Zero => zeroVector;
        /// <summary>
        /// (1, 1, 1)
        /// </summary>
        public static Vector3Float One => oneVector;
        /// <summary>
        /// (0, 0, 1)
        /// </summary>
        public static Vector3Float Forward => forwardVector;
        /// <summary>
        /// (0, 0, -1)
        /// </summary>
        public static Vector3Float Back => backVector;
        /// <summary>
        /// (0, 1, 0)
        /// </summary>
        public static Vector3Float Up => upVector;
        /// <summary>
        /// (0, -1, 0)
        /// </summary>
        public static Vector3Float Down => downVector;
        /// <summary>
        /// (-1, 0, 0)
        /// </summary>
        public static Vector3Float Left => leftVector;
        /// <summary>
        /// (1, 0, 0)
        /// </summary>
        public static Vector3Float Right => rightVector;
        /// <summary>
        /// (float.PositiveInfinity)
        /// </summary>

        public static Vector3Float PositiveInfinity => positiveInfinityVector;
        /// <summary>
        /// (float.NegativeInfinity)
        /// </summary>
        public static Vector3Float NegativeInfinity => negativeInfinityVector;


        public Vector3Float(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
        }
        public Vector3Float(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }
        public Vector3Float(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException("无效的 Vector3Float index!");
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("无效的 Vector3Float index!");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator +(Vector3Float a, Vector3Float b) => new Vector3Float(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3Float a, Vector3Float b) => new Vector3Float(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3Float a) => new Vector3Float(-a.x, -a.y, -a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator *(Vector3Float a, float d) => new Vector3Float(a.x * d, a.y * d, a.z * d);
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator *(float d, Vector3Float a) => new Vector3Float(a.x * d, a.y * d, a.z * d);
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator /(Vector3Float a, float d) => new Vector3Float(a.x / d, a.y / d, a.z / d);

        /// <summary>
        /// 极小误差相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Float lhs, Vector3Float rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 < 9.999999439624929E-11;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Float lhs, Vector3Float rhs) => !(lhs == rhs);


        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }


        [MethodImpl((MethodImplOptions)256)]
        public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is Vector3Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3Float other)
        {
           return (double)this.x == (double)other.x && (double)this.y == (double)other.y && (double)this.z == (double)other.z;
        }
    }
}
