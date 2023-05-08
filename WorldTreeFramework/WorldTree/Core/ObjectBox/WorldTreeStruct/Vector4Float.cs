/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:56

* 描述： 

*/

using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

namespace WorldTree
{
    /// <summary>
    /// 浮点四维向量
    /// </summary>
    public partial struct Vector4Float : IEquatable<Vector4Float>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        private static readonly Vector4Float zeroVector = new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f);
        private static readonly Vector4Float oneVector = new Vector4Float(1f, 1f, 1f, 1f);
        private static readonly Vector4Float positiveInfinityVector = new Vector4Float(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        private static readonly Vector4Float negativeInfinityVector = new Vector4Float(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public static Vector4Float Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.zeroVector;
        }

        public static Vector4Float One
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.oneVector;
        }

        public static Vector4Float PositiveInfinity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector4Float.positiveInfinityVector;
        }

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
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// 创建一个具有给定x, y, z分量的新向量，并将w设置为零。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0.0f;
        }

        /// <summary>
        /// 创建一个具有给定x, y分量的新向量，并将z和w设置为零。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
            this.w = 0.0f;
        }

        /// <summary>
        /// 创建一个 x, y, z ,w 为 value4 的新向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4Float(float value4)
        {
            this.x = value4;
            this.y = value4;
            this.z = value4;
            this.w = value4;
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
                    case 3:
                        return this.w;
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
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4Float index!");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator +(Vector4Float a, Vector4Float b) => new Vector4Float(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator -(Vector4Float a, Vector4Float b) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(Vector4Float a, Vector4Float b) => new Vector4Float(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator /(Vector4Float a, Vector4Float b) => new Vector4Float(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator -(Vector4Float a) => new Vector4Float(-a.x, -a.y, -a.z, -a.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(Vector4Float a, float d) => new Vector4Float(a.x * d, a.y * d, a.z * d, a.w * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator *(float d, Vector4Float a) => new Vector4Float(a.x * d, a.y * d, a.z * d, a.w * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4Float operator /(Vector4Float a, float d) => new Vector4Float(a.x / d, a.y / d, a.z / d, a.w / d);

        /// <summary>
        /// 极小误差相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector4Float lhs, Vector4Float rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            float num4 = lhs.w - rhs.w;
            return (double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3 + (double)num4 * (double)num4 < 9.999999439624929E-11;
        }

        /// <summary>
        /// 极小误差不相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector4Float lhs, Vector4Float rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4Float(Vector3Float v) => new(v.x, v.y, v.z, 0.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Float(Vector4Float v) => new Vector3Float(v.x, v.y, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector4Float(Vector2Float v) => new(v.x, v.y, 0.0f, 0.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Float(Vector4Float v) => new Vector2Float(v.x, v.y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is Vector4Float other1 && this.Equals(other1);

        /// <summary>
        /// 完全相等判断
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4Float other) => (double)this.x == (double)other.x && (double)this.y == (double)other.y && (double)this.z == (double)other.z && (double)this.w == (double)other.w;


        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z + (double)this.w * (double)this.w);
        }

        /// <summary>
        /// 返回该向量的平方长度(只读)。
        /// </summary>
        public float sqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z + (double)this.w * (double)this.w);
        }

        /// <summary>
        /// 返回大小为1的向量(只读)。
        /// </summary>
        public Vector4Float normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float num = this.magnitude;
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
            this.x = newX;
            this.y = newY;
            this.z = newZ;
            this.w = newW;
        }

        /// <summary>
        /// 轴 乘等于
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector4Float scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        #endregion
    }


}
