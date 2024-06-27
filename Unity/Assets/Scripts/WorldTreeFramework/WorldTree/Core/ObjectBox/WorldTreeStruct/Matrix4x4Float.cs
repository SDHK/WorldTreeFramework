/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 17:51

* 描述： 4*4矩阵

*/

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 4*4矩阵
	/// </summary>
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

		/// <summary>
		/// 第一行
		/// </summary>
		public float M00, M10, M20, M30;
		/// <summary>
		/// 第二行
		/// </summary>
		public float M01, M11, M21, M31;
		/// <summary>
		/// 第三行
		/// </summary>
		public float M02, M12, M22, M32;
		/// <summary>
		/// 第四行
		/// </summary>
		public float M03, M13, M23, M33;

		/// <summary>
		/// 零矩阵(只读)。
		/// </summary>
		private static readonly Matrix4x4Float zeroMatrix = new Matrix4x4Float(new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 0.0f));
		/// <summary>
		/// 单位矩阵(只读)。
		/// </summary>
		private static readonly Matrix4x4Float identityMatrix = new Matrix4x4Float(new Vector4Float(1f, 0.0f, 0.0f, 0.0f), new Vector4Float(0.0f, 1f, 0.0f, 0.0f), new Vector4Float(0.0f, 0.0f, 1f, 0.0f), new Vector4Float(0.0f, 0.0f, 0.0f, 1f));

		/// <summary>
		/// 返回一个所有元素为零的矩阵(只读)。
		/// </summary>
		public static Matrix4x4Float Zero => Matrix4x4Float.zeroMatrix;

		/// <summary>
		/// 返回单位矩阵(只读)。
		/// </summary>
		public static Matrix4x4Float Identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Matrix4x4Float.identityMatrix;
		}

		public Matrix4x4Float(Vector4Float column0, Vector4Float column1, Vector4Float column2, Vector4Float column3)
		{
			this.M00 = column0.X;
			this.M01 = column1.X;
			this.M02 = column2.X;
			this.M03 = column3.X;
			this.M10 = column0.Y;
			this.M11 = column1.Y;
			this.M12 = column2.Y;
			this.M13 = column3.Y;
			this.M20 = column0.Z;
			this.M21 = column1.Z;
			this.M22 = column2.Z;
			this.M23 = column3.Z;
			this.M30 = column0.W;
			this.M31 = column1.W;
			this.M32 = column2.W;
			this.M33 = column3.W;
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
						return this.M00;
					case 1:
						return this.M10;
					case 2:
						return this.M20;
					case 3:
						return this.M30;
					case 4:
						return this.M01;
					case 5:
						return this.M11;
					case 6:
						return this.M21;
					case 7:
						return this.M31;
					case 8:
						return this.M02;
					case 9:
						return this.M12;
					case 10:
						return this.M22;
					case 11:
						return this.M32;
					case 12:
						return this.M03;
					case 13:
						return this.M13;
					case 14:
						return this.M23;
					case 15:
						return this.M33;
					default:
						throw new IndexOutOfRangeException("Invalid Matrix4x4Float index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						this.M00 = value;
						break;
					case 1:
						this.M10 = value;
						break;
					case 2:
						this.M20 = value;
						break;
					case 3:
						this.M30 = value;
						break;
					case 4:
						this.M01 = value;
						break;
					case 5:
						this.M11 = value;
						break;
					case 6:
						this.M21 = value;
						break;
					case 7:
						this.M31 = value;
						break;
					case 8:
						this.M02 = value;
						break;
					case 9:
						this.M12 = value;
						break;
					case 10:
						this.M22 = value;
						break;
					case 11:
						this.M32 = value;
						break;
					case 12:
						this.M03 = value;
						break;
					case 13:
						this.M13 = value;
						break;
					case 14:
						this.M23 = value;
						break;
					case 15:
						this.M33 = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Matrix4x4Float index!");
				}
			}
		}


		public static Matrix4x4Float operator *(Matrix4x4Float lhs, Matrix4x4Float rhs)
		{
			Matrix4x4Float matrix4x4;
			matrix4x4.M00 = (float)((double)lhs.M00 * (double)rhs.M00 + (double)lhs.M01 * (double)rhs.M10 + (double)lhs.M02 * (double)rhs.M20 + (double)lhs.M03 * (double)rhs.M30);
			matrix4x4.M01 = (float)((double)lhs.M00 * (double)rhs.M01 + (double)lhs.M01 * (double)rhs.M11 + (double)lhs.M02 * (double)rhs.M21 + (double)lhs.M03 * (double)rhs.M31);
			matrix4x4.M02 = (float)((double)lhs.M00 * (double)rhs.M02 + (double)lhs.M01 * (double)rhs.M12 + (double)lhs.M02 * (double)rhs.M22 + (double)lhs.M03 * (double)rhs.M32);
			matrix4x4.M03 = (float)((double)lhs.M00 * (double)rhs.M03 + (double)lhs.M01 * (double)rhs.M13 + (double)lhs.M02 * (double)rhs.M23 + (double)lhs.M03 * (double)rhs.M33);
			matrix4x4.M10 = (float)((double)lhs.M10 * (double)rhs.M00 + (double)lhs.M11 * (double)rhs.M10 + (double)lhs.M12 * (double)rhs.M20 + (double)lhs.M13 * (double)rhs.M30);
			matrix4x4.M11 = (float)((double)lhs.M10 * (double)rhs.M01 + (double)lhs.M11 * (double)rhs.M11 + (double)lhs.M12 * (double)rhs.M21 + (double)lhs.M13 * (double)rhs.M31);
			matrix4x4.M12 = (float)((double)lhs.M10 * (double)rhs.M02 + (double)lhs.M11 * (double)rhs.M12 + (double)lhs.M12 * (double)rhs.M22 + (double)lhs.M13 * (double)rhs.M32);
			matrix4x4.M13 = (float)((double)lhs.M10 * (double)rhs.M03 + (double)lhs.M11 * (double)rhs.M13 + (double)lhs.M12 * (double)rhs.M23 + (double)lhs.M13 * (double)rhs.M33);
			matrix4x4.M20 = (float)((double)lhs.M20 * (double)rhs.M00 + (double)lhs.M21 * (double)rhs.M10 + (double)lhs.M22 * (double)rhs.M20 + (double)lhs.M23 * (double)rhs.M30);
			matrix4x4.M21 = (float)((double)lhs.M20 * (double)rhs.M01 + (double)lhs.M21 * (double)rhs.M11 + (double)lhs.M22 * (double)rhs.M21 + (double)lhs.M23 * (double)rhs.M31);
			matrix4x4.M22 = (float)((double)lhs.M20 * (double)rhs.M02 + (double)lhs.M21 * (double)rhs.M12 + (double)lhs.M22 * (double)rhs.M22 + (double)lhs.M23 * (double)rhs.M32);
			matrix4x4.M23 = (float)((double)lhs.M20 * (double)rhs.M03 + (double)lhs.M21 * (double)rhs.M13 + (double)lhs.M22 * (double)rhs.M23 + (double)lhs.M23 * (double)rhs.M33);
			matrix4x4.M30 = (float)((double)lhs.M30 * (double)rhs.M00 + (double)lhs.M31 * (double)rhs.M10 + (double)lhs.M32 * (double)rhs.M20 + (double)lhs.M33 * (double)rhs.M30);
			matrix4x4.M31 = (float)((double)lhs.M30 * (double)rhs.M01 + (double)lhs.M31 * (double)rhs.M11 + (double)lhs.M32 * (double)rhs.M21 + (double)lhs.M33 * (double)rhs.M31);
			matrix4x4.M32 = (float)((double)lhs.M30 * (double)rhs.M02 + (double)lhs.M31 * (double)rhs.M12 + (double)lhs.M32 * (double)rhs.M22 + (double)lhs.M33 * (double)rhs.M32);
			matrix4x4.M33 = (float)((double)lhs.M30 * (double)rhs.M03 + (double)lhs.M31 * (double)rhs.M13 + (double)lhs.M32 * (double)rhs.M23 + (double)lhs.M33 * (double)rhs.M33);
			return matrix4x4;
		}

		public static Vector4Float operator *(Matrix4x4Float lhs, Vector4Float vector)
		{
			Vector4Float vector4;
			vector4.X = (float)((double)lhs.M00 * (double)vector.X + (double)lhs.M01 * (double)vector.Y + (double)lhs.M02 * (double)vector.Z + (double)lhs.M03 * (double)vector.W);
			vector4.Y = (float)((double)lhs.M10 * (double)vector.X + (double)lhs.M11 * (double)vector.Y + (double)lhs.M12 * (double)vector.Z + (double)lhs.M13 * (double)vector.W);
			vector4.Z = (float)((double)lhs.M20 * (double)vector.X + (double)lhs.M21 * (double)vector.Y + (double)lhs.M22 * (double)vector.Z + (double)lhs.M23 * (double)vector.W);
			vector4.W = (float)((double)lhs.M30 * (double)vector.X + (double)lhs.M31 * (double)vector.Y + (double)lhs.M32 * (double)vector.Z + (double)lhs.M33 * (double)vector.W);
			return vector4;
		}

		public static bool operator ==(Matrix4x4Float lhs, Matrix4x4Float rhs) => lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);

		public static bool operator !=(Matrix4x4Float lhs, Matrix4x4Float rhs) => !(lhs == rhs);


		public override int GetHashCode()
		{
			Vector4Float column = this.GetColumn(0);
			int hashCode = column.GetHashCode();
			column = this.GetColumn(1);
			int num1 = column.GetHashCode() << 2;
			int num2 = hashCode ^ num1;
			column = this.GetColumn(2);
			int num3 = column.GetHashCode() >> 2;
			int num4 = num2 ^ num3;
			column = this.GetColumn(3);
			int num5 = column.GetHashCode() >> 1;
			return num4 ^ num5;
		}
		public override bool Equals(object other) => other is Matrix4x4Float other1 && this.Equals(other1);

		public bool Equals(Matrix4x4Float other)
		{
			bool isEquals = true;
			for (int i = 0; i < 16; i++)
			{
				if (this[i] != other[i])
				{
					isEquals = false;
					break;
				}
			}
			return isEquals;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return ToString(null, null);
		}

		/// <summary>
		/// 返回这个矩阵的字符串表示形式。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return ToString(format, null);
		}
		/// <summary>
		/// 返回这个矩阵的字符串表示形式。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (string.IsNullOrEmpty(format))
				format = "F5";
			if (formatProvider == null)
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n",
				M00.ToString(format, formatProvider), M01.ToString(format, formatProvider), M02.ToString(format, formatProvider), M03.ToString(format, formatProvider),
				M10.ToString(format, formatProvider), M11.ToString(format, formatProvider), M12.ToString(format, formatProvider), M13.ToString(format, formatProvider),
				M20.ToString(format, formatProvider), M21.ToString(format, formatProvider), M22.ToString(format, formatProvider), M23.ToString(format, formatProvider),
				M30.ToString(format, formatProvider), M31.ToString(format, formatProvider), M32.ToString(format, formatProvider), M33.ToString(format, formatProvider));
		}

		/// <summary>
		/// 这个矩阵的逆。(只读)
		/// </summary>
		public Matrix4x4Float Inverse => Matrix4x4Float.GetInverse(this);

		/// <summary>
		/// 第一列
		/// </summary>
		public Vector4Float Column0 => new(this.M00, this.M10, this.M20, this.M30);
		/// <summary>
		/// 第二列
		/// </summary>
		public Vector4Float Column1 => new(this.M01, this.M11, this.M21, this.M31);
		/// <summary>
		/// 第三列
		/// </summary>
		public Vector4Float Column2 => new(this.M02, this.M12, this.M22, this.M32);
		/// <summary>
		/// 第四列
		/// </summary>
		public Vector4Float Column3 => new(this.M03, this.M13, this.M23, this.M33);

		/// <summary>
		/// 第一行
		/// </summary>
		public Vector4Float Row0 => new(this.M00, this.M01, this.M02, this.M03);
		/// <summary>
		/// 第二行
		/// </summary>
		public Vector4Float Row1 => new(this.M10, this.M11, this.M12, this.M13);
		/// <summary>
		/// 第三行
		/// </summary>
		public Vector4Float Row2 => new(this.M20, this.M21, this.M22, this.M23);
		/// <summary>
		/// 第四行
		/// </summary>
		public Vector4Float Row3 => new(this.M30, this.M31, this.M32, this.M33);

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
			this[0, index] = column.X;
			this[1, index] = column.Y;
			this[2, index] = column.Z;
			this[3, index] = column.W;
		}

		/// <summary>
		/// 设置矩阵的一行。
		/// </summary>
		public void SetRow(int index, Vector4Float row)
		{
			this[index, 0] = row.X;
			this[index, 1] = row.Y;
			this[index, 2] = row.Z;
			this[index, 3] = row.W;
		}

		/// <summary>
		/// 创建一个正交投影矩阵。
		/// </summary>
		/// <param name="width">宽</param>
		/// <param name="height">高</param>
		/// <param name="zNearPlane">近平面</param>
		/// <param name="zFarPlane">远平面</param>
		public static Matrix4x4Float CreateOrthographic(
		  float width,
		  float height,
		  float zNearPlane,
		  float zFarPlane)
		{
			Matrix4x4Float orthographic;
			orthographic.M00 = 2f / width;
			orthographic.M01 = orthographic.M02 = orthographic.M03 = 0.0f;
			orthographic.M11 = 2f / height;
			orthographic.M10 = orthographic.M12 = orthographic.M13 = 0.0f;
			orthographic.M22 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
			orthographic.M20 = orthographic.M21 = orthographic.M23 = 0.0f;
			orthographic.M30 = orthographic.M31 = 0.0f;
			orthographic.M32 = zNearPlane / (zNearPlane - zFarPlane);
			orthographic.M33 = 1f;
			return orthographic;
		}
		/// <summary>
		/// 创建一个正交投影矩阵。
		/// </summary>
		/// <param name="left">左</param>
		/// <param name="right">右</param>
		/// <param name="bottom">底</param>
		/// <param name="top">顶</param>
		/// <param name="zNearPlane">近平面</param>
		/// <param name="zFarPlane">远平面</param>
		public static Matrix4x4Float CreateOrthographicOffCenter(
			 float left,
			 float right,
			 float bottom,
			 float top,
			 float zNearPlane,
			 float zFarPlane)
		{
			Matrix4x4Float orthographicOffCenter;
			orthographicOffCenter.M00 = (float)(2.0 / ((double)right - (double)left));
			orthographicOffCenter.M01 = orthographicOffCenter.M02 = orthographicOffCenter.M03 = 0.0f;
			orthographicOffCenter.M11 = (float)(2.0 / ((double)top - (double)bottom));
			orthographicOffCenter.M10 = orthographicOffCenter.M12 = orthographicOffCenter.M13 = 0.0f;
			orthographicOffCenter.M22 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
			orthographicOffCenter.M20 = orthographicOffCenter.M21 = orthographicOffCenter.M23 = 0.0f;
			orthographicOffCenter.M30 = (float)(((double)left + (double)right) / ((double)left - (double)right));
			orthographicOffCenter.M31 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
			orthographicOffCenter.M32 = zNearPlane / (zNearPlane - zFarPlane);
			orthographicOffCenter.M33 = 1f;
			return orthographicOffCenter;
		}

		/// <summary>
		/// 创建一个透视投影矩阵。
		/// </summary>
		/// <param name="width">宽</param>
		/// <param name="height">高</param>
		/// <param name="nearPlaneDistance">近平面</param>
		/// <param name="farPlaneDistance">原平面</param>
		public static Matrix4x4Float CreatePerspective(
		  float width,
		  float height,
		  float nearPlaneDistance,
		  float farPlaneDistance)
		{
			if ((double)nearPlaneDistance <= 0.0)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));
			if ((double)farPlaneDistance <= 0.0)
				throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));
			if ((double)nearPlaneDistance >= (double)farPlaneDistance)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));
			Matrix4x4Float perspective;
			perspective.M00 = 2f * nearPlaneDistance / width;
			perspective.M01 = perspective.M02 = perspective.M03 = 0.0f;
			perspective.M11 = 2f * nearPlaneDistance / height;
			perspective.M10 = perspective.M12 = perspective.M13 = 0.0f;
			perspective.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			perspective.M20 = perspective.M21 = 0.0f;
			perspective.M23 = -1f;
			perspective.M30 = perspective.M31 = perspective.M33 = 0.0f;
			perspective.M32 = (float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance));
			return perspective;
		}

		/// <summary>
		/// 创建一个透视投影矩阵
		/// </summary>
		/// <param name="left">左</param>
		/// <param name="right">右</param>
		/// <param name="bottom">底</param>
		/// <param name="top">顶</param>
		/// <param name="nearPlaneDistance">近平面</param>
		/// <param name="farPlaneDistance">原平面</param>
		public static Matrix4x4Float CreatePerspectiveOffCenter(
		  float left,
		  float right,
		  float bottom,
		  float top,
		  float nearPlaneDistance,
		  float farPlaneDistance)
		{
			if ((double)nearPlaneDistance <= 0.0)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));
			if ((double)farPlaneDistance <= 0.0)
				throw new ArgumentOutOfRangeException(nameof(farPlaneDistance));
			if ((double)nearPlaneDistance >= (double)farPlaneDistance)
				throw new ArgumentOutOfRangeException(nameof(nearPlaneDistance));
			Matrix4x4Float perspectiveOffCenter;
			perspectiveOffCenter.M00 = (float)(2.0 * (double)nearPlaneDistance / ((double)right - (double)left));
			perspectiveOffCenter.M01 = perspectiveOffCenter.M02 = perspectiveOffCenter.M03 = 0.0f;
			perspectiveOffCenter.M11 = (float)(2.0 * (double)nearPlaneDistance / ((double)top - (double)bottom));
			perspectiveOffCenter.M10 = perspectiveOffCenter.M12 = perspectiveOffCenter.M13 = 0.0f;
			perspectiveOffCenter.M20 = (float)(((double)left + (double)right) / ((double)right - (double)left));
			perspectiveOffCenter.M21 = (float)(((double)top + (double)bottom) / ((double)top - (double)bottom));
			perspectiveOffCenter.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			perspectiveOffCenter.M23 = -1f;
			perspectiveOffCenter.M32 = (float)((double)nearPlaneDistance * (double)farPlaneDistance / ((double)nearPlaneDistance - (double)farPlaneDistance));
			perspectiveOffCenter.M30 = perspectiveOffCenter.M31 = perspectiveOffCenter.M33 = 0.0f;
			return perspectiveOffCenter;
		}

		/// <summary>
		/// 创建观察矩阵
		/// </summary>
		/// <param name="cameraPosition">观察位置</param>
		/// <param name="cameraTarget">观察目标</param>
		/// <param name="cameraUpVector">观察者上方向量</param>
		public static Matrix4x4Float CreateLookAt(
		  Vector3Float cameraPosition,
		  Vector3Float cameraTarget,
		  Vector3Float cameraUpVector)
		{
			Vector3Float vector3_1 = (cameraPosition - cameraTarget).Normalized;
			Vector3Float vector3_2 = (cameraUpVector.Cross(vector3_1)).Normalized;
			Vector3Float vector1 = vector3_1.Cross(vector3_2);
			Matrix4x4Float lookAt;
			lookAt.M00 = vector3_2.X;
			lookAt.M01 = vector1.X;
			lookAt.M02 = vector3_1.X;
			lookAt.M03 = 0.0f;
			lookAt.M10 = vector3_2.Y;
			lookAt.M11 = vector1.Y;
			lookAt.M12 = vector3_1.Y;
			lookAt.M13 = 0.0f;
			lookAt.M20 = vector3_2.Z;
			lookAt.M21 = vector1.Z;
			lookAt.M22 = vector3_1.Z;
			lookAt.M23 = 0.0f;
			lookAt.M30 = -(vector3_2.Dot(cameraPosition));
			lookAt.M31 = -(vector1.Dot(cameraPosition));
			lookAt.M32 = -(vector3_1.Dot(cameraPosition));
			lookAt.M33 = 1f;
			return lookAt;
		}

		/// <summary>
		/// 创建一个坐标系的变换矩阵
		/// </summary>
		/// <param name="position">位置</param>
		/// <param name="forward">前方</param>
		/// <param name="up">上方</param>
		public static Matrix4x4Float CreateWorld(Vector3Float position, Vector3Float forward, Vector3Float up)
		{
			Vector3Float vector3_1 = -forward.Normalized;
			Vector3Float vector2 = (up.Cross(vector3_1)).Normalized;
			Vector3Float vector3_2 = vector3_1.Cross(vector2);
			Matrix4x4Float world;
			world.M00 = vector2.X;
			world.M01 = vector2.Y;
			world.M02 = vector2.Z;
			world.M03 = 0.0f;
			world.M10 = vector3_2.X;
			world.M11 = vector3_2.Y;
			world.M12 = vector3_2.Z;
			world.M13 = 0.0f;
			world.M20 = vector3_1.X;
			world.M21 = vector3_1.Y;
			world.M22 = vector3_1.Z;
			world.M23 = 0.0f;
			world.M30 = position.X;
			world.M31 = position.Y;
			world.M32 = position.Z;
			world.M33 = 1f;
			return world;
		}

		/// <summary>
		/// 创建阴影矩阵
		/// </summary>
		/// <param name="lightDirection">光照向量</param>
		/// <param name="plane">平面</param>
		public static Matrix4x4Float CreateShadow(Vector3Float lightDirection, PlaneFloat plane)
		{
			PlaneFloat plane1 = plane.Normalized;
			float num1 = (float)((double)plane1.Normal.X * (double)lightDirection.X + (double)plane1.Normal.Y * (double)lightDirection.Y + (double)plane1.Normal.Z * (double)lightDirection.Z);
			float num2 = -plane1.Normal.X;
			float num3 = -plane1.Normal.Y;
			float num4 = -plane1.Normal.Z;
			float num5 = -plane1.Distance;
			Matrix4x4Float shadow;
			shadow.M00 = num2 * lightDirection.X + num1;
			shadow.M10 = num3 * lightDirection.X;
			shadow.M20 = num4 * lightDirection.X;
			shadow.M30 = num5 * lightDirection.X;
			shadow.M01 = num2 * lightDirection.Y;
			shadow.M11 = num3 * lightDirection.Y + num1;
			shadow.M21 = num4 * lightDirection.Y;
			shadow.M31 = num5 * lightDirection.Y;
			shadow.M02 = num2 * lightDirection.Z;
			shadow.M12 = num3 * lightDirection.Z;
			shadow.M22 = num4 * lightDirection.Z + num1;
			shadow.M32 = num5 * lightDirection.Z;
			shadow.M03 = 0.0f;
			shadow.M13 = 0.0f;
			shadow.M23 = 0.0f;
			shadow.M33 = num1;
			return shadow;
		}

		/// <summary>
		/// 创建反射矩阵
		/// </summary>
		/// <param name="value">反射平面</param>
		public static Matrix4x4Float CreateReflection(PlaneFloat value)
		{
			value = value.Normalize();
			float x = value.Normal.X;
			float y = value.Normal.Y;
			float z = value.Normal.Z;
			float num1 = -2f * x;
			float num2 = -2f * y;
			float num3 = -2f * z;
			Matrix4x4Float reflection;
			reflection.M00 = (float)((double)num1 * (double)x + 1.0);
			reflection.M01 = num2 * x;
			reflection.M02 = num3 * x;
			reflection.M03 = 0.0f;
			reflection.M10 = num1 * y;
			reflection.M11 = (float)((double)num2 * (double)y + 1.0);
			reflection.M12 = num3 * y;
			reflection.M13 = 0.0f;
			reflection.M20 = num1 * z;
			reflection.M21 = num2 * z;
			reflection.M22 = (float)((double)num3 * (double)z + 1.0);
			reflection.M23 = 0.0f;
			reflection.M30 = num1 * value.Distance;
			reflection.M31 = num2 * value.Distance;
			reflection.M32 = num3 * value.Distance;
			reflection.M33 = 1f;
			return reflection;
		}

		/// <summary>
		/// 创建缩放矩阵。
		/// </summary>
		public static Matrix4x4Float Scale(Vector3Float vector)
		{
			Matrix4x4Float matrix4x4;
			matrix4x4.M00 = vector.X;
			matrix4x4.M01 = 0.0f;
			matrix4x4.M02 = 0.0f;
			matrix4x4.M03 = 0.0f;
			matrix4x4.M10 = 0.0f;
			matrix4x4.M11 = vector.Y;
			matrix4x4.M12 = 0.0f;
			matrix4x4.M13 = 0.0f;
			matrix4x4.M20 = 0.0f;
			matrix4x4.M21 = 0.0f;
			matrix4x4.M22 = vector.Z;
			matrix4x4.M23 = 0.0f;
			matrix4x4.M30 = 0.0f;
			matrix4x4.M31 = 0.0f;
			matrix4x4.M32 = 0.0f;
			matrix4x4.M33 = 1f;
			return matrix4x4;
		}

		/// <summary>
		/// 创建一个转换矩阵。
		/// </summary>
		public static Matrix4x4Float Translate(Vector3Float vector)
		{
			Matrix4x4Float matrix4x4;
			matrix4x4.M00 = 1f;
			matrix4x4.M01 = 0.0f;
			matrix4x4.M02 = 0.0f;
			matrix4x4.M03 = vector.X;
			matrix4x4.M10 = 0.0f;
			matrix4x4.M11 = 1f;
			matrix4x4.M12 = 0.0f;
			matrix4x4.M13 = vector.Y;
			matrix4x4.M20 = 0.0f;
			matrix4x4.M21 = 0.0f;
			matrix4x4.M22 = 1f;
			matrix4x4.M23 = vector.Z;
			matrix4x4.M30 = 0.0f;
			matrix4x4.M31 = 0.0f;
			matrix4x4.M32 = 0.0f;
			matrix4x4.M33 = 1f;
			return matrix4x4;
		}

		/// <summary>
		/// 创建一个旋转矩阵。
		/// </summary>
		public static Matrix4x4Float Rotate(QuaternionFloat q)
		{
			float num1 = q.X * 2f;
			float num2 = q.Y * 2f;
			float num3 = q.Z * 2f;
			float num4 = q.X * num1;
			float num5 = q.Y * num2;
			float num6 = q.Z * num3;
			float num7 = q.X * num2;
			float num8 = q.X * num3;
			float num9 = q.Y * num3;
			float num10 = q.W * num1;
			float num11 = q.W * num2;
			float num12 = q.W * num3;
			Matrix4x4Float matrix4x4;
			matrix4x4.M00 = (float)(1.0 - ((double)num5 + (double)num6));
			matrix4x4.M10 = num7 + num12;
			matrix4x4.M20 = num8 - num11;
			matrix4x4.M30 = 0.0f;
			matrix4x4.M01 = num7 - num12;
			matrix4x4.M11 = (float)(1.0 - ((double)num4 + (double)num6));
			matrix4x4.M21 = num9 + num10;
			matrix4x4.M31 = 0.0f;
			matrix4x4.M02 = num8 + num11;
			matrix4x4.M12 = num9 - num10;
			matrix4x4.M22 = (float)(1.0 - ((double)num4 + (double)num5));
			matrix4x4.M32 = 0.0f;
			matrix4x4.M03 = 0.0f;
			matrix4x4.M13 = 0.0f;
			matrix4x4.M23 = 0.0f;
			matrix4x4.M33 = 1f;
			return matrix4x4;
		}


		/// <summary>
		/// 创建一个转换矩阵
		/// </summary>
		/// <param name="pos">位置</param>
		/// <param name="q">旋转</param>
		/// <param name="s">缩放</param>
		public static Matrix4x4Float TRS(Vector3Float pos, QuaternionFloat q, Vector3Float s)
		{
			//计算旋转矩阵的行列式并求其倒数
			float det = 1f / (q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);

			//计算矩阵的第一列
			float x2 = q.X + q.X;
			float y2 = q.Y + q.Y;
			float z2 = q.Z + q.Z;

			float xx = q.X * x2 * det;
			float xy = q.X * y2 * det;
			float xz = q.X * z2 * det;
			float xw = q.W * x2 * det;

			float yy = q.Y * y2 * det;
			float yz = q.Y * z2 * det;
			float yw = q.W * y2 * det;

			float zz = q.Z * z2 * det;
			float zw = q.W * z2 * det;

			//计算变换矩阵
			Matrix4x4Float matrix = new Matrix4x4Float();
			matrix[0, 0] = (1f - yy - zz) * s.X;
			matrix[0, 1] = (xy - zw) * s.Y;
			matrix[0, 2] = (xz + yw) * s.Z;
			matrix[0, 3] = pos.X;

			matrix[1, 0] = (xy + zw) * s.X;
			matrix[1, 1] = (1f - xx - zz) * s.Y;
			matrix[1, 2] = (yz - xw) * s.Z;
			matrix[1, 3] = pos.Y;

			matrix[2, 0] = (xz - yw) * s.X;
			matrix[2, 1] = (yz + xw) * s.Y;
			matrix[2, 2] = (1f - xx - yy) * s.Z;
			matrix[2, 3] = pos.Z;

			matrix[3, 0] = 0f;
			matrix[3, 1] = 0f;
			matrix[3, 2] = 0f;
			matrix[3, 3] = 1f;

			return matrix;
		}

		/// <summary>
		/// 逆矩阵
		/// </summary>
		public static Matrix4x4Float GetInverse(Matrix4x4Float m)
		{
			Vector4Float c0 = m.GetColumn(0);
			Vector4Float c1 = m.GetColumn(1);
			Vector4Float c2 = m.GetColumn(2);
			Vector4Float c3 = m.GetColumn(3);

			Vector4Float r0y_r1y_r0x_r1x = new(c1.X, c1.Y, c0.X, c0.Y);
			Vector4Float r0z_r1z_r0w_r1w = new(c2.X, c2.Y, c3.X, c3.Y);
			Vector4Float r2y_r3y_r2x_r3x = new(c0.X, c0.Y, c1.X, c1.Y);
			Vector4Float r2z_r3z_r2w_r3w = new(c3.X, c3.Y, c2.X, c2.Y);

			Vector4Float r1y_r2y_r1x_r2x = new(c1.Y, c1.Z, c0.Y, c0.Z);
			Vector4Float r1z_r2z_r1w_r2w = new(c2.Y, c2.Z, c3.Y, c3.Z);
			Vector4Float r3y_r0y_r3x_r0x = new(c1.W, c1.X, c0.W, c0.X);
			Vector4Float r3z_r0z_r3w_r0w = new(c2.W, c2.X, c3.W, c3.X);

			Vector4Float r0_wzyx = new(r0z_r1z_r0w_r1w.Z, r0z_r1z_r0w_r1w.X, r0y_r1y_r0x_r1x.X, r0y_r1y_r0x_r1x.Z);
			Vector4Float r1_wzyx = new(r0z_r1z_r0w_r1w.W, r0z_r1z_r0w_r1w.Y, r0y_r1y_r0x_r1x.Y, r0y_r1y_r0x_r1x.W);
			Vector4Float r2_wzyx = new(r2z_r3z_r2w_r3w.Z, r2z_r3z_r2w_r3w.X, r2y_r3y_r2x_r3x.X, r2y_r3y_r2x_r3x.Z);
			Vector4Float r3_wzyx = new(r2z_r3z_r2w_r3w.W, r2z_r3z_r2w_r3w.Y, r2y_r3y_r2x_r3x.Y, r2y_r3y_r2x_r3x.W);
			Vector4Float r0_xyzw = new(r0y_r1y_r0x_r1x.Z, r0y_r1y_r0x_r1x.X, r0z_r1z_r0w_r1w.X, r0z_r1z_r0w_r1w.Z);

			// Calculate remaining inner term pairs. inner terms have zw=-xy, so we only have to calculate xy and can pack two pairs per vector.
			// 计算剩余的内部项对。内项有zw=-xy，所以我们只需要计算xy每个向量可以包含两对向量。
			Vector4Float inner12_23 = r1y_r2y_r1x_r2x * r2z_r3z_r2w_r3w - r1z_r2z_r1w_r2w * r2y_r3y_r2x_r3x;
			Vector4Float inner02_13 = r0y_r1y_r0x_r1x * r2z_r3z_r2w_r3w - r0z_r1z_r0w_r1w * r2y_r3y_r2x_r3x;
			Vector4Float inner30_01 = r3z_r0z_r3w_r0w * r0y_r1y_r0x_r1x - r3y_r0y_r3x_r0x * r0z_r1z_r0w_r1w;

			// Expand inner terms back to 4 components. zw signs still need to be flipped
			// 将内部项展开回4个分量。Zw标志仍然需要被翻转
			Vector4Float inner12 = new(inner12_23.X, inner12_23.Z, inner12_23.Z, inner12_23.X);
			Vector4Float inner23 = new(inner12_23.Y, inner12_23.W, inner12_23.W, inner12_23.Y);

			Vector4Float inner02 = new(inner02_13.X, inner02_13.Z, inner02_13.Z, inner02_13.X);
			Vector4Float inner13 = new(inner02_13.Y, inner02_13.W, inner02_13.W, inner02_13.Y);

			// Calculate minors
			Vector4Float minors0 = r3_wzyx * inner12 - r2_wzyx * inner13 + r1_wzyx * inner23;

			Vector4Float denom = r0_xyzw * minors0;

			// Horizontal sum of denominator. Free sign flip of z and w compensates for missing flip in inner terms.
			// 分母的水平和。z和w的自由符号翻转弥补了内部项中缺失的翻转。
			denom += new Vector4Float(denom.Y, denom.X, denom.W, denom.Z);   // x+y        x+y            z+w            z+w
			denom -= new Vector4Float(denom.Z, denom.Z, denom.X, denom.X);   // x+y-z-w  x+y-z-w        z+w-x-y        z+w-x-y

			Vector4Float rcp_denom_ppnn = Vector4Float.One / denom;
			Matrix4x4Float res = new();
			res.SetColumn(0, minors0 * rcp_denom_ppnn);

			Vector4Float inner30 = new(inner30_01.X, inner30_01.Z, inner30_01.Z, inner30_01.X);
			Vector4Float inner01 = new(inner30_01.Y, inner30_01.W, inner30_01.W, inner30_01.Y);

			Vector4Float minors1 = r2_wzyx * inner30 - r0_wzyx * inner23 - r3_wzyx * inner02;
			res.SetColumn(1, minors1 * rcp_denom_ppnn);

			Vector4Float minors2 = r0_wzyx * inner13 - r1_wzyx * inner30 - r3_wzyx * inner01;
			res.SetColumn(2, minors2 * rcp_denom_ppnn);

			Vector4Float minors3 = r1_wzyx * inner02 - r0_wzyx * inner12 + r2_wzyx * inner01;
			res.SetColumn(2, minors3 * rcp_denom_ppnn);

			return res;
		}


		/// <summary>
		/// 返回在空间中变换的平面。
		/// </summary>
		public PlaneFloat TransformPlane(PlaneFloat plane)
		{
			Matrix4x4Float inverse = this.Inverse;
			float x = plane.Normal.X;
			float y = plane.Normal.Y;
			float z = plane.Normal.Z;
			float distance = plane.Distance;
			return new PlaneFloat(new Vector3Float((float)((double)inverse.M00 * (double)x + (double)inverse.M10 * (double)y + (double)inverse.M20 * (double)z + (double)inverse.M30 * (double)distance), (float)((double)inverse.M01 * (double)x + (double)inverse.M11 * (double)y + (double)inverse.M21 * (double)z + (double)inverse.M31 * (double)distance), (float)((double)inverse.M02 * (double)x + (double)inverse.M12 * (double)y + (double)inverse.M22 * (double)z + (double)inverse.M32 * (double)distance)), (float)((double)inverse.M03 * (double)x + (double)inverse.M13 * (double)y + (double)inverse.M23 * (double)z + (double)inverse.M33 * (double)distance));
		}

		#endregion


	}
}
