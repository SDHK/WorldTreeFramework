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
    public partial struct Vector3Float : IEquatable<Vector3Float>
    {
		/// <summary>
		/// ε
		/// </summary>
		public const float K_EPSILON = 1E-05f;//?
		/// <summary>
		/// ε
		/// </summary>
		public const float K_EPSILON_NORMAL_SQRT = 1E-15f;//?

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
		/// (0, 0, 0)
		/// </summary>
		private static readonly Vector3Float zeroVector = new Vector3Float(0.0f, 0.0f, 0.0f);
		/// <summary>
		/// (1, 1, 1)
		/// </summary>
		private static readonly Vector3Float oneVector = new Vector3Float(1f, 1f, 1f);
		/// <summary>
		/// (0, 1, 0)
		/// </summary>
		private static readonly Vector3Float upVector = new Vector3Float(0.0f, 1f, 0.0f);
		/// <summary>
		/// (0, -1, 0)
		/// </summary>
		private static readonly Vector3Float downVector = new Vector3Float(0.0f, -1f, 0.0f);
		/// <summary>
		/// (-1, 0, 0)
		/// </summary>
		private static readonly Vector3Float leftVector = new Vector3Float(-1f, 0.0f, 0.0f);
		/// <summary>
		/// (1, 0, 0)
		/// </summary>
		private static readonly Vector3Float rightVector = new Vector3Float(1f, 0.0f, 0.0f);
		/// <summary>
		/// (0, 0, 1)
		/// </summary>
		private static readonly Vector3Float forwardVector = new Vector3Float(0.0f, 0.0f, 1f);
		/// <summary>
		/// (0, 0, -1)
		/// </summary>
		private static readonly Vector3Float backVector = new Vector3Float(0.0f, 0.0f, -1f);
		/// <summary>
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		private static readonly Vector3Float positiveInfinityVector = new Vector3Float(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
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
		/// (float.PositiveInfinity),是一个特殊的向量，表示无穷大
		/// </summary>
		public static Vector3Float PositiveInfinity => positiveInfinityVector;
		/// <summary>
		/// (float.NegativeInfinity),是一个特殊的向量，表示无穷小
		/// </summary>
		public static Vector3Float NegativeInfinity => negativeInfinityVector;


        public Vector3Float(float value3)
        {
            this.X = value3;
            this.Y = value3;
            this.Z = value3;
        }
        public Vector3Float(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0.0f;
        }
        public Vector3Float(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3Float index!");
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
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3Float index!");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator +(Vector3Float a, Vector3Float b) => new Vector3Float(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3Float a, Vector3Float b) => new Vector3Float(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator -(Vector3Float a) => new Vector3Float(-a.X, -a.Y, -a.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator *(Vector3Float a, float d) => new Vector3Float(a.X * d, a.Y * d, a.Z * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator *(float d, Vector3Float a) => new Vector3Float(a.X * d, a.Y * d, a.Z * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float operator /(Vector3Float a, float d) => new Vector3Float(a.X / d, a.Y / d, a.Z / d);

        /// <summary>
        /// 极小误差相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Float lhs, Vector3Float rhs)
        {
            float num1 = lhs.X - rhs.X;
            float num2 = lhs.Y - rhs.Y;
            float num3 = lhs.Z - rhs.Z;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 < 9.999999439624929E-11;
        }
        /// <summary>
        /// 极小误差不相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Float lhs, Vector3Float rhs) => !(lhs == rhs);


        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is Vector3Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3Float other) => (double)this.X == (double)other.X && (double)this.Y == (double)other.Y && (double)this.Z == (double)other.Z;



        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z);
        }
        /// <summary>
        /// 返回该向量的平方长度(只读)。
        /// </summary>
        public float SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z);
        }

        /// <summary>
        /// 返回大小为1的向量(只读)。
        /// </summary>
        public Vector3Float Normalized
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
        /// 使这个向量的大小为1。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float num = this.Magnitude;
            if ((double)num > 9.999999747378752E-06)
                this = this / num;
            else
                this = Vector3Float.Zero;
        }

        /// <summary>
        /// 设置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(float newX, float newY, float newZ)
        {
            this.X = newX;
            this.Y = newY;
            this.Z = newZ;
        }

        /// <summary>
        /// 轴 乘等于
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void Scale( Vector3Float scale)
        {
            this.X *= scale.X;
            this.Y *= scale.Y;
            this.Z *= scale.Z;
        }

        /// <summary>
        /// 2d向量转3d向量 对应到XZ
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  Vector3Float ToVector2FloatXZ() => new Vector3Float(this.X, 0, this.Y);
     
        #endregion
    }


}
