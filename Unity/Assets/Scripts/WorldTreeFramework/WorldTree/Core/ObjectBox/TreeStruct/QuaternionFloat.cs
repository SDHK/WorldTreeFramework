/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:11

* 描述： 框架内置的浮点四元数结构体

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 浮点四元数。
	/// </summary>
	public partial struct QuaternionFloat : IEquatable<QuaternionFloat>
    {
		/// <summary>
		/// 四元数的X分量。
		/// </summary>
		public float X;
		/// <summary>
		/// 四元数的Y分量。
		/// </summary>
		public float Y;
		/// <summary>
		/// 四元数的Z分量。
		/// </summary>
		public float Z;
		/// <summary>
		/// 四元数的W分量。
		/// </summary>
		public float W;

		/// <summary>
		/// 单位四元数。
		/// </summary>
		private static readonly QuaternionFloat identityQuaternion = new QuaternionFloat(0.0f, 0.0f, 0.0f, 1f);



        /// <summary>
        /// 用给定的x,y,z,w分量构造新的四元数。
        /// </summary>
        public QuaternionFloat(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
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
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
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
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

		/// <summary>
		/// 是否相等
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEqualUsingDot(float dot) => (double)dot > 0.9999989867210388;

        public static QuaternionFloat operator *(QuaternionFloat lhs, QuaternionFloat rhs) => new QuaternionFloat((float)((double)lhs.W * (double)rhs.X + (double)lhs.X * (double)rhs.W + (double)lhs.Y * (double)rhs.Z - (double)lhs.Z * (double)rhs.Y), (float)((double)lhs.W * (double)rhs.Y + (double)lhs.Y * (double)rhs.W + (double)lhs.Z * (double)rhs.X - (double)lhs.X * (double)rhs.Z), (float)((double)lhs.W * (double)rhs.Z + (double)lhs.Z * (double)rhs.W + (double)lhs.X * (double)rhs.Y - (double)lhs.Y * (double)rhs.X), (float)((double)lhs.W * (double)rhs.W - (double)lhs.X * (double)rhs.X - (double)lhs.Y * (double)rhs.Y - (double)lhs.Z * (double)rhs.Z));

        public static Vector3Float operator *(QuaternionFloat rotation, Vector3Float point)
        {
            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3Float vector3;
            vector3.X = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.X + ((double)num7 - (double)num12) * (double)point.Y + ((double)num8 + (double)num11) * (double)point.Z);
            vector3.Y = (float)(((double)num7 + (double)num12) * (double)point.X + (1.0 - ((double)num4 + (double)num6)) * (double)point.Y + ((double)num9 - (double)num10) * (double)point.Z);
            vector3.Z = (float)(((double)num8 - (double)num11) * (double)point.X + ((double)num9 + (double)num10) * (double)point.Y + (1.0 - ((double)num4 + (double)num5)) * (double)point.Z);
            return vector3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(QuaternionFloat lhs, QuaternionFloat rhs) => QuaternionFloat.IsEqualUsingDot(lhs.Dot(rhs));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(QuaternionFloat lhs, QuaternionFloat rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2 ^ this.W.GetHashCode() >> 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is QuaternionFloat other1 && this.Equals(other1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(QuaternionFloat other) => this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z) && this.W.Equals(other.W);

		/// <summary>
		/// 单位四元数。
		/// </summary>
		public static QuaternionFloat Identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => QuaternionFloat.identityQuaternion;
        }
        /// <summary>
        /// 返回大小为1的四元数(只读)。
        /// </summary>
        public QuaternionFloat Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float num = (float)Math.Sqrt(this.Dot(this));
                return (double)num < (double)MathFloat.Epsilon ? QuaternionFloat.Identity : new QuaternionFloat(X / num, Y / num, Z / num, W / num);
            }
        }
        /// <summary>
        /// 四元数的欧拉角
        /// </summary>
        public Vector3Float EulerAngles
        {
            get
            {
                //将四元数转换为旋转矩阵
                Matrix4x4Float m = Matrix4x4Float.TRS(Vector3Float.Zero, this, Vector3Float.One);

                //从旋转矩阵中提取欧拉角
                Vector3Float euler = default;

                if (m[2, 0] < 1)
                {
                    if (m[2, 0] > -1)
                    {
                        euler.X = MathFloat.Atan2(-m[2, 1], m[2, 2]);
                        euler.Y = MathFloat.Asin(m[2, 0]);
                        euler.Z = MathFloat.Atan2(-m[1, 0], m[0, 0]);
                    }
                    else
                    {
                        euler.X = MathFloat.Atan2(m[1, 2], m[1, 1]);
                        euler.Y = -MathFloat.PI / 2f;
                        euler.Z = 0f;
                    }
                }
                else
                {
                    euler.X = MathFloat.Atan2(m[1, 2], m[1, 1]);
                    euler.Y = MathFloat.PI / 2f;
                    euler.Z = 0f;
                }

                //将弧度转换为角度
                euler *= MathFloat.RAD_DEG;
                return euler;
            }
            set
            {
                //将角度转换为弧度
                Vector3Float euler = value * MathFloat.DEG_RAD;

                //计算对应的四元数
                QuaternionFloat q = QuaternionFloat.Identity;
                q *= QuaternionFloat.AngleAxis(euler.X, Vector3Float.Right);
                q *= QuaternionFloat.AngleAxis(euler.Y, Vector3Float.Up);
                q *= QuaternionFloat.AngleAxis(euler.Z, Vector3Float.Forward);

                this = q;
            }
        }

        /// <summary>
        /// 创建一个旋转
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="axis">轴</param>
        public static QuaternionFloat AngleAxis(float angle, Vector3Float axis)
        {
            //将角度转换为弧度
            float rad = MathFloat.DEG_RAD * angle;

            //归一化向量
            axis.Normalize();

            //将角度的一半表示为弧度
            float halfAngle = rad * 0.5f;

            //计算sin和cos值
            float sin = MathFloat.Sin(halfAngle);
            float cos = MathFloat.Cos(halfAngle);

            //将向量与sin值相乘并乘以四元数的W分量
            Vector3Float scaledAxis = axis * sin;
            return new QuaternionFloat(scaledAxis.X, scaledAxis.Y, scaledAxis.Z, cos);
        }

        /// <summary>
        /// 创建一个旋转
        /// </summary>
        public static QuaternionFloat LookRotation(Vector3Float forward) => LookRotation(forward, Vector3Float.Up);

        /// <summary>
        /// 创建一个旋转
        /// </summary>
        public static QuaternionFloat LookRotation(Vector3Float forward, Vector3Float upwards)
        {
            //如果forward和upwards相同，则返回单位四元数
            if (forward == upwards || forward == -upwards)
            {
                return QuaternionFloat.Identity;
            }

            //计算向量间的旋转
            Vector3Float right = upwards.Cross(forward);
            upwards = forward.Cross(right);
            forward.Normalize();
            upwards.Normalize();
            right.Normalize();

            //根据向量构造四元数
            float m00 = right.X;
            float m01 = upwards.X;
            float m02 = forward.X;
            float m10 = right.Y;
            float m11 = upwards.Y;
            float m12 = forward.Y;
            float m20 = right.Z;
            float m21 = upwards.Z;
            float m22 = forward.Z;

            float num8 = (m00 + m11) + m22;
            QuaternionFloat quaternion = new();

            if (num8 > 0f)
            {
                float num = (float)Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }

            if ((m00 >= m11) && (m00 >= m22))
            {
                float num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                float num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }

            if (m11 > m22)
            {
                float num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                float num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }

            float num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            float num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }


    }
}
