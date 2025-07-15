﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
    /// <summary>
    /// 解三角静态工具
    /// </summary>
    public static class MathTriangle
    {
        /// <summary>
        /// 解三角形 : 获得角A : 角A=180-(角B+角C) 同理
        /// </summary>
        /// <param name="angleB">角B</param>
        /// <param name="angleC">角C</param>
        /// <returns> 角A</returns>
        public static float GetAngleFromAngle(float angleB, float angleC)//获取角
        {
            return 180 - (angleB + angleC);
        }

        /// <summary>
        /// 解三角形 : 获得外接圆直径
        /// </summary>
        /// <param name="angle">角</param>
        /// <param name="edge">角的对边</param>
        /// <returns> 外接圆直径</returns>
        public static float GetDiameter(float angle, float edge)//获取外接圆直径
        {
            return edge / MathFloat.Sin(angle * MathFloat.DEG_RAD);//获取外接直径
        }

        /// <summary>
        /// 解三角形 : 获得角的对边 （只有乘法应该是最快的）
        /// </summary>
        /// <param name="angle">角</param>
        /// <param name="diameter">外接圆直径</param>
        /// <returns> 角的对边</returns>
        public static float GetEdgeFromDiameter(float angle, float diameter)//通过外接圆直径获得边
        {
            return MathFloat.Sin(angle * MathFloat.DEG_RAD) * diameter;//通过直径获得边
        }

        /// <summary>
        /// 解三角形 : 通过外接圆直径获得对角
        /// </summary>
        /// <param name="edge">角的对边</param>
        /// <param name="diameter">外接圆直径</param>
        /// <returns> 小于等于90度的对角</returns>
        public static float GetAngleFromDiameter(float edge, float diameter)//通过外接圆直径获得<=90的角
        {
            return MathFloat.Asin(edge / diameter) * MathFloat.RAD_DEG;//获得<=90的角
        }


        /// <summary>
        /// 解三角形 : 获得角:从三边获得一角
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="edgeC">角C的对边</param>
        /// <returns> 角A</returns>
        public static float GetAngleFromEdge(float edgeA, float edgeB, float edgeC)
        {
            return MathFloat.Acos((edgeB * edgeB + edgeC * edgeC - edgeA * edgeA) / (2 * (edgeB * edgeC))) * MathFloat.RAD_DEG;
        }

        /// <summary>
        /// 解三角形 : 获得夹角的对边(有开方耗性能)
        /// </summary>
        /// <param name="angleA">夹角A</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="edgeC">角C的对边</param>
        /// <returns> 夹角A的对边</returns>
        public static float GetEdgeFormAngle(float angleA, float edgeB, float edgeC)
        {

            return MathFloat.Sqrt(edgeB * edgeB + edgeC * edgeC - 2 * edgeB * edgeC * MathFloat.Cos(angleA * MathFloat.DEG_RAD));
        }

    }
}