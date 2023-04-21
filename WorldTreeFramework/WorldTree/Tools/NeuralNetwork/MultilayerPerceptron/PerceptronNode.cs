
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
        public TreeList<PerceptronLine> List1;

        /// <summary>
        /// 下连接
        /// </summary>
        public TreeList<PerceptronLine> List2;

        /// <summary>
        /// 误差增量
        /// </summary>
        public double delta = 0;

        /// <summary>
        /// 偏置项
        /// </summary>
        public double Bias = 0;

        /// <summary>
        /// 结果
        /// </summary>
        public double result = 0;

        public override string ToString()
        {
            return $"{this.GetType().Name}\tDelta:[{delta}]\tBias:[{Bias}]\tResult:[{result}]";
        }

        /// <summary>
        /// 正向传播计算
        /// </summary>
        public void ForwardPropagation()
        {
            if (List1 != null)
            {
                double ThresholdResults = Bias;
                for (int i = 0; i < List1.Count; i++)
                {
                    ThresholdResults = (List1[i].node1.result * List1[i].weight) + ThresholdResults;
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
            if (List2 != null)
            {
                double error = 0;
                for (int i = 0; i < List2.Count; i++)
                {
                    //误差 += 下级节点的 误差增量 * 权重 
                    error += List2[i].node2.delta * List2[i].weight;
                    List2[i].BackPropagationWeight();
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
            Bias += delta;
        }
    }

    class PerceptronNodeRemoveRule : RemoveRule<PerceptronNode>
    {
        public override void OnEvent(PerceptronNode self)
        {
            self.result = 0;
            self.delta = 0;
            self.List1 = null;
            self.List2 = null;
        }
    }

    public static class PerceptronNodeRule
    {
        /// <summary>
        /// 连接
        /// </summary>
        public static void Link(this PerceptronNode self, PerceptronNode node)
        {
            _ = self.List2 ?? self.AddChild(out self.List2);
            _ = node.List1 ?? node.AddChild(out node.List1);

            self.AddChild(out PerceptronLine perceptronLine, self, node);

            self.List2.Add(perceptronLine);
            node.List1.Add(perceptronLine);
        }
    }
}
