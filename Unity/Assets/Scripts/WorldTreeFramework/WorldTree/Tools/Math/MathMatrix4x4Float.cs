/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/8 11:59

* 描述： 

*/

namespace WorldTree
{
	/// <summary>
	/// 4x4矩阵数学工具类
	/// </summary>
	public static class MathMatrix4x4Float
    {
        /// <summary>
        /// 通过这个矩阵 变换一个位置。(一般)
        /// </summary>
        /// <param name="point"></param>
        public static Vector3Float MultiplyPoint(this Matrix4x4Float self, Vector3Float point)
        {
            Vector3Float vector3;
            vector3.X = (float)((double)self.M00 * (double)point.X + (double)self.M01 * (double)point.Y + (double)self.M02 * (double)point.Z) + self.M03;
            vector3.Y = (float)((double)self.M10 * (double)point.X + (double)self.M11 * (double)point.Y + (double)self.M12 * (double)point.Z) + self.M13;
            vector3.Z = (float)((double)self.M20 * (double)point.X + (double)self.M21 * (double)point.Y + (double)self.M22 * (double)point.Z) + self.M23;
            float num = 1f / ((float)((double)self.M30 * (double)point.X + (double)self.M31 * (double)point.Y + (double)self.M32 * (double)point.Z) + self.M33);
            vector3.X *= num;
            vector3.Y *= num;
            vector3.Z *= num;
            return vector3;
        }
        /// <summary>
        /// 通过这个矩阵变换位置(快速)。
        /// </summary>
        public static Vector3Float MultiplyPoint3x4(this Matrix4x4Float self, Vector3Float point)
        {
            Vector3Float vector3;
            vector3.X = (float)((double)self.M00 * (double)point.X + (double)self.M01 * (double)point.Y + (double)self.M02 * (double)point.Z) + self.M03;
            vector3.Y = (float)((double)self.M10 * (double)point.X + (double)self.M11 * (double)point.Y + (double)self.M12 * (double)point.Z) + self.M13;
            vector3.Z = (float)((double)self.M20 * (double)point.X + (double)self.M21 * (double)point.Y + (double)self.M22 * (double)point.Z) + self.M23;
            return vector3;
        }

        /// <summary>
        /// 用这个矩阵变换一个方向。
        /// </summary>
        public static Vector3Float MultiplyVector(this Matrix4x4Float self, Vector3Float vector)
        {
            Vector3Float vector3;
            vector3.X = (float)((double)self.M00 * (double)vector.X + (double)self.M01 * (double)vector.Y + (double)self.M02 * (double)vector.Z);
            vector3.Y = (float)((double)self.M10 * (double)vector.X + (double)self.M11 * (double)vector.Y + (double)self.M12 * (double)vector.Z);
            vector3.Z = (float)((double)self.M20 * (double)vector.X + (double)self.M21 * (double)vector.Y + (double)self.M22 * (double)vector.Z);
            return vector3;
        }



    }
}
