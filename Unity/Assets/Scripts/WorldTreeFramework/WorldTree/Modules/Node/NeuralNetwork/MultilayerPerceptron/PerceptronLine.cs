
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/21 15:22

* 描述： 感知器连线

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 感知器连线
    /// </summary>
    public class PerceptronLine : Node, ChildOf<PerceptronNode>
        , AsAwake<PerceptronNode, PerceptronNode>
    {
		/// <summary>
		/// 随机数
		/// </summary>
		public static Random rand = new Random();

        /// <summary>
        /// 上连接
        /// </summary>
        public PerceptronNode Node1;

        /// <summary>
        /// 下连接
        /// </summary>
        public PerceptronNode Node2;

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight = 0;

        public override string ToString()
        {
            return $"{this.GetType().Name}\tweight:[{Weight}]";
        }
    }

}
