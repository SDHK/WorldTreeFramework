/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:51

* 描述： 4*4矩阵

*/

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using static Unity.Mathematics.math;

namespace WorldTree
{
    public partial struct Matrix4x4Float : IEquatable<Matrix4x4Float>
    {
        // memory layout:
        //
        //                row no (=vertical)
        //               |  0   1   2   3
        //            ---+----------------
        //            0  | m00 m10 m20 m30
        // column no  1  | m01 m11 m21 m31
        // (=horiz)   2  | m02 m12 m22 m32
        //            3  | m03 m13 m23 m33

        public float m00, m10, m20, m30;
        public float m01, m11, m21, m31;
        public float m02, m12, m22, m32;
        public float m03, m13, m23, m33;


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

        public bool Equals(Matrix4x4Float other)
        {
            throw new NotImplementedException();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F5";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n",
                m00.ToString(format, formatProvider), m01.ToString(format, formatProvider), m02.ToString(format, formatProvider), m03.ToString(format, formatProvider),
                m10.ToString(format, formatProvider), m11.ToString(format, formatProvider), m12.ToString(format, formatProvider), m13.ToString(format, formatProvider),
                m20.ToString(format, formatProvider), m21.ToString(format, formatProvider), m22.ToString(format, formatProvider), m23.ToString(format, formatProvider),
                m30.ToString(format, formatProvider), m31.ToString(format, formatProvider), m32.ToString(format, formatProvider), m33.ToString(format, formatProvider));
        }

        /// <summary>
        /// 这个矩阵的逆。(只读)
        /// </summary>
        public Matrix4x4Float inverse => Matrix4x4Float.Inverse(this);

        public Vector4Float Column0 => new(this.m00, this.m10, this.m20, this.m30);
        public Vector4Float Column1 => new(this.m01, this.m11, this.m21, this.m31);
        public Vector4Float Column2 => new(this.m02, this.m12, this.m22, this.m32);
        public Vector4Float Column3 => new(this.m03, this.m13, this.m23, this.m33);

        public Vector4Float Row0 => new(this.m00, this.m01, this.m02, this.m03);
        public Vector4Float Row1 => new(this.m10, this.m11, this.m12, this.m13);
        public Vector4Float Row2 => new(this.m20, this.m21, this.m22, this.m23);
        public Vector4Float Row3 => new(this.m30, this.m31, this.m32, this.m33);

        #region 方法


        /// <summary>
        /// 得到矩阵的一列。
        /// </summary>
        public Vector4Float GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return Column0;
                case 1:
                    return Column1;
                case 2:
                    return Column2;
                case 3:
                    return Column3;
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        /// <summary>
        /// 返回矩阵的一行。
        /// </summary>
        public Vector4Float GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return Row0;
                case 1:
                    return Row1;
                case 2:
                    return Row2;
                case 3:
                    return Row3;
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        /// <summary>
        /// 设置矩阵的一列。
        /// </summary>
        public void SetColumn(int index, Vector4Float column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        /// <summary>
        /// 设置矩阵的一行。
        /// </summary>
        public void SetRow(int index, Vector4Float row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
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

        /// <summary>
        /// 逆矩阵
        /// </summary>
        public static Matrix4x4Float Inverse(Matrix4x4Float m)
        {
            Vector4Float c0 = m.GetColumn(0);
            Vector4Float c1 = m.GetColumn(1);
            Vector4Float c2 = m.GetColumn(2);
            Vector4Float c3 = m.GetColumn(3);

            Vector4Float r0y_r1y_r0x_r1x = new(c1.x, c1.y, c0.x, c0.y);
            Vector4Float r0z_r1z_r0w_r1w = new(c2.x, c2.y, c3.x, c3.y);
            Vector4Float r2y_r3y_r2x_r3x = new(c0.x, c0.y, c1.x, c1.y);
            Vector4Float r2z_r3z_r2w_r3w = new(c3.x, c3.y, c2.x, c2.y);

            Vector4Float r1y_r2y_r1x_r2x = new(c1.y, c1.z, c0.y, c0.z);
            Vector4Float r1z_r2z_r1w_r2w = new(c2.y, c2.z, c3.y, c3.z);
            Vector4Float r3y_r0y_r3x_r0x = new(c1.w, c1.x, c0.w, c0.x);
            Vector4Float r3z_r0z_r3w_r0w = new(c2.w, c2.x, c3.w, c3.x);

            Vector4Float r0_wzyx = new(r0z_r1z_r0w_r1w.z, r0z_r1z_r0w_r1w.x, r0y_r1y_r0x_r1x.x, r0y_r1y_r0x_r1x.z);
            Vector4Float r1_wzyx = new(r0z_r1z_r0w_r1w.w, r0z_r1z_r0w_r1w.y, r0y_r1y_r0x_r1x.y, r0y_r1y_r0x_r1x.w);
            Vector4Float r2_wzyx = new(r2z_r3z_r2w_r3w.z, r2z_r3z_r2w_r3w.x, r2y_r3y_r2x_r3x.x, r2y_r3y_r2x_r3x.z);
            Vector4Float r3_wzyx = new(r2z_r3z_r2w_r3w.w, r2z_r3z_r2w_r3w.y, r2y_r3y_r2x_r3x.y, r2y_r3y_r2x_r3x.w);
            Vector4Float r0_xyzw = new(r0y_r1y_r0x_r1x.z, r0y_r1y_r0x_r1x.x, r0z_r1z_r0w_r1w.x, r0z_r1z_r0w_r1w.z);

            // Calculate remaining inner term pairs. inner terms have zw=-xy, so we only have to calculate xy and can pack two pairs per vector.
            // 计算剩余的内部项对。内项有zw=-xy，所以我们只需要计算xy每个向量可以包含两对向量。
            Vector4Float inner12_23 = r1y_r2y_r1x_r2x * r2z_r3z_r2w_r3w - r1z_r2z_r1w_r2w * r2y_r3y_r2x_r3x;
            Vector4Float inner02_13 = r0y_r1y_r0x_r1x * r2z_r3z_r2w_r3w - r0z_r1z_r0w_r1w * r2y_r3y_r2x_r3x;
            Vector4Float inner30_01 = r3z_r0z_r3w_r0w * r0y_r1y_r0x_r1x - r3y_r0y_r3x_r0x * r0z_r1z_r0w_r1w;

            // Expand inner terms back to 4 components. zw signs still need to be flipped
            // 将内部项展开回4个分量。Zw标志仍然需要被翻转
            Vector4Float inner12 = new(inner12_23.x, inner12_23.z, inner12_23.z, inner12_23.x);
            Vector4Float inner23 = new(inner12_23.y, inner12_23.w, inner12_23.w, inner12_23.y);

            Vector4Float inner02 = new(inner02_13.x, inner02_13.z, inner02_13.z, inner02_13.x);
            Vector4Float inner13 = new(inner02_13.y, inner02_13.w, inner02_13.w, inner02_13.y);

            // Calculate minors
            Vector4Float minors0 = r3_wzyx * inner12 - r2_wzyx * inner13 + r1_wzyx * inner23;

            Vector4Float denom = r0_xyzw * minors0;

            // Horizontal sum of denominator. Free sign flip of z and w compensates for missing flip in inner terms.
            // 分母的水平和。z和w的自由符号翻转弥补了内部项中缺失的翻转。
            denom += new Vector4Float(denom.y, denom.x, denom.w, denom.z);   // x+y        x+y            z+w            z+w
            denom -= new Vector4Float(denom.z, denom.z, denom.x, denom.x);   // x+y-z-w  x+y-z-w        z+w-x-y        z+w-x-y

            Vector4Float rcp_denom_ppnn = Vector4Float.One / denom;
            Matrix4x4Float res = new();
            res.SetColumn(0, minors0 * rcp_denom_ppnn);

            Vector4Float inner30 = new(inner30_01.x, inner30_01.z, inner30_01.z, inner30_01.x);
            Vector4Float inner01 = new(inner30_01.y, inner30_01.w, inner30_01.w, inner30_01.y);

            Vector4Float minors1 = r2_wzyx * inner30 - r0_wzyx * inner23 - r3_wzyx * inner02;
            res.SetColumn(1, minors1 * rcp_denom_ppnn);

            Vector4Float minors2 = r0_wzyx * inner13 - r1_wzyx * inner30 - r3_wzyx * inner01;
            res.SetColumn(2, minors2 * rcp_denom_ppnn);

            Vector4Float minors3 = r1_wzyx * inner02 - r0_wzyx * inner12 + r2_wzyx * inner01;
            res.SetColumn(2, minors3 * rcp_denom_ppnn);

            return res;
        }

        //!!!!

        /// <summary>
        /// 返回在空间中变换的平面。
        /// </summary>
        public PlaneFloat TransformPlane(PlaneFloat plane)
        {
            Matrix4x4Float inverse = this.inverse;
            float x = plane.normal.x;
            float y = plane.normal.y;
            float z = plane.normal.z;
            float distance = plane.distance;
            return new PlaneFloat(new Vector3Float((float)((double)inverse.m00 * (double)x + (double)inverse.m10 * (double)y + (double)inverse.m20 * (double)z + (double)inverse.m30 * (double)distance), (float)((double)inverse.m01 * (double)x + (double)inverse.m11 * (double)y + (double)inverse.m21 * (double)z + (double)inverse.m31 * (double)distance), (float)((double)inverse.m02 * (double)x + (double)inverse.m12 * (double)y + (double)inverse.m22 * (double)z + (double)inverse.m32 * (double)distance)), (float)((double)inverse.m03 * (double)x + (double)inverse.m13 * (double)y + (double)inverse.m23 * (double)z + (double)inverse.m33 * (double)distance));
        }

        #endregion


    }
}
