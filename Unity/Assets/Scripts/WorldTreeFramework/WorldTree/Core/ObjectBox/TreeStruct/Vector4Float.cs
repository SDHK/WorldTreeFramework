/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:56

* 描述： 

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 浮点四维向量
    /// </summary>
    public partial struct Vector4Float : IEquatable<Vector4Float>
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
		/// Z轴坐标
		/// </summary>
		public float Z;
		/// <summary>
		/// W轴坐标
		/// </summary>
		public float W;

		/// <summary>
		/// (0, 0, 0, 0)
		/// </summary>
		private static readonly Vector4Float zeroVector = new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f);
		/// <summary>
		/// (1, 1, 1, 1)
		/// </summary>
		private static readonly Vector4Float oneVector = new Vector4Float(1f, 1f, 1f, 1f);
		/// <summary>
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		private static readonly Vector4Float positiveInfinityVector = new Vector4Float(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
		private static readonly Vector4Float negativeInfinityVector = new Vector4Float(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

		/// <summary>
		/// (0, 0, 0, 0)
		/// </summary>
		public static Vector4Float Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.zeroVector;
        }

		/// <summary>
		/// (1, 1, 1, 1)
		/// </summary>
		public static Vector4Float One
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.oneVector;
        }

		/// <summary>
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		public static Vector4Float PositiveInfinity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.positiveInfinityVector;
        }

		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
		public static Vector4Float NegativeInfinity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.negativeInfinityVector;
        }

        /// <summary>
        /// 创建一个具有给定x, y, z, w分量的新向量。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// 创建一个具有给定x, y, z分量的新向量，并将w设置为零。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 0.0f;
        }

        /// <summary>
        /// 创建一个具有给定x, y分量的新向量，并将z和w设置为零。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0.0f;
            this.W = 0.0f;
        }

        /// <summary>
        /// 创建一个 x, y, z ,w 为 value4 的新向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float value4)
        {
            this.X = value4;
            this.Y = value4;
            this.Z = value4;
            this.W = value4;
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
                    case 2:
                        return this.Z;
                    case 3:
                        return this.W;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4Float index!");
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
                    case 2:
                        this.Z = value;
                        break;
                    case 3:
                        this.W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4Float index!");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator +(Vector4Float a, Vector4Float b) => new Vector4Float(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator -(Vector4Float a, Vector4Float b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(Vector4Float a, Vector4Float b) => new Vector4Float(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator /(Vector4Float a, Vector4Float b) => new Vector4Float(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator -(Vector4Float a) => new Vector4Float(-a.X, -a.Y, -a.Z, -a.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(Vector4Float a, float d) => new Vector4Float(a.X * d, a.Y * d, a.Z * d, a.W * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(float d, Vector4Float a) => new Vector4Float(a.X * d, a.Y * d, a.Z * d, a.W * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator /(Vector4Float a, float d) => new Vector4Float(a.X / d, a.Y / d, a.Z / d, a.W / d);

        /// <summary>
        /// 极小误差相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector4Float lhs, Vector4Float rhs)
        {
            float num1 = lhs.X - rhs.X;
            float num2 = lhs.Y - rhs.Y;
            float num3 = lhs.Z - rhs.Z;
            float num4 = lhs.W - rhs.W;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 + (double)num4 * (double)num4 < 9.999999439624929E-11;
        }

        /// <summary>
        /// 极小误差不相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector4Float lhs, Vector4Float rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4Float(Vector3Float v) => new(v.X, v.Y, v.Z, 0.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Float(Vector4Float v) => new Vector3Float(v.X, v.Y, v.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4Float(Vector2Float v) => new(v.X, v.Y, 0.0f, 0.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Float(Vector4Float v) => new Vector2Float(v.X, v.Y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2 ^ this.W.GetHashCode() >> 1;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is Vector4Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4Float other) => (double)this.X == (double)other.X && (double)this.Y == (double)other.Y && (double)this.Z == (double)other.Z && (double)this.W == (double)other.W;


        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z + (double)this.W * (double)this.W);
        }

        /// <summary>
        /// 返回该向量的平方长度(只读)。
        /// </summary>
        public float SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z + (double)this.W * (double)this.W);
        }

        /// <summary>
        /// 返回大小为1的向量(只读)。
        /// </summary>
        public Vector4Float Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float num = this.Magnitude;
                return (double)num > 9.999999747378752E-06 ? this / num : Zero;
            }
        }


        #region 方法

        /// <summary>
        /// 设置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(float newX, float newY, float newZ, float newW)
        {
            this.X = newX;
            this.Y = newY;
            this.Z = newZ;
            this.W = newW;
        }

        /// <summary>
        /// 轴 乘等于
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector4Float scale)
        {
            this.X *= scale.X;
            this.Y *= scale.Y;
            this.Z *= scale.Z;
            this.W *= scale.W;
        }

        #endregion
    }


}
