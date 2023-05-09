/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 14:46

* 描述： 框架内置的浮点向量结构体

*/

using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

namespace WorldTree
{
    /// <summary>
    /// 浮点二维向量
    /// </summary>
    public partial struct Vector2Float : IEquatable<Vector2Float>
    {
        public float x;
        public float y;


        private static readonly Vector2Float zeroVector = new(0.0f, 0.0f);
        private static readonly Vector2Float oneVector = new(1f, 1f);
        private static readonly Vector2Float upVector = new(0.0f, 1f);
        private static readonly Vector2Float downVector = new(0.0f, -1f);
        private static readonly Vector2Float leftVector = new(-1f, 0.0f);
        private static readonly Vector2Float rightVector = new(1f, 0.0f);
        private static readonly Vector2Float positiveInfinityVector = new(float.PositiveInfinity, float.PositiveInfinity);
        private static readonly Vector2Float negativeInfinityVector = new(float.NegativeInfinity, float.NegativeInfinity);


        /// <summary>
        /// (0, 0)
        /// </summary>
        public static Vector2Float Zero => zeroVector;
        /// <summary>
        /// (1, 1)
        /// </summary>
        public static Vector2Float One => oneVector;
        /// <summary>
        /// (0, 1)
        /// </summary>
        public static Vector2Float Up => upVector;
        /// <summary>
        /// (0, -1)
        /// </summary>
        public static Vector2Float Down => downVector;
        /// <summary>
        /// (-1, 0)
        /// </summary>
        public static Vector2Float Left => leftVector;
        /// <summary>
        /// (1, 0)
        /// </summary>
        public static Vector2Float Right => rightVector;
        /// <summary>
        /// (float.PositiveInfinity)
        /// </summary>
        public static Vector2Float PositiveInfinity => positiveInfinityVector;
        /// <summary>
        /// (float.NegativeInfinity)
        /// </summary>
        public static Vector2Float NegativeInfinity => negativeInfinityVector;


        public Vector2Float(float value2)
        {
            this.x = value2;
            this.y = value2;
        }
        public Vector2Float(float x, float y)
        {
            this.x = x;
            this.y = y;
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2Float index!");
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2Float index!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator +(Vector2Float a, Vector2Float b) => new(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator -(Vector2Float a, Vector2Float b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(Vector2Float a, Vector2Float b) => new(a.x * b.x, a.y * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator /(Vector2Float a, Vector2Float b) => new(a.x / b.x, a.y / b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator -(Vector2Float a) => new(-a.x, -a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(Vector2Float a, float d) => new(a.x * d, a.y * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(float d, Vector2Float a) => new(a.x * d, a.y * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator /(Vector2Float a, float d) => new(a.x / d, a.y / d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2Float lhs, Vector2Float rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 < 9.999999439624929E-11;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2Float lhs, Vector2Float rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Float(Vector3Float v) => new(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Float(Vector2Float v) => new(v.x, v.y, 0.0f);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode() << 2;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        public override bool Equals(object other) => other is Vector2Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2Float other) => (double)this.x == (double)other.x && (double)this.y == (double)other.y;


        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.x * (double)this.x + (double)this.y * (double)this.y);
        }
        /// <summary>
        /// 返回该向量的平方长度(只读)。
        /// </summary>
        public float sqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y);
        }

        /// <summary>
        /// 返回大小为1的向量(只读)。
        /// </summary>
        public Vector2Float normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float magnitude = this.magnitude;
                return (double)magnitude > 9.999999747378752E-06 ? this / magnitude : Zero;
            }
        }

    }
}
