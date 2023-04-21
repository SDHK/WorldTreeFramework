
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/18 19:37

* 描述： 感知器单元节点

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 感知器单元节点
    /// </summary>
    public class PerceptronNode : Node, IAwake, ChildOf<INode>
    {
        /// <summary>
        /// 上连接
        /// </summary>
        public TreeList<PerceptronLine> Links1;

        /// <summary>
        /// 下连接
        /// </summary>
        public TreeList<PerceptronLine> Links2;

        /// <summary>
        /// 误差增量
        /// </summary>
        public double delta = 0;

        /// <summary>
        /// 偏置项
        /// </summary>
        public double bias = 0;

        /// <summary>
        /// 结果
        /// </summary>
        public double result = 0;

        public override string ToString()
        {
            return $"{this.GetType().Name}\tDelta:[{delta}]\tBias:[{bias}]\tResult:[{result}]";
        }

        /// <summary>
        /// 正向传播计算
        /// </summary>
        public void ForwardPropagation()
        {
            if (Links1 != null)
            {
                double ThresholdResults = bias;
                for (int i = 0; i < Links1.Count; i++)
                {
                    ThresholdResults = (Links1[i].node1.result * Links1[i].weight) + ThresholdResults;
                }
                //通过 激活函数 拿到 0 到 1 的数值
                result = 1d / (Math.Exp(-ThresholdResults) + 1);
            }
        }

        /// <summary>
        /// 反向传播计算
        /// </summary>
        public void BackPropagation()
        {
            if (Links2 != null)
            {
                double error = 0;
                for (int i = 0; i < Links2.Count; i++)
                {
                    //误差 += 下级节点的 误差增量 * 权重 
                    error += Links2[i].node2.delta * Links2[i].weight;
                    Links2[i].BackPropagationWeight();
                }
                SetError(error);
            }
        }

        /// <summary>
        /// 设置误差
        /// </summary>
        public void SetError(double error)
        {
            //误差增量 = 斜率 * 误差。 斜率就是向谷底接近的速度
            delta = (result * (1d - result)) * error;
            bias += delta;
        }
    }

    class PerceptronNodeRemoveRule : RemoveRule<PerceptronNode>
    {
        public override void OnEvent(PerceptronNode self)
        {
            self.result = 0;
            self.delta = 0;
            self.Links1 = null;
            self.Links2 = null;
        }
    }

    public static class PerceptronNodeRule
    {
        /// <summary>
        /// 连接
        /// </summary>
        public static void Link(this PerceptronNode self, PerceptronNode node)
        {
            _ = self.Links2 ?? self.AddChild(out self.Links2);
            _ = node.Links1 ?? node.AddChild(out node.Links1);

            self.AddChild(out PerceptronLine perceptronLine, self, node);

            self.Links2.Add(perceptronLine);
            node.Links1.Add(perceptronLine);
        }
    }
}
