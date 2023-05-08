
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/20 11:26

* 描述： 三角形

*/


using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 三角形
    /// </summary>
    public partial struct Triangle : IEquatable<Triangle>
    {
        private float angleA;//角
        private float angleB;
        private float angleC;
        private float edgeA;//边
        private float edgeB;
        private float edgeC;
        private float diameter;//外接圆直径
        //=================================================
        /// <summary>
        /// 返回三角形的角A  [正数]
        /// </summary>
        public float AngleA { get { return angleA; } }
        /// <summary>
        /// 返回三角形的角B  [正数]
        /// </summary>
        public float AngleB { get { return angleB; } }
        /// <summary>
        /// 返回三角形的角C  [正数]
        /// </summary>
        public float AngleC { get { return angleC; } }

        /// <summary>
        /// 返回三角形的角A对边
        /// </summary>
        public float EdgeA { get { return edgeA; } }
        /// <summary>
        /// 返回三角形的角B对边
        /// </summary>
        public float EdgeB { get { return edgeB; } }
        /// <summary>
        /// 返回三角形的角C对边
        /// </summary>
        public float EdgeC { get { return edgeC; } }
        /// <summary>
        /// 返回三角形的外接圆直径
        /// </summary>
        public float Diameter { get { return diameter; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Triangle lhs, Triangle rhs)
        {
            return lhs.Equals(rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Triangle lhs, Triangle rhs) => !(lhs == rhs);

        /// <summary>
        /// 创建三角形
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="edgeC">角C的对边</param>
        /// <remarks> 提供 : 三角形的三个边 , 全解三角形</remarks>
        public static Triangle CreateEEE(float edgeA, float edgeB, float edgeC) => new Triangle().SetEEE(edgeA, edgeB, edgeC);

        /// <summary>
        /// 创建三角形
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="obtuseAngleB">角B优先解为钝角,一般为false</param>
        /// <remarks>提供 : 一个角和两个边 , 全解三角形</remarks>
        public static Triangle CreateAEE(float angleA, float edgeB, float edgeC, bool obtuseAngleB = false) => new Triangle().SetAEE(angleA, edgeB, edgeC, obtuseAngleB);

        /// <summary>
        /// 创建三角形
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="angleB">夹角B</param>
        /// <param name="edgeC">角C的对边</param>
        /// <remarks>提供 : 两个边和一个夹角 , 全解三角形</remarks>
        public static Triangle CreateEAE(float edgeA, float angleB, float edgeC) => new Triangle().SetEAE(edgeA, angleB, edgeC);

        /// <summary>
        /// 创建三角形
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="angleC">角C</param>
        /// <remarks>提供 : 两个角夹和一个边 , 全解三角形</remarks>
        public static Triangle CreateAEA(float angleA, float edgeB, float angleC) => new Triangle().SetAEA(angleA, edgeB, angleC);


        //=[三角形设置]=================================================
        /// <summary>
        /// 边边边
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="edgeC">角C的对边</param>
        /// <remarks> 提供 : 三角形的三个边 , 全解三角形</remarks>
        public Triangle SetEEE(float edgeA, float edgeB, float edgeC)
        {
            this.edgeA = edgeA;
            this.edgeB = edgeB;
            this.edgeC = edgeC;
            angleA = MathTriangle.GetAngleFromEdge(edgeA, edgeB, edgeC);
            angleB = MathTriangle.GetAngleFromEdge(edgeB, edgeA, edgeC);
            angleC = MathTriangle.GetAngleFromEdge(edgeC, edgeA, edgeB);

            return this;
        }
        /// <summary>
        /// 角边边 
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="obtuseAngleB">角B优先解为钝角,一般为false</param>
        /// <remarks>提供 : 一个角和两个边 , 全解三角形</remarks>
        public Triangle SetAEE(float angleA, float edgeA, float edgeB, bool obtuseAngleB = false)//1角2边解三角，钝角锐角都可解，obtuseAngleB钝角优先解
        {
            this.angleA = angleA;
            this.edgeA = edgeA;
            diameter = MathTriangle.GetDiameter(angleA, edgeA);

            angleB = MathTriangle.GetAngleFromDiameter(edgeB, diameter);//获得<=90的角
            angleC = 90 - MathTriangle.GetAngleFromAngle(90 - angleA, angleB);//绕一圈获得c角
            edgeC = MathTriangle.GetEdgeFromDiameter(angleC, diameter);//通过角获得边
            if (angleA <= 90 && obtuseAngleB)//b为钝角优先解
            {
                angleB = MathTriangle.GetAngleFromAngle(angleA, angleC);//角b为钝角重计算
            }
            else
            {
                angleC = MathTriangle.GetAngleFromAngle(angleA, angleB);//c为钝角优先解
                edgeC = MathTriangle.GetEdgeFromDiameter(angleC, diameter);//通过角获得边
            }
            return this;
        }
        /// <summary>
        /// 边角边 
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="angleB">夹角B</param>
        /// <param name="edgeC">角C的对边</param>
        /// <remarks>提供 : 两个边和一个夹角 , 全解三角形</remarks>
        public Triangle SetEAE(float edgeA, float angleB, float edgeC)
        {
            this.edgeA = edgeA;
            this.angleB = angleB;
            this.edgeC = edgeC;
            edgeB = MathTriangle.GetEdgeFormAngle(angleB, edgeA, edgeC);
            angleA = MathTriangle.GetAngleFromEdge(edgeA, edgeC, edgeB);
            angleC = MathTriangle.GetAngleFromEdge(edgeC, edgeA, edgeB);

            return this;
        }
        /// <summary>
        /// 角边角
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="angleC">角C</param>
        /// <remarks>提供 : 两个角夹和一个边 , 全解三角形</remarks>
        public Triangle SetAEA(float angleA, float edgeB, float angleC)
        {
            this.angleA = angleA;
            this.edgeB = edgeB;
            this.angleC = angleC;
            angleB = MathTriangle.GetAngleFromAngle(angleA, angleC);//暂时求出不定角
            diameter = MathTriangle.GetDiameter(angleB, edgeB);//获取外接直径
            edgeA = MathTriangle.GetEdgeFromDiameter(angleA, diameter);//通过角获得边
            edgeC = MathTriangle.GetEdgeFromDiameter(angleC, diameter);//通过角获得边

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(angleA, angleB, angleC, edgeA, edgeB, edgeC, diameter);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is Triangle other1 && this.Equals(other1);

        public bool Equals(Triangle other)
        {
            return this.edgeA == other.edgeA &&
                this.edgeB == other.edgeB &&
                this.edgeC == other.edgeC &&
                this.angleA == other.angleA &&
                this.angleB == other.angleB &&
                this.angleC == other.angleC &&
                this.diameter == other.diameter;
        }
    }


}

