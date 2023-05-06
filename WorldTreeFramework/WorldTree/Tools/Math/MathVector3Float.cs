
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/17 11:59

* 描述： 3d向量数学计算静态类扩展

*/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 3d向量
    /// </summary>
    public static class MathVector3Float
    {
        /// <summary>
        /// 返回最小值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float Min(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));

        /// <summary>
        /// 返回最大值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float Max(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));

        /// <summary>
        /// 返回a和b之间的距离。</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector3Float a, Vector3Float b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2 + (double)num3 * (double)num3);
        }

        /// <summary>
        /// 返回vector的副本，其大小限制为maxLength。</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float ClampMagnitude(this Vector3Float vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;
            if ((double)sqrMagnitude <= (double)maxLength * (double)maxLength)
                return vector;
            float num1 = (float)Math.Sqrt((double)sqrMagnitude);
            float num2 = vector.x / num1;
            float num3 = vector.y / num1;
            float num4 = vector.z / num1;
            return new Vector3Float(num2 * maxLength, num3 * maxLength, num4 * maxLength);
        }


        /// <summary>
        /// 将向量反射出由法线定义的平面。
        /// </summary>
        /// <param name="inDirection">入射向量</param>
        /// <param name="inNormal">平面法线向量</param>
        /// <returns>反射向量</returns>
        public static Vector3Float Reflect(this Vector3Float inDirection, Vector3Float inNormal)
        {
            float num = -2f * inNormal.Dot(inDirection);
            return new Vector3Float(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
        }

        /// <summary>
        /// 两个向量的点积。
        /// </summary>
        public static float Dot(this Vector3Float lhs, Vector3Float rhs) => (float)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y + (double)lhs.z * (double)rhs.z);

        /// <summary>
        /// 两个向量的外积。
        /// </summary>
        public static Vector3Float Cross(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float((float)((double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), (float)((double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), (float)((double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x));


        /// <summary>
        /// 将一个向量投影到另一个向量上。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float Project(this Vector3Float vector, Vector3Float onNormal)
        {
            float num1 = onNormal.Dot(onNormal);
            if (num1 < Vector3Float.kEpsilon)
                return Vector3Float.Zero;
            float num2 = vector.Dot(onNormal);
            return new Vector3Float(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
        }

        /// <summary>
        /// 将一个向量投影到一个平面上，这个平面由一个正交于平面的法线定义。
        /// </summary>
        /// <param name="vector">向量在平面上的位置。</param>
        /// <param name="planeNormal">向量指向平面的方向</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Float ProjectOnPlane(this Vector3Float vector, Vector3Float planeNormal)
        {
            float num1 = planeNormal.Dot(planeNormal);
            if (num1 < MathFloat.Epsilon)
                return vector;
            float num2 = vector.Dot(planeNormal);
            return new Vector3Float(vector.x - planeNormal.x * num2 / num1, vector.y - planeNormal.y * num2 / num1, vector.z - planeNormal.z * num2 / num1);
        }

        /// <summary>
        /// 计算和向量之间的夹角。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector3Float from, Vector3Float to)
        {
            float num = (float)Math.Sqrt((double)from.sqrMagnitude * (double)to.sqrMagnitude);
            return (double)num < 1.0000000036274937E-15 ? 0.0f : (float)Math.Acos((double)Math.Clamp(from.Dot(to) / num, -1f, 1f)) * 57.29578f;
        }

        /// <summary>
        /// 计算向量from和to之间相对于轴的带符号角度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(this Vector3Float from, Vector3Float to, Vector3Float axis)
        {
            float num1 = from.Angle(to);
            float num2 = (float)((double)from.y * (double)to.z - (double)from.z * (double)to.y);
            float num3 = (float)((double)from.z * (double)to.x - (double)from.x * (double)to.z);
            float num4 = (float)((double)from.x * (double)to.y - (double)from.y * (double)to.x);
            float num5 = Math.Sign((float)((double)axis.x * (double)num2 + (double)axis.y * (double)num3 + (double)axis.z * (double)num4));
            return num1 * num5;
        }


        //======================================


        /// <summary>
        /// 判断是否平行线
        /// </summary>
        public static bool TryParallel(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d)
        {
            return (b - a).Cross(d - c) == Vector3Float.Zero;
        }

        /// <summary>
        /// 判断点在线上
        /// </summary>
        public static bool TryPointOnLine(Vector3Float node1, Vector3Float node2, Vector3Float point)
        {
            float maxX = node1.x > node2.x ? node1.x : node2.x;
            float minX = node1.x > node2.x ? node2.x : node1.x;
            float maxY = node1.y > node2.y ? node1.y : node2.y;
            float minY = node1.y > node2.y ? node2.y : node1.y;
            float maxZ = node1.z > node2.z ? node1.z : node2.z;
            float minZ = node1.z > node2.z ? node2.z : node1.z;

            Vector3Float Vector1_P = point - node1;
            Vector3Float Vector1_2 = node2 - node1;
            return Vector1_P.Dot(Vector1_2) - Vector1_2.Dot(Vector1_P) == 0 &&
                    (point.x >= minX && point.x <= maxX) &&
                    (point.y >= minY && point.y <= maxY) &&
                    (point.z >= minZ && point.z <= maxZ);
        }


        /// <summary>
        /// 快速排斥：true有可能相交，false 为排斥
        /// </summary>
        public static bool RapidRepulsion(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d)
        {
            return Math.Min(a.x, b.x) <= Math.Max(c.x, d.x) ?
                    Math.Min(c.x, d.x) <= Math.Max(a.x, b.x) ?
                    Math.Min(a.y, b.y) <= Math.Max(c.y, d.y) ?
                    Math.Min(c.y, d.y) <= Math.Max(a.y, b.y) ?
                    Math.Min(a.z, b.z) <= Math.Max(c.z, d.z) ?
                    Math.Min(c.z, d.z) <= Math.Max(a.z, b.z) : false : false : false : false : false;
        }

        /// <summary>
        /// 跨立实验:只判断交叉，重叠点或线不算
        /// </summary>
        public static bool CrossExperiment(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d, out Vector3Float intersectPosition)
        {
            intersectPosition = Vector3Float.Zero;
            Vector3Float ab = b - a;
            Vector3Float ca = a - c;
            Vector3Float cd = d - c;
            Vector3Float ad = d - a;
            Vector3Float cb = b - c;
            // 判断 三角形acb法线 与 三角形abd法线 方向一致 和 三角形cad法线 与 三角形cbd法线 方向一致
            if (-ca.Cross(ab).Dot(ab.Cross(ad)) > 0 && ca.Cross(cd).Dot(cd.Cross(cb)) > 0)
            {
                Vector3Float v1 = ca.Cross(cd);
                Vector3Float v2 = cd.Cross(ab);

                float ratio = v1.Dot(v2) / v2.sqrMagnitude;//获得比值
                intersectPosition = a + ab * ratio;
                return true;
            }
            return false;
        }

        public static Vector3Float Lerp(this Vector3Float a, Vector3Float b, float t)
        {
            t = Math.Clamp(t, 0, 1);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        /// <param name="TimeRatio">点的位置（时间比例0~1）</param>
        /// <param name="points">曲线拉伸坐标点</param>
        /// <returns> 点的位置 </returns>
        public static Vector3Float BezierCurve(TreeList<Vector3Float> points, float TimeRatio)
        {
            while (points.Count > 1)
            {
                points.Parent.AddChild(out TreeList<Vector3Float> newp);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector3Float p0p1 = points[i].Lerp(points[i + 1], TimeRatio);
                    newp.Add(p0p1);
                }
                points.Dispose();
                points = newp;
            }
            var point = points[0];
            points.Dispose();
            return point;
        }

        /// <summary>
        /// 计算多点中心
        /// </summary>
        /// <param name="points">多点位置集合</param>
        /// <returns>中心点</returns>
        public static Vector3Float Center(TreeList<Vector3Float> points)
        {
            Vector3Float Center = new Vector3Float();
            for (int i = 0; i < points.Count; i++) Center += points[i];
            return Center / points.Count;
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        /// <summary>
        /// 获取向量绕某轴旋转x度后的位置（三维）
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="center">旋转的中心点</param>
        /// <param name="axis">旋转的轴</param>
        /// <param name="angle">要旋转角度（顺时针为正）</param>
        /// <returns>旋转后的位置</returns>
        public static Vector3Float RotateRound(Vector3Float vector, Vector3Float center, Vector3Float axis, float angle)
        {
            Vector3Float point = Quaternion.AngleAxis(angle, axis) * vector; //算出旋转后的向量
            Vector3Float resultVec3 = center + point;        //加上旋转中心位置得到旋转后的位置
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


        //四元数计算源码？
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            float halfAngle = angle * 0.5f;
            float s = Mathf.Sin(halfAngle * Mathf.Deg2Rad);
            return new Quaternion(axis.x * s, axis.y * s, axis.z * s, Mathf.Cos(halfAngle * Mathf.Deg2Rad));
        }
    }
}
