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
        /// <summary>
        /// 大小
        /// </summary>
        internal const int SIZE = 16;

		/// <summary>
		/// 平面的法向量。
		/// </summary>
		private Vector3Float normal;

		/// <summary>
		/// 沿平面法线从平面到世界原点的距离。
		/// </summary>
		private float distance;


        /// <summary>
        /// 平面的法向量。
        /// </summary>
        public Vector3Float Normal
        {
            get => this.normal;
            set => this.normal = value;
        }

        /// <summary>
        /// 沿平面法线从平面到世界原点的距离。
        /// </summary>
        public float Distance
        {
            get => this.distance;
            set => this.distance = value;
        }


        /// <summary>
        /// 创建一个平面
        /// </summary>
        /// <param name="inNormal">法线向量</param>
        /// <param name="inPoint">平面上的一点</param>
        public PlaneFloat(Vector3Float inNormal, Vector3Float inPoint)
        {
            this.normal = inNormal.Normalized;
            this.distance = -(this.normal.Dot(inPoint));
        }

        /// <summary>
        /// 创建一个平面
        /// </summary>
        /// <param name="inNormal">法线向量</param>
        /// <param name="d">沿平面法线从平面到世界原点的距离</param>
        public PlaneFloat(Vector3Float inNormal, float d)
        {
            this.normal = inNormal.Normalized;
            this.distance = d;
        }

        /// <summary>
        /// 创建一个平面
        /// </summary>
        public PlaneFloat(Vector3Float a, Vector3Float b, Vector3Float c)
        {
            this.normal = ((b - a).Cross(c - a)).Normalized;
            this.distance = -(this.normal.Dot(a));
        }

        /// <summary>
        /// 标准化平面
        /// </summary>
        public PlaneFloat Normalized
        {
            get
            {
                return new PlaneFloat(this.Normal, this.Distance).Normalize();
            }
        }

        #region 方法

        /// <summary>
        /// 设置为标准化平面
        /// </summary>
        public PlaneFloat Normalize()
        {
            var sqrMagnitude = this.Normal.SqrMagnitude;
            if ((double)Math.Abs(sqrMagnitude - 1f) < 1.1920928955078125E-07)
                return this;
            float magnitude = (float)Math.Sqrt((double)sqrMagnitude);
            this.Normal /= magnitude;
            this.Distance /= magnitude;
            return this;
        }


        /// <summary>
        /// 使用平面内的点和法线来设置平面的方向。
        /// </summary>
        public void SetNormalAndPosition(Vector3Float inNormal, Vector3Float inPoint)
        {
            this.normal = inNormal.Normalized;
            this.distance = -(inNormal.Dot(inPoint));
        }

        /// <summary>
        /// 使用平面内的三个点设置平面。
        /// </summary>
        /// <remarks>当你往下看平面的上表面时，这些点是顺时针旋转的。</remarks>
        public void Set3Points(Vector3Float a, Vector3Float b, Vector3Float c)
        {
            this.normal = ((b - a).Cross(c - a)).Normalized;
            this.distance = -(this.normal.Dot(a));
        }

        /// <summary>
        /// 使平面朝向相反的方向。
        /// </summary>
        public void Flip()
        {
            this.normal = -this.normal;
            this.distance = -this.distance;
        }

        #endregion
    }



}
