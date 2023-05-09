/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:11

* 描述： 框架内置的浮点四元数结构体

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    public partial struct QuaternionFloat : IEquatable<QuaternionFloat>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        private static readonly QuaternionFloat identityQuaternion = new QuaternionFloat(0.0f, 0.0f, 0.0f, 1f);



        /// <summary>
        /// 用给定的x,y,z,w分量构造新的四元数。
        /// </summary>
        public QuaternionFloat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
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
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
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
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEqualUsingDot(float dot) => (double)dot > 0.9999989867210388;

        public static QuaternionFloat operator *(QuaternionFloat lhs, QuaternionFloat rhs) => new QuaternionFloat((float)((double)lhs.w * (double)rhs.x + (double)lhs.x * (double)rhs.w + (double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), (float)((double)lhs.w * (double)rhs.y + (double)lhs.y * (double)rhs.w + (double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), (float)((double)lhs.w * (double)rhs.z + (double)lhs.z * (double)rhs.w + (double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x), (float)((double)lhs.w * (double)rhs.w - (double)lhs.x * (double)rhs.x - (double)lhs.y * (double)rhs.y - (double)lhs.z * (double)rhs.z));

        public static Vector3Float operator *(QuaternionFloat rotation, Vector3Float point)
        {
            float num1 = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num1;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num1;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vector3Float vector3;
            vector3.x = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.x + ((double)num7 - (double)num12) * (double)point.y + ((double)num8 + (double)num11) * (double)point.z);
            vector3.y = (float)(((double)num7 + (double)num12) * (double)point.x + (1.0 - ((double)num4 + (double)num6)) * (double)point.y + ((double)num9 - (double)num10) * (double)point.z);
            vector3.z = (float)(((double)num8 - (double)num11) * (double)point.x + ((double)num9 + (double)num10) * (double)point.y + (1.0 - ((double)num4 + (double)num5)) * (double)point.z);
            return vector3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(QuaternionFloat lhs, QuaternionFloat rhs) => QuaternionFloat.IsEqualUsingDot(lhs.Dot(rhs));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(QuaternionFloat lhs, QuaternionFloat rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is QuaternionFloat other1 && this.Equals(other1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(QuaternionFloat other) => this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);


        public static QuaternionFloat identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => QuaternionFloat.identityQuaternion;
        }
        /// <summary>
        /// 返回大小为1的四元数(只读)。
        /// </summary>
        public QuaternionFloat normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float num = (float)Math.Sqrt(this.Dot(this));
                return (double)num < (double)MathFloat.Epsilon ? QuaternionFloat.identity : new QuaternionFloat(x / num, y / num, z / num, w / num);
            }
        }

        public Vector3Float eulerAngles
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
                        euler.x = MathFloat.Atan2(-m[2, 1], m[2, 2]);
                        euler.y = MathFloat.Asin(m[2, 0]);
                        euler.z = MathFloat.Atan2(-m[1, 0], m[0, 0]);
                    }
                    else
                    {
                        euler.x = MathFloat.Atan2(m[1, 2], m[1, 1]);
                        euler.y = -MathFloat.PI / 2f;
                        euler.z = 0f;
                    }
                }
                else
                {
                    euler.x = MathFloat.Atan2(m[1, 2], m[1, 1]);
                    euler.y = MathFloat.PI / 2f;
                    euler.z = 0f;
                }

                //将弧度转换为角度
                euler *= MathFloat.Rad2Deg;
                return euler;
            }
            set
            {
                //将角度转换为弧度
                Vector3Float euler = value * MathFloat.Deg2Rad;

                //计算对应的四元数
                QuaternionFloat q = QuaternionFloat.identity;
                q *= QuaternionFloat.AngleAxis(euler.x, Vector3Float.Right);
                q *= QuaternionFloat.AngleAxis(euler.y, Vector3Float.Up);
                q *= QuaternionFloat.AngleAxis(euler.z, Vector3Float.Forward);

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
            float rad = MathFloat.Deg2Rad * angle;

            //归一化向量
            axis.Normalize();

            //将角度的一半表示为弧度
            float halfAngle = rad * 0.5f;

            //计算sin和cos值
            float sin = MathFloat.Sin(halfAngle);
            float cos = MathFloat.Cos(halfAngle);

            //将向量与sin值相乘并乘以四元数的W分量
            Vector3Float scaledAxis = axis * sin;
            return new QuaternionFloat(scaledAxis.x, scaledAxis.y, scaledAxis.z, cos);
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
                return QuaternionFloat.identity;
            }

            //计算向量间的旋转
            Vector3Float right = upwards.Cross(forward);
            upwards = forward.Cross(right);
            forward.Normalize();
            upwards.Normalize();
            right.Normalize();

            //根据向量构造四元数
            float m00 = right.x;
            float m01 = upwards.x;
            float m02 = forward.x;
            float m10 = right.y;
            float m11 = upwards.y;
            float m12 = forward.y;
            float m20 = right.z;
            float m21 = upwards.z;
            float m22 = forward.z;

            float num8 = (m00 + m11) + m22;
            QuaternionFloat quaternion = new();

            if (num8 > 0f)
            {
                float num = (float)Math.Sqrt(num8 + 1f);
                quaternion.w = num * 0.5f;
                num = 0.5f / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }

            if ((m00 >= m11) && (m00 >= m22))
            {
                float num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                float num4 = 0.5f / num7;
                quaternion.x = 0.5f * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }

            if (m11 > m22)
            {
                float num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                float num3 = 0.5f / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5f * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }

            float num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            float num2 = 0.5f / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5f * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }


    }
}
