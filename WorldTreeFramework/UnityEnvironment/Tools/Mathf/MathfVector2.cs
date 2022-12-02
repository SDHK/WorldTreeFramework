
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/17 11:46

* 描述： 2d向量数学计算静态工具类

*/

using System.Collections.Generic;
using UnityEngine;


namespace WorldTree
{
    /// <summary>
    /// 数学：2d向量
    /// </summary>
    public static class MathVector2
    {

        /// <summary>
        /// 判断1维向量相等：向量并不是直接==判断
        /// </summary>
        public static bool Vector1Equal(this float v1, float v2) => Mathf.Pow(v1 - v2, 2) < 9.99999944E-11f;

        /// <summary>
        /// 3d向量转2d向量 只取XY
        /// </summary>
        public static Vector2 ToXY(this Vector3 vector) => new Vector2(vector.x, vector.y);
        /// <summary>
        /// 3d向量转2d向量 只取XZ
        /// </summary>
        public static Vector2 ToXZ(this Vector3 vector) => new Vector2(vector.x, vector.z);

        /// <summary>
        /// 2d向量叉乘:通过x,y 算出z轴
        /// </summary>
        public static float Cross(this Vector2 vector, Vector2 vector2) => vector.x * vector2.y - vector2.x * vector.y;

        /// <summary>
        /// 2d向量投影
        /// </summary>
        public static Vector2 Project(this Vector2 vector, Vector2 onNormal)
        {
            float num = Vector2.Dot(onNormal, onNormal);
            if (num < Mathf.Epsilon)
            {
                return Vector2.zero;
            }
            float num2 = Vector2.Dot(vector, onNormal);
            return new Vector2(onNormal.x * num2 / num, onNormal.y * num2 / num);
        }

        /// <summary>
        /// 判断点在线上
        /// </summary>
        public static bool TryPointOnLine(Vector2 node1, Vector2 node2, Vector2 point)
        {
            float maxX, minX, maxY, minY;
            maxX = node1.x > node2.x ? node1.x : node2.x;
            minX = node1.x > node2.x ? node2.x : node1.x;
            maxY = node1.y > node2.y ? node1.y : node2.y;
            minY = node1.y > node2.y ? node2.y : node1.y;

            Vector2 Vector1_P = (point - node1).normalized;
            Vector2 Vector2_P = (point - node2).normalized;
            Vector2 Vector1_2 = (node2 - node1).normalized;

            return Vector1_P == Vector1_2 && Vector2_P == -Vector1_2 &&

                    (point.x >= minX && point.x <= maxX) &&
                    (point.y >= minY && point.y <= maxY);
        }

        /// <summary>
        /// 判断是否平行线
        /// </summary>
        public static bool TryParallel(Vector2 a, Vector2 b, Vector2 c, Vector2 d) => (b - a).Cross(d - c).Vector1Equal(0);

        /// <summary>
        /// 快速排斥：true有可能相交，false 为排斥
        /// </summary>
        public static bool RapidRepulsion(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return Mathf.Min(a.x, b.x) <= Mathf.Max(c.x, d.x) ?
                    Mathf.Min(c.y, d.y) <= Mathf.Max(a.y, b.y) ?
                    Mathf.Min(c.x, d.x) <= Mathf.Max(a.x, b.x) ?
                    Mathf.Min(a.y, b.y) <= Mathf.Max(c.y, d.y) : false : false : false;
        }

        /// <summary>
        /// 跨立实验:只判断交叉，重叠点或线不算
        /// </summary>
        public static bool CrossExperiment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersectPosition)
        {
            intersectPosition = Vector2.zero;
            Vector2 ab = b - a;
            Vector2 ca = a - c;
            Vector2 cd = d - c;
            Vector2 ad = d - a;
            Vector2 cb = b - c;
            if (Cross(-ca, ab) * Cross(ab, ad) > 0 && Cross(ca, cd) * Cross(cd, cb) > 0)
            {
                float v1 = Cross(ca, cd);
                float v2 = Cross(cd, ab);
                float ratio = (v1 * v2) / (v2 * v2);//获得比值

                intersectPosition = a + ab * ratio;
                return true;
            }
            return false;

        }

        /// <summary>
        /// 获取点绕某轴旋转x度后的位置（二维）
        /// </summary>
        /// <param name="position">要旋转的点</param>
        /// <param name="center">旋转的中心点</param>
        /// <param name="angle">一次旋转多少角度（顺时针为正）</param>
        /// <returns>旋转后的位置</returns>
        public static Vector2 RotateRound(Vector2 vector, Vector2 center, float angle)
        {
            Vector2 point = Quaternion.AngleAxis(angle, Vector3.forward) * vector; //算出旋转后的向量
            Vector2 result = center + point;        //加上旋转中心位置得到旋转后的位置
            return result;
        }

        /// <summary>
        /// 角度转向量
        /// </summary>
        public static Vector2 ToVector2(this float angle) => new Vector2(Mathf.Sin(angle / Mathf.Rad2Deg), Mathf.Cos(angle / Mathf.Rad2Deg));

        /// <summary>
        /// 向量转角度 +-180
        /// </summary>
        public static float ToAngle(this Vector2 vector) => Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;



        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        /// <param name="TimeRatio">点的位置（时间比例0~1）</param>
        /// <param name="points">曲线拉伸坐标点</param>
        /// <returns> 点的位置 </returns>
        public static Vector2 BezierCurve(List<Vector2> points, float TimeRatio)
        {
            while (points.Count > 1)
            {
                List<Vector2> newp = new List<Vector2>();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 p0p1 = Vector2.Lerp(points[i], points[i + 1], TimeRatio);
                    newp.Add(p0p1);
                }
                points = newp;
            }
            return points[0];
        }


        /// <summary>
        /// 计算多点中心
        /// </summary>
        /// <param name="points">多点位置集合</param>
        /// <returns>中心点</returns>
        public static Vector2 VectorsCenter(List<Vector2> points)
        {
            Vector2 Center = new Vector2();
            for (int i = 0; i < points.Count; i++) Center += points[i];
            return Center / points.Count;
        }


    }

}
