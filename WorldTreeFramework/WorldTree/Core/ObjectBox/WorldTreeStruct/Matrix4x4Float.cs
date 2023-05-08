/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:51

* 描述： 4*4矩阵

*/

using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

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

        /// <summary>
        /// 返回一个所有元素为零的矩阵(只读)。
        /// </summary>
        public static Matrix4x4Float zero => Matrix4x4Float.zeroMatrix;

        /// <summary>
        /// 返回单位矩阵(只读)。
        /// </summary>
        public static Matrix4x4Float identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Matrix4x4Float.identityMatrix;
        }

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
                        throw new IndexOutOfRangeException("Invalid Matrix4x4Float index!");
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
                        throw new IndexOutOfRangeException("Invalid Matrix4x4Float index!");
                }
            }
        }


        public static Matrix4x4Float operator *(Matrix4x4Float lhs, Matrix4x4Float rhs)
        {
            Matrix4x4Float matrix4x4;
            matrix4x4.m00 = (float)((double)lhs.m00 * (double)rhs.m00 + (double)lhs.m01 * (double)rhs.m10 + (double)lhs.m02 * (double)rhs.m20 + (double)lhs.m03 * (double)rhs.m30);
            matrix4x4.m01 = (float)((double)lhs.m00 * (double)rhs.m01 + (double)lhs.m01 * (double)rhs.m11 + (double)lhs.m02 * (double)rhs.m21 + (double)lhs.m03 * (double)rhs.m31);
            matrix4x4.m02 = (float)((double)lhs.m00 * (double)rhs.m02 + (double)lhs.m01 * (double)rhs.m12 + (double)lhs.m02 * (double)rhs.m22 + (double)lhs.m03 * (double)rhs.m32);
            matrix4x4.m03 = (float)((double)lhs.m00 * (double)rhs.m03 + (double)lhs.m01 * (double)rhs.m13 + (double)lhs.m02 * (double)rhs.m23 + (double)lhs.m03 * (double)rhs.m33);
            matrix4x4.m10 = (float)((double)lhs.m10 * (double)rhs.m00 + (double)lhs.m11 * (double)rhs.m10 + (double)lhs.m12 * (double)rhs.m20 + (double)lhs.m13 * (double)rhs.m30);
            matrix4x4.m11 = (float)((double)lhs.m10 * (double)rhs.m01 + (double)lhs.m11 * (double)rhs.m11 + (double)lhs.m12 * (double)rhs.m21 + (double)lhs.m13 * (double)rhs.m31);
            matrix4x4.m12 = (float)((double)lhs.m10 * (double)rhs.m02 + (double)lhs.m11 * (double)rhs.m12 + (double)lhs.m12 * (double)rhs.m22 + (double)lhs.m13 * (double)rhs.m32);
            matrix4x4.m13 = (float)((double)lhs.m10 * (double)rhs.m03 + (double)lhs.m11 * (double)rhs.m13 + (double)lhs.m12 * (double)rhs.m23 + (double)lhs.m13 * (double)rhs.m33);
            matrix4x4.m20 = (float)((double)lhs.m20 * (double)rhs.m00 + (double)lhs.m21 * (double)rhs.m10 + (double)lhs.m22 * (double)rhs.m20 + (double)lhs.m23 * (double)rhs.m30);
            matrix4x4.m21 = (float)((double)lhs.m20 * (double)rhs.m01 + (double)lhs.m21 * (double)rhs.m11 + (double)lhs.m22 * (double)rhs.m21 + (double)lhs.m23 * (double)rhs.m31);
            matrix4x4.m22 = (float)((double)lhs.m20 * (double)rhs.m02 + (double)lhs.m21 * (double)rhs.m12 + (double)lhs.m22 * (double)rhs.m22 + (double)lhs.m23 * (double)rhs.m32);
            matrix4x4.m23 = (float)((double)lhs.m20 * (double)rhs.m03 + (double)lhs.m21 * (double)rhs.m13 + (double)lhs.m22 * (double)rhs.m23 + (double)lhs.m23 * (double)rhs.m33);
            matrix4x4.m30 = (float)((double)lhs.m30 * (double)rhs.m00 + (double)lhs.m31 * (double)rhs.m10 + (double)lhs.m32 * (double)rhs.m20 + (double)lhs.m33 * (double)rhs.m30);
            matrix4x4.m31 = (float)((double)lhs.m30 * (double)rhs.m01 + (double)lhs.m31 * (double)rhs.m11 + (double)lhs.m32 * (double)rhs.m21 + (double)lhs.m33 * (double)rhs.m31);
            matrix4x4.m32 = (float)((double)lhs.m30 * (double)rhs.m02 + (double)lhs.m31 * (double)rhs.m12 + (double)lhs.m32 * (double)rhs.m22 + (double)lhs.m33 * (double)rhs.m32);
            matrix4x4.m33 = (float)((double)lhs.m30 * (double)rhs.m03 + (double)lhs.m31 * (double)rhs.m13 + (double)lhs.m32 * (double)rhs.m23 + (double)lhs.m33 * (double)rhs.m33);
            return matrix4x4;
        }

        public static Vector4Float operator *(Matrix4x4Float lhs, Vector4Float vector)
        {
            Vector4Float vector4;
            vector4.x = (float)((double)lhs.m00 * (double)vector.x + (double)lhs.m01 * (double)vector.y + (double)lhs.m02 * (double)vector.z + (double)lhs.m03 * (double)vector.w);
            vector4.y = (float)((double)lhs.m10 * (double)vector.x + (double)lhs.m11 * (double)vector.y + (double)lhs.m12 * (double)vector.z + (double)lhs.m13 * (double)vector.w);
            vector4.z = (float)((double)lhs.m20 * (double)vector.x + (double)lhs.m21 * (double)vector.y + (double)lhs.m22 * (double)vector.z + (double)lhs.m23 * (double)vector.w);
            vector4.w = (float)((double)lhs.m30 * (double)vector.x + (double)lhs.m31 * (double)vector.y + (double)lhs.m32 * (double)vector.z + (double)lhs.m33 * (double)vector.w);
            return vector4;
        }

        public static bool operator ==(Matrix4x4Float lhs, Matrix4x4Float rhs) => lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);

        public static bool operator !=(Matrix4x4Float lhs, Matrix4x4Float rhs) => !(lhs == rhs);




        ///// <summary>
        ///// 返回在空间中变换的平面。
        ///// </summary>
        //public PlaneFloat TransformPlane(PlaneFloat plane)
        //{
        //    Matrix4x4Float inverse = this.inverse;
        //    float x = plane.normal.x;
        //    float y = plane.normal.y;
        //    float z = plane.normal.z;
        //    float distance = plane.distance;
        //    return new PlaneFloat(new Vector3Float((float)((double)inverse.m00 * (double)x + (double)inverse.m10 * (double)y + (double)inverse.m20 * (double)z + (double)inverse.m30 * (double)distance), (float)((double)inverse.m01 * (double)x + (double)inverse.m11 * (double)y + (double)inverse.m21 * (double)z + (double)inverse.m31 * (double)distance), (float)((double)inverse.m02 * (double)x + (double)inverse.m12 * (double)y + (double)inverse.m22 * (double)z + (double)inverse.m32 * (double)distance)), (float)((double)inverse.m03 * (double)x + (double)inverse.m13 * (double)y + (double)inverse.m23 * (double)z + (double)inverse.m33 * (double)distance));
        //}


        public bool Equals(Matrix4x4Float other)
        {
            throw new NotImplementedException();
        }
    }
    public static partial class Matrix4x4FloatRule
    {

        /// <summary>
        ///   <para>Get a column of the matrix.</para>
        /// </summary>
        public static Vector4Float GetColumn(this Matrix4x4Float self, int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector4Float(self.m00, self.m10, self.m20, self.m30);
                case 1:
                    return new Vector4Float(self.m01, self.m11, self.m21, self.m31);
                case 2:
                    return new Vector4Float(self.m02, self.m12, self.m22, self.m32);
                case 3:
                    return new Vector4Float(self.m03, self.m13, self.m23, self.m33);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        /// <summary>
        ///   <para>Returns a row of the matrix.</para>
        /// </summary>
        public static Vector4Float GetRow(this Matrix4x4Float self, int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector4Float(self.m00, self.m01, self.m02, self.m03);
                case 1:
                    return new Vector4Float(self.m10, self.m11, self.m12, self.m13);
                case 2:
                    return new Vector4Float(self.m20, self.m21, self.m22, self.m23);
                case 3:
                    return new Vector4Float(self.m30, self.m31, self.m32, self.m33);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        /// <summary>
        ///   <para>Sets a column of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="column"></param>
        public static void SetColumn(this Matrix4x4Float self, int index, Vector4Float column)
        {
            self[0, index] = column.x;
            self[1, index] = column.y;
            self[2, index] = column.z;
            self[3, index] = column.w;
        }

        /// <summary>
        ///   <para>Sets a row of the matrix.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="row"></param>
        public static void SetRow(this Matrix4x4Float self, int index, Vector4Float row)
        {
            self[index, 0] = row.x;
            self[index, 1] = row.y;
            self[index, 2] = row.z;
            self[index, 3] = row.w;
        }




        /// <summary>
        /// 创建缩放矩阵。
        /// </summary>
        public static Matrix4x4Float Scale(Vector3Float vector)
        {
            Matrix4x4Float matrix4x4;
            matrix4x4.m00 = vector.x;
            matrix4x4.m01 = 0.0f;
            matrix4x4.m02 = 0.0f;
            matrix4x4.m03 = 0.0f;
            matrix4x4.m10 = 0.0f;
            matrix4x4.m11 = vector.y;
            matrix4x4.m12 = 0.0f;
            matrix4x4.m13 = 0.0f;
            matrix4x4.m20 = 0.0f;
            matrix4x4.m21 = 0.0f;
            matrix4x4.m22 = vector.z;
            matrix4x4.m23 = 0.0f;
            matrix4x4.m30 = 0.0f;
            matrix4x4.m31 = 0.0f;
            matrix4x4.m32 = 0.0f;
            matrix4x4.m33 = 1f;
            return matrix4x4;
        }

        /// <summary>
        /// 创建一个转换矩阵。
        /// </summary>
        public static Matrix4x4Float Translate(Vector3Float vector)
        {
            Matrix4x4Float matrix4x4;
            matrix4x4.m00 = 1f;
            matrix4x4.m01 = 0.0f;
            matrix4x4.m02 = 0.0f;
            matrix4x4.m03 = vector.x;
            matrix4x4.m10 = 0.0f;
            matrix4x4.m11 = 1f;
            matrix4x4.m12 = 0.0f;
            matrix4x4.m13 = vector.y;
            matrix4x4.m20 = 0.0f;
            matrix4x4.m21 = 0.0f;
            matrix4x4.m22 = 1f;
            matrix4x4.m23 = vector.z;
            matrix4x4.m30 = 0.0f;
            matrix4x4.m31 = 0.0f;
            matrix4x4.m32 = 0.0f;
            matrix4x4.m33 = 1f;
            return matrix4x4;
        }

        /// <summary>
        /// 创建一个旋转矩阵。
        /// </summary>
        public static Matrix4x4Float Rotate(QuaternionFloat q)
        {
            float num1 = q.x * 2f;
            float num2 = q.y * 2f;
            float num3 = q.z * 2f;
            float num4 = q.x * num1;
            float num5 = q.y * num2;
            float num6 = q.z * num3;
            float num7 = q.x * num2;
            float num8 = q.x * num3;
            float num9 = q.y * num3;
            float num10 = q.w * num1;
            float num11 = q.w * num2;
            float num12 = q.w * num3;
            Matrix4x4Float matrix4x4;
            matrix4x4.m00 = (float)(1.0 - ((double)num5 + (double)num6));
            matrix4x4.m10 = num7 + num12;
            matrix4x4.m20 = num8 - num11;
            matrix4x4.m30 = 0.0f;
            matrix4x4.m01 = num7 - num12;
            matrix4x4.m11 = (float)(1.0 - ((double)num4 + (double)num6));
            matrix4x4.m21 = num9 + num10;
            matrix4x4.m31 = 0.0f;
            matrix4x4.m02 = num8 + num11;
            matrix4x4.m12 = num9 - num10;
            matrix4x4.m22 = (float)(1.0 - ((double)num4 + (double)num5));
            matrix4x4.m32 = 0.0f;
            matrix4x4.m03 = 0.0f;
            matrix4x4.m13 = 0.0f;
            matrix4x4.m23 = 0.0f;
            matrix4x4.m33 = 1f;
            return matrix4x4;
        }


        /// <summary>
        /// 创建一个新的矩阵
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="q">旋转</param>
        /// <param name="s">缩放</param>
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
