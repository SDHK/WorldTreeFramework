
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/20 11:26

* 描述： 全解三角形

*/


namespace WorldTree
{

    /// <summary>
    /// 全解三角形 :需要用静态扩展分成ECS模式
    /// </summary>
    public class Triangle : Entity
    {
        private float angleA = 0;//角
        private float angleB = 0;
        private float angleC = 0;
        private float edgeA = 0;//边
        private float edgeB = 0;
        private float edgeC = 0;
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

        //=[三角形全解]=================================================
        /// <summary>
        /// 边边边 :
        /// 提供 : 三角形的三个边 , 全解三角形
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="edgeC">角C的对边</param>
        public Triangle SolutionsFromEEE(float edgeA, float edgeB, float edgeC)
        {
            this.edgeA = edgeA;
            this.edgeB = edgeB;
            this.edgeC = edgeC;
            angleA = TriangleTool.GetAngleFromEdge(edgeA, edgeB, edgeC);
            angleB = TriangleTool.GetAngleFromEdge(edgeB, edgeA, edgeC);
            angleC = TriangleTool.GetAngleFromEdge(edgeC, edgeA, edgeB);

            return this;
        }
        /// <summary>
        /// 角边边 :
        /// 提供 : 一个角和两个边 , 全解三角形
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="obtuseAngleB">角B优先解为钝角,一般为false</param>
        public Triangle SolutionsFromAEE(float angleA, float edgeA, float edgeB, bool obtuseAngleB=false)//1角2边解三角，钝角锐角都可解，obtuseAngleB钝角优先解
        {
            this.angleA = angleA;
            this.edgeA = edgeA;
            diameter = TriangleTool.GetDiameter(angleA, edgeA);

            angleB = TriangleTool.GetAngleFromDiameter(edgeB, diameter);//获得<=90的角
            angleC = 90 - TriangleTool.GetAngleFromAngle(90 - angleA, angleB);//绕一圈获得c角
            edgeC = TriangleTool.GetEdgeFromDiameter(angleC, diameter);//通过角获得边
            if (angleA <= 90 && obtuseAngleB)//b为钝角优先解
            {
                angleB = TriangleTool.GetAngleFromAngle(angleA, angleC);//角b为钝角重计算
            }
            else
            {
                angleC = TriangleTool.GetAngleFromAngle(angleA, angleB);//c为钝角优先解
                edgeC = TriangleTool.GetEdgeFromDiameter(angleC, diameter);//通过角获得边
            }
            return this;
        }
        /// <summary>
        /// 边角边 :
        /// 提供 : 两个边和一个夹角 , 全解三角形
        /// </summary>
        /// <param name="edgeA">角A的对边</param>
        /// <param name="angleB">夹角B</param>
        /// <param name="edgeC">角C的对边</param>
        public Triangle SolutionsFromEAE(float edgeA, float angleB, float edgeC)
        {
            this.edgeA = edgeA;
            this.angleB = angleB;
            this.edgeC = edgeC;
            edgeB = TriangleTool.GetEdgeFormAngle(angleB, edgeA, edgeC);
            angleA = TriangleTool.GetAngleFromEdge(edgeA, edgeC, edgeB);
            angleC = TriangleTool.GetAngleFromEdge(edgeC, edgeA, edgeB);

            return this;
        }
        /// <summary>
        /// 角边角 :
        /// 提供 : 两个角夹和一个边 , 全解三角形
        /// </summary>
        /// <param name="angleA">角A</param>
        /// <param name="edgeB">角B的对边</param>
        /// <param name="angleC">角C</param>
        public Triangle SolutionsFromAEA(float angleA, float edgeB, float angleC)
        {
            this.angleA = angleA;
            this.edgeB = edgeB;
            this.angleC = angleC;
            angleB = TriangleTool.GetAngleFromAngle(angleA, angleC);//暂时求出不定角
            diameter = TriangleTool.GetDiameter(angleB, edgeB);//获取外接直径
            edgeA = TriangleTool.GetEdgeFromDiameter(angleA, diameter);//通过角获得边
            edgeC = TriangleTool.GetEdgeFromDiameter(angleC, diameter);//通过角获得边

            return this;
        }

    }


}

