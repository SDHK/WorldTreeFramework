/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 14:46

* 描述： 框架内置的浮点向量结构体

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 浮点二维向量
    /// </summary>
    public partial struct Vector2Float : IEquatable<Vector2Float>
    {
		/// <summary>
		/// X轴坐标
		/// </summary>
		public float X;
		/// <summary>
		/// Y轴坐标
		/// </summary>
		public float Y;

		/// <summary>
		/// (0, 0)
		/// </summary>
		private static readonly Vector2Float zeroVector = new(0.0f, 0.0f);
		/// <summary>
		/// (1, 1)
		/// </summary>
		private static readonly Vector2Float oneVector = new(1f, 1f);
		/// <summary>
		/// (0, 1)
		/// </summary>
		private static readonly Vector2Float upVector = new(0.0f, 1f);
		/// <summary>
		/// (0, -1)
		/// </summary>
		private static readonly Vector2Float downVector = new(0.0f, -1f);
		/// <summary>
		/// (-1, 0)
		/// </summary>
		private static readonly Vector2Float leftVector = new(-1f, 0.0f);
		/// <summary>
		/// (1, 0)
		/// </summary>
		private static readonly Vector2Float rightVector = new(1f, 0.0f);
		/// <summary>
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		private static readonly Vector2Float positiveInfinityVector = new(float.PositiveInfinity, float.PositiveInfinity);
		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
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
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		public static Vector2Float PositiveInfinity => positiveInfinityVector;
		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
		public static Vector2Float NegativeInfinity => negativeInfinityVector;


        public Vector2Float(float value2)
        {
            this.X = value2;
            this.Y = value2;
        }
        public Vector2Float(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
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
                        this.X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2Float index!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator +(Vector2Float a, Vector2Float b) => new(a.X + b.X, a.Y + b.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator -(Vector2Float a, Vector2Float b) => new(a.X - b.X, a.Y - b.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(Vector2Float a, Vector2Float b) => new(a.X * b.X, a.Y * b.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator /(Vector2Float a, Vector2Float b) => new(a.X / b.X, a.Y / b.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator -(Vector2Float a) => new(-a.X, -a.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(Vector2Float a, float d) => new(a.X * d, a.Y * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator *(float d, Vector2Float a) => new(a.X * d, a.Y * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Float operator /(Vector2Float a, float d) => new(a.X / d, a.Y / d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2Float lhs, Vector2Float rhs)
        {
            float num1 = lhs.X - rhs.X;
            float num2 = lhs.Y - rhs.Y;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 < 9.999999439624929E-11;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2Float lhs, Vector2Float rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Float(Vector3Float v) => new(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Float(Vector2Float v) => new(v.X, v.Y, 0.0f);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() << 2;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        public override bool Equals(object other) => other is Vector2Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2Float other) => (double)this.X == (double)other.X && (double)this.Y == (double)other.Y;


        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y);
        }
        /// <summary>
        /// 返回该向量的平方长度(只读)。
        /// </summary>
        public float SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y);
        }

        /// <summary>
        /// 返回大小为1的向量(只读)。
        /// </summary>
        public Vector2Float Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float magnitude = this.Magnitude;
                return (double)magnitude > 9.999999747378752E-06 ? this / magnitude : Zero;
            }
        }

    }
}
