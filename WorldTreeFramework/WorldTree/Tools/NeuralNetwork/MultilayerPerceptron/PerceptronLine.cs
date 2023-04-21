
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
    public class PerceptronLine : Node, IAwake<PerceptronNode, PerceptronNode>, ChildOf<PerceptronNode>
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


        //权重+=上连接的结果*下连接的误差增量


        /// <summary>
        /// 权重变更
        /// </summary>
        public void BackPropagationWeight()
        {
            if (node1 is null || node2 is null) return;

            //需要偏置值！！！  注意：反向传播被权重影响，需要单独算
            weight += node1.result * node2.delta;
        }
    }
    class PerceptronLineAwakeRule : AwakeRule<PerceptronLine, PerceptronNode, PerceptronNode>
    {
        public override void OnEvent(PerceptronLine self, PerceptronNode node1, PerceptronNode node2)
        {
            self.node1 = node1;
            self.node2 = node2;
            self.weight = PerceptronLine.rand.NextDouble() * 2.0 - 1.0;
        }

    }
    class PerceptronLineRemoveRule : RemoveRule<PerceptronLine>
    {
        public override void OnEvent(PerceptronLine self)
        {
            self.weight = 0;
            self.node1 = null;
            self.node2 = null;
        }
    }
}
