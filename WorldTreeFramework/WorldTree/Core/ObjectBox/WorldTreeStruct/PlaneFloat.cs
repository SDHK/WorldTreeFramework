/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/8 14:32

* 描述： 

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 三维平面
    /// </summary>
    public partial struct PlaneFloat
    {
        internal const int size = 16;
        private Vector3Float m_Normal;
        private float m_Distance;


        /// <summary>
        /// 平面的法向量。
        /// </summary>
        public Vector3Float normal
        {
            get => this.m_Normal;
            set => this.m_Normal = value;
        }

        /// <summary>
        /// 沿平面法线从平面到世界原点的距离。
        /// </summary>
        public float distance
        {
            get => this.m_Distance;
            set => this.m_Distance = value;
        }


        /// <summary>
        /// 创建一个平面
        /// </summary>
        /// <param name="inNormal">法线向量</param>
        /// <param name="inPoint">平面上的一点</param>
        public PlaneFloat(Vector3Float inNormal, Vector3Float inPoint)
        {
            this.m_Normal = inNormal.normalized;
            this.m_Distance = -(this.m_Normal.Dot(inPoint));
        }

        /// <summary>
        /// 创建一个平面
        /// </summary>
        /// <param name="inNormal">法线向量</param>
        /// <param name="d">沿平面法线从平面到世界原点的距离</param>
        public PlaneFloat(Vector3Float inNormal, float d)
        {
            this.m_Normal = inNormal.normalized;
            this.m_Distance = d;
        }

        /// <summary>
        /// 创建一个平面
        /// </summary>
        public PlaneFloat(Vector3Float a, Vector3Float b, Vector3Float c)
        {
            this.m_Normal = ((b - a).Cross(c - a)).normalized;
            this.m_Distance = -(this.m_Normal.Dot(a));
        }

        /// <summary>
        /// 标准化平面
        /// </summary>
        public PlaneFloat normalized
        {
            get
            {
                return new PlaneFloat(this.normal, this.distance).Normalize();
            }
        }

        #region 方法

        /// <summary>
        /// 设置为标准化平面
        /// </summary>
        public PlaneFloat Normalize()
        {
            var sqrMagnitude = this.normal.sqrMagnitude;
            if ((double)Math.Abs(sqrMagnitude - 1f) < 1.1920928955078125E-07)
                return this;
            float magnitude = (float)Math.Sqrt((double)sqrMagnitude);
            this.normal /= magnitude;
            this.distance /= magnitude;
            return this;
        }


        /// <summary>
        /// 使用平面内的点和法线来设置平面的方向。
        /// </summary>
        public void SetNormalAndPosition(Vector3Float inNormal, Vector3Float inPoint)
        {
            this.m_Normal = inNormal.normalized;
            this.m_Distance = -(inNormal.Dot(inPoint));
        }

        /// <summary>
        /// 使用平面内的三个点设置平面。
        /// </summary>
        /// <remarks>当你往下看平面的上表面时，这些点是顺时针旋转的。</remarks>
        public void Set3Points(Vector3Float a, Vector3Float b, Vector3Float c)
        {
            this.m_Normal = ((b - a).Cross(c - a)).normalized;
            this.m_Distance = -(this.m_Normal.Dot(a));
        }

        /// <summary>
        /// 使平面朝向相反的方向。
        /// </summary>
        public void Flip()
        {
            this.m_Normal = -this.m_Normal;
            this.m_Distance = -this.m_Distance;
        }

        #endregion
    }



}
