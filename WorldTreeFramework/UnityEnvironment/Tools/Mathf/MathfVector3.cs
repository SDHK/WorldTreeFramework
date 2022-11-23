
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/17 11:59

* 描述： 3d向量数学计算静态类扩展

*/

using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 数学：3d向量
    /// </summary>
    public static class MathVector3
    {
        /// <summary>
        /// 2d向量转3d向量 对应到XZ
        /// </summary>
        public static Vector3 ToXZ(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);


        /// <summary>
        /// 判断是否平行线
        /// </summary>
        public static bool TryParallel(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Vector3.Cross(b - a, d - c) == Vector3.zero;
        }

        /// <summary>
        /// 判断点在线上
        /// </summary>
        public static bool TryPointOnLine(Vector3 node1, Vector3 node2, Vector3 point)
        {
            float maxX = node1.x > node2.x ? node1.x : node2.x;
            float minX = node1.x > node2.x ? node2.x : node1.x;
            float maxY = node1.y > node2.y ? node1.y : node2.y;
            float minY = node1.y > node2.y ? node2.y : node1.y;
            float maxZ = node1.z > node2.z ? node1.z : node2.z;
            float minZ = node1.z > node2.z ? node2.z : node1.z;

            Vector3 Vector1_P = point - node1;
            Vector3 Vector1_2 = node2 - node1;
            return Vector3.Dot(Vector1_P, Vector1_2) - Vector3.Dot(Vector1_2, Vector1_P) == 0 &&
                    (point.x >= minX && point.x <= maxX) &&
                    (point.y >= minY && point.y <= maxY) &&
                    (point.z >= minZ && point.z <= maxZ);
        }


        /// <summary>
        /// 快速排斥：true有可能相交，false 为排斥
        /// </summary>
        public static bool RapidRepulsion(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return Mathf.Min(a.x, b.x) <= Mathf.Max(c.x, d.x) ?
                    Mathf.Min(c.x, d.x) <= Mathf.Max(a.x, b.x) ?
                    Mathf.Min(a.y, b.y) <= Mathf.Max(c.y, d.y) ?
                    Mathf.Min(c.y, d.y) <= Mathf.Max(a.y, b.y) ?
                    Mathf.Min(a.z, b.z) <= Mathf.Max(c.z, d.z) ?
                    Mathf.Min(c.z, d.z) <= Mathf.Max(a.z, b.z) : false : false : false : false : false;
        }

        /// <summary>
        /// 跨立实验:只判断交叉，重叠点或线不算
        /// </summary>
        public static bool CrossExperiment(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPosition)
        {
            intersectPosition = Vector3.zero;
            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;
            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 判断 三角形acb法线 与 三角形abd法线 方向一致 和 三角形cad法线 与 三角形cbd法线 方向一致
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0 && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v1 = Vector3.Cross(ca, cd);
                Vector3 v2 = Vector3.Cross(cd, ab);

                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;//获得比值
                intersectPosition = a + ab * ratio;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取向量绕某轴旋转x度后的位置（三维）
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="center">旋转的中心点</param>
        /// <param name="axis">旋转的轴</param>
        /// <param name="angle">要旋转角度（顺时针为正）</param>
        /// <returns>旋转后的位置</returns>
        public static Vector3 RotateRound(Vector3 vector, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * vector; //算出旋转后的向量
            Vector3 resultVec3 = center + point;        //加上旋转中心位置得到旋转后的位置
            return resultVec3;
        }

        /// <summary>
        /// 三维向量转换为欧拉角：向量不能为0
        /// </summary>
        /// <param name="vector">指向向量</param>
        /// <returns>return : 欧拉角(360度)</returns>
        public static Vector3 ToEulerAngle(this Vector3 vector)
        {
            return Quaternion.LookRotation(vector).eulerAngles;
        }

        /// <summary>
        /// 欧拉角转换为三维向量
        /// </summary>
        /// <param name="eulerAngles">欧拉角</param>
        /// <returns>转换的三维向量</returns>
        public static Vector3 ToVector3(this Vector3 eulerAngles)
        {
            Vector3 vector = new Vector3(0, 0, 1);
            vector = Quaternion.AngleAxis(eulerAngles.x, -Vector3.left) * vector; //算出旋转后的向量
            vector = Quaternion.AngleAxis(eulerAngles.y, Vector3.up) * vector; //算出旋转后的向量

            return vector;
        }


        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        /// <param name="TimeRatio">点的位置（时间比例0~1）</param>
        /// <param name="points">曲线拉伸坐标点</param>
        /// <returns> 点的位置 </returns>
        public static Vector3 BezierCurve(List<Vector3> points, float TimeRatio)
        {
            while (points.Count > 1)
            {
                List<Vector3> newp = new List<Vector3>();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector3 p0p1 = Vector3.Lerp(points[i], points[i + 1], TimeRatio);
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
        public static Vector3 Center(List<Vector3> points)
        {
            Vector3 Center = new Vector3();
            for (int i = 0; i < points.Count; i++) Center += points[i];
            return Center / points.Count;
        }
    }
}
