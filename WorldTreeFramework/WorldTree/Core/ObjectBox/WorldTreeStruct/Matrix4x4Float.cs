/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:51

* 描述： 

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    public partial struct Matrix4x4Float : IEquatable<Matrix4x4Float>
    {

        public float m00;
        public float m10;
        public float m20;
        public float m30;

        public float m01;
        public float m11;
        public float m21;
        public float m31;

        public float m02;
        public float m12;
        public float m22;
        public float m32;

        public float m03;
        public float m13;
        public float m23;
        public float m33;


        private static readonly Matrix4x4Float zeroMatrix = new Matrix4x4Float(new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f));
        private static readonly Matrix4x4Float identityMatrix = new Matrix4x4Float(new Vector4Float(1f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 1f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 1f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 1f));


        public Matrix4x4Float(Vector4Float column0, Vector4Float column1, Vector4Float column2, Vector4Float column3)
        {
            this.m00 = column0.x;
            this.m01 = column1.x;
            this.m02 = column2.x;
            this.m03 = column3.x;
            this.m10 = column0.y;
            this.m11 = column1.y;
            this.m12 = column2.y;
            this.m13 = column3.y;
            this.m20 = column0.z;
            this.m21 = column1.z;
            this.m22 = column2.z;
            this.m23 = column3.z;
            this.m30 = column0.w;
            this.m31 = column1.w;
            this.m32 = column2.w;
            this.m33 = column3.w;
        }


        public float this[int row, int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[row + column * 4];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[row + column * 4] = value;
        }


        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.m00;
                    case 1:
                        return this.m10;
                    case 2:
                        return this.m20;
                    case 3:
                        return this.m30;
                    case 4:
                        return this.m01;
                    case 5:
                        return this.m11;
                    case 6:
                        return this.m21;
                    case 7:
                        return this.m31;
                    case 8:
                        return this.m02;
                    case 9:
                        return this.m12;
                    case 10:
                        return this.m22;
                    case 11:
                        return this.m32;
                    case 12:
                        return this.m03;
                    case 13:
                        return this.m13;
                    case 14:
                        return this.m23;
                    case 15:
                        return this.m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.m00 = value;
                        break;
                    case 1:
                        this.m10 = value;
                        break;
                    case 2:
                        this.m20 = value;
                        break;
                    case 3:
                        this.m30 = value;
                        break;
                    case 4:
                        this.m01 = value;
                        break;
                    case 5:
                        this.m11 = value;
                        break;
                    case 6:
                        this.m21 = value;
                        break;
                    case 7:
                        this.m31 = value;
                        break;
                    case 8:
                        this.m02 = value;
                        break;
                    case 9:
                        this.m12 = value;
                        break;
                    case 10:
                        this.m22 = value;
                        break;
                    case 11:
                        this.m32 = value;
                        break;
                    case 12:
                        this.m03 = value;
                        break;
                    case 13:
                        this.m13 = value;
                        break;
                    case 14:
                        this.m23 = value;
                        break;
                    case 15:
                        this.m33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }


        public bool Equals(Matrix4x4Float other)
        {
            throw new NotImplementedException();
        }
    }
    public static partial class Matrix4x4FloatRule
    {
        public static Matrix4x4Float TRS(Vector3Float pos, QuaternionFloat q, Vector3Float s)
        {
            //计算旋转矩阵的行列式并求其倒数
            float det = 1f / (q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);

            //计算矩阵的第一列
            float x2 = q.x + q.x;
            float y2 = q.y + q.y;
            float z2 = q.z + q.z;

            float xx = q.x * x2 * det;
            float xy = q.x * y2 * det;
            float xz = q.x * z2 * det;
            float xw = q.w * x2 * det;

            float yy = q.y * y2 * det;
            float yz = q.y * z2 * det;
            float yw = q.w * y2 * det;

            float zz = q.z * z2 * det;
            float zw = q.w * z2 * det;

            //计算变换矩阵
            Matrix4x4Float matrix = new Matrix4x4Float();
            matrix[0, 0] = (1f - yy - zz) * s.x;
            matrix[0, 1] = (xy - zw) * s.y;
            matrix[0, 2] = (xz + yw) * s.z;
            matrix[0, 3] = pos.x;

            matrix[1, 0] = (xy + zw) * s.x;
            matrix[1, 1] = (1f - xx - zz) * s.y;
            matrix[1, 2] = (yz - xw) * s.z;
            matrix[1, 3] = pos.y;

            matrix[2, 0] = (xz - yw) * s.x;
            matrix[2, 1] = (yz + xw) * s.y;
            matrix[2, 2] = (1f - xx - yy) * s.z;
            matrix[2, 3] = pos.z;

            matrix[3, 0] = 0f;
            matrix[3, 1] = 0f;
            matrix[3, 2] = 0f;
            matrix[3, 3] = 1f;

            return matrix;
        }


    }
}
