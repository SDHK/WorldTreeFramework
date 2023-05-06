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


        public Vector3Float(float value3)
        {
            this.x = value3;
            this.y = value3;
            this.z = value3;
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
        /// <summary>
        /// 极小误差不相等
        /// </summary>
        /// <remarks>这种比较方法是为了解决浮点数精度问题。由于浮点数的精度有限，直接比较两个浮点数是否相等可能会出现问题。</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Float lhs, Vector3Float rhs) => !(lhs == rhs);


        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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



        /// <summary>
        /// 返回该向量的长度(只读)。
        /// </summary>
        public float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)Math.Sqrt((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z);
        }

        /// <summary>
        /// 使这个向量的大小为1</para>
        /// </summary>
        public Vector3Float normalized
        {
            get
            {
                float num = this.magnitude;
                return (double)num > 9.999999747378752E-06 ? this / num : Zero;
            }
        }
        /// <summary>
        /// 返回该向量的平方长度(只读)。</para>
        /// </summary>
        public float sqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z);
        }
    }

    public static class Vector3FloatRule
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(this Vector3Float self, float newX, float newY, float newZ)
        {
            self.x = newX;
            self.y = newY;
            self.z = newZ;
        }





        /// <summary>
        /// 轴 乘等于
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(this Vector3Float self, Vector3Float scale)
        {
            self.x *= scale.x;
            self.y *= scale.y;
            self.z *= scale.z;
        }

        /// <summary>
        /// 返回最小值
        /// </summary>
        public static Vector3Float Min(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));

        /// <summary>
        /// 返回最大值
        /// </summary>
        public static Vector3Float Max(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));


        /// <summary>
        /// 返回a和b之间的距离。</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector3Float a, Vector3Float b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        }

        /// <summary>
        /// 返回vector的副本，其大小限制为maxLength。</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        [MethodImpl((MethodImplOptions)256)]
        public static Vector3Float ClampMagnitude(this Vector3Float vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;
            if ((double)sqrMagnitude <= (double)maxLength * (double)maxLength)
                return vector;
            float num1 = (float)Math.Sqrt((double)sqrMagnitude);
            float num2 = vector.x / num1;
            float num3 = vector.y / num1;
            float num4 = vector.z / num1;
            return new Vector3Float(num2 * maxLength, num3 * maxLength, num4 * maxLength);
        }

        /// <summary>
        /// 将向量反射出由法线定义的平面。
        /// </summary>
        /// <param name="inDirection">入射向量</param>
        /// <param name="inNormal">平面法线向量</param>
        /// <returns>反射向量</returns>
        public static Vector3Float Reflect(this Vector3Float inDirection, Vector3Float inNormal)
        {
            float num = -2f * inNormal.Dot(inDirection);
            return new Vector3Float(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
        }

        /// <summary>
        /// 两个向量的点积。
        /// </summary>
        public static float Dot(this Vector3Float lhs, Vector3Float rhs) => (float)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y + (double)lhs.z * (double)rhs.z);

        /// <summary>
        /// 将一个向量投影到另一个向量上。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float Project(this Vector3Float vector, Vector3Float onNormal)
        {
            float num1 = onNormal.Dot(onNormal);
            if (num1 < Vector3Float.kEpsilon)
                return Vector3Float.Zero;
            float num2 = vector.Dot(onNormal);
            return new Vector3Float(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
        }

        /// <summary>
        /// 将一个向量投影到一个平面上，这个平面由一个正交于平面的法线定义。
        /// </summary>
        /// <param name="vector">向量在平面上的位置。</param>
        /// <param name="planeNormal">向量指向平面的方向</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float ProjectOnPlane(this Vector3Float vector, Vector3Float planeNormal)
        {
            float num1 = planeNormal.Dot(planeNormal);
            if (num1 < MathFloat.Epsilon)
                return vector;
            float num2 = vector.Dot(planeNormal);
            return new Vector3Float(vector.x - planeNormal.x * num2 / num1, vector.y - planeNormal.y * num2 / num1, vector.z - planeNormal.z * num2 / num1);
        }

        /// <summary>
        /// 计算和向量之间的夹角。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector3Float from, Vector3Float to)
        {
            float num = (float)Math.Sqrt((double)from.sqrMagnitude * (double)to.sqrMagnitude);
            return (double)num < 1.0000000036274937E-15 ? 0.0f : (float)Math.Acos((double)Math.Clamp(from.Dot(to) / num, -1f, 1f)) * 57.29578f;
        }

        /// <summary>
        /// 计算向量from和to之间相对于轴的带符号角度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector3Float from, Vector3Float to, Vector3Float axis)
        {
            float num1 = from.Angle(to);
            float num2 = (float)((double)from.y * (double)to.z - (double)from.z * (double)to.y);
            float num3 = (float)((double)from.z * (double)to.x - (double)from.x * (double)to.z);
            float num4 = (float)((double)from.x * (double)to.y - (double)from.y * (double)to.x);
            float num5 = Math.Sign((float)((double)axis.x * (double)num2 + (double)axis.y * (double)num3 + (double)axis.z * (double)num4));
            return num1 * num5;
        }

    }
}
