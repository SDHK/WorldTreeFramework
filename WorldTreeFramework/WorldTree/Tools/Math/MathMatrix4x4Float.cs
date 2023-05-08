/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/8 11:59

* 描述： 

*/

namespace WorldTree
{
    public static class MathMatrix4x4Float
    {
        /// <summary>
        /// 通过这个矩阵 变换一个位置。(一般)
        /// </summary>
        /// <param name="point"></param>
        public static Vector3Float MultiplyPoint(this Matrix4x4Float self, Vector3Float point)
        {
            Vector3Float vector3;
            vector3.x = (float)((double)self.m00 * (double)point.x + (double)self.m01 * (double)point.y + (double)self.m02 * (double)point.z) + self.m03;
            vector3.y = (float)((double)self.m10 * (double)point.x + (double)self.m11 * (double)point.y + (double)self.m12 * (double)point.z) + self.m13;
            vector3.z = (float)((double)self.m20 * (double)point.x + (double)self.m21 * (double)point.y + (double)self.m22 * (double)point.z) + self.m23;
            float num = 1f / ((float)((double)self.m30 * (double)point.x + (double)self.m31 * (double)point.y + (double)self.m32 * (double)point.z) + self.m33);
            vector3.x *= num;
            vector3.y *= num;
            vector3.z *= num;
            return vector3;
        }
        /// <summary>
        /// 通过这个矩阵变换位置(快速)。
        /// </summary>
        public static Vector3Float MultiplyPoint3x4(this Matrix4x4Float self, Vector3Float point)
        {
            Vector3Float vector3;
            vector3.x = (float)((double)self.m00 * (double)point.x + (double)self.m01 * (double)point.y + (double)self.m02 * (double)point.z) + self.m03;
            vector3.y = (float)((double)self.m10 * (double)point.x + (double)self.m11 * (double)point.y + (double)self.m12 * (double)point.z) + self.m13;
            vector3.z = (float)((double)self.m20 * (double)point.x + (double)self.m21 * (double)point.y + (double)self.m22 * (double)point.z) + self.m23;
            return vector3;
        }

        /// <summary>
        /// 用这个矩阵变换一个方向。
        /// </summary>
        public static Vector3Float MultiplyVector(this Matrix4x4Float self, Vector3Float vector)
        {
            Vector3Float vector3;
            vector3.x = (float)((double)self.m00 * (double)vector.x + (double)self.m01 * (double)vector.y + (double)self.m02 * (double)vector.z);
            vector3.y = (float)((double)self.m10 * (double)vector.x + (double)self.m11 * (double)vector.y + (double)self.m12 * (double)vector.z);
            vector3.z = (float)((double)self.m20 * (double)vector.x + (double)self.m21 * (double)vector.y + (double)self.m22 * (double)vector.z);
            return vector3;
        }



    }
}
