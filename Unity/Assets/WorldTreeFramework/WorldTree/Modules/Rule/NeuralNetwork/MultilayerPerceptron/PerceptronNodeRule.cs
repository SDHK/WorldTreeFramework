
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 20:47

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class PerceptronNodeRule
    {
        class RemoveRule : RemoveRule<PerceptronNode>
        {
            protected override void OnEvent(PerceptronNode self)
            {
                self.result = 0;
                self.delta = 0;
                self.Links1 = null;
                self.Links2 = null;
            }
        }


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


        /// <summary>
        /// 正向传播计算
        /// </summary>
        public static void ForwardPropagation(this PerceptronNode self)
        {
            if (self.Links1 != null)
            {
                double ThresholdResults = self.bias;
                for (int i = 0; i < self.Links1.Count; i++)
                {
                    ThresholdResults = (self.Links1[i].node1.result * self.Links1[i].weight) + ThresholdResults;
                }
                //通过 激活函数 拿到 0 到 1 的数值
                self.result = 1d / (Math.Exp(-ThresholdResults) + 1);
            }
        }

        /// <summary>
        /// 反向传播计算
        /// </summary>
        public static void BackPropagation(this PerceptronNode self)
        {
            if (self.Links2 != null)
            {
                double error = 0;
                for (int i = 0; i < self.Links2.Count; i++)
                {
                    //误差 += 下级节点的 误差增量 * 权重 
                    error += self.Links2[i].node2.delta * self.Links2[i].weight;
                    self.Links2[i].BackPropagationWeight();
                }
                self.SetError(error);
            }
        }

        /// <summary>
        /// 设置误差
        /// </summary>
        public static void SetError(this PerceptronNode self, double error)
        {
            //误差增量 = 斜率 * 误差。 斜率就是向谷底接近的速度
            self.delta = (self.result * (1d - self.result)) * error;
            self.bias += self.delta;
        }

    }
}
