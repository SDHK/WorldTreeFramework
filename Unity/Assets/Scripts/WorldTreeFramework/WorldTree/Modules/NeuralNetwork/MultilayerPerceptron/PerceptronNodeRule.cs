/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
using System;

namespace WorldTree
{
    public static class PerceptronNodeRule
    {
        class RemoveRule : RemoveRule<PerceptronNode>
        {
            protected override void Execute(PerceptronNode self)
            {
                self.Result = 0;
                self.Delta = 0;
                self.Link1List = null;
                self.Link2List = null;
            }
        }


        /// <summary>
        /// 连接
        /// </summary>
        public static void Link(this PerceptronNode self, PerceptronNode node)
        {
            _ = self.Link2List ?? self.AddChild(out self.Link2List);
            _ = node.Link1List ?? node.AddChild(out node.Link1List);

            self.AddChild(out PerceptronLine perceptronLine, self, node);

            self.Link2List.Add(perceptronLine);
            node.Link1List.Add(perceptronLine);
        }


        /// <summary>
        /// 正向传播计算
        /// </summary>
        public static void ForwardPropagation(this PerceptronNode self)
        {
            if (self.Link1List != null)
            {
                double thresholdResults = self.Bias;
                for (int i = 0; i < self.Link1List.Count; i++)
                {
                    thresholdResults = (self.Link1List[i].Node1.Result * self.Link1List[i].Weight) + thresholdResults;
                }
                //通过 激活函数 拿到 0 到 1 的数值
                self.Result = 1d / (Math.Exp(-thresholdResults) + 1);
            }
        }

        /// <summary>
        /// 反向传播计算
        /// </summary>
        public static void BackPropagation(this PerceptronNode self)
        {
            if (self.Link2List != null)
            {
                double error = 0;
                for (int i = 0; i < self.Link2List.Count; i++)
                {
                    //误差 += 下级节点的 误差增量 * 权重 
                    error += self.Link2List[i].Node2.Delta * self.Link2List[i].Weight;
                    self.Link2List[i].BackPropagationWeight();
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
            self.Delta = (self.Result * (1d - self.Result)) * error;
            self.Bias += self.Delta;
        }

    }
}
