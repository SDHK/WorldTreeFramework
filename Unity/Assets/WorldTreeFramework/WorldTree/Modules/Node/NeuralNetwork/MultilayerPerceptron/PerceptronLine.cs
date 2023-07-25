
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
        , AsRule<IAwakeRule<PerceptronNode, PerceptronNode>>
    {
        public static Random rand = new Random();

        /// <summary>
        /// 上连接
        /// </summary>
        public PerceptronNode node1;

        /// <summary>
        /// 下连接
        /// </summary>
        public PerceptronNode node2;

        /// <summary>
        /// 权重
        /// </summary>
        public double weight = 0;

        public override string ToString()
        {
            return $"{this.GetType().Name}\tweight:[{weight}]";
        }

        /// <summary>
        /// 权重变更
        /// </summary>
        public void BackPropagationWeight()
        {
            if (node1 is null || node2 is null) return;

            //权重+=上连接的结果*下连接的误差增量
            weight += node1.result * node2.delta;
        }
    }

}
