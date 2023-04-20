using System;
using UnityEditor.PackageManager;

namespace WorldTree
{
    /// <summary>
    /// 感知机连线
    /// </summary>
    public class PerceptronLine
    {
        /// <summary>
        /// 上连接
        /// </summary>
        PerceptronNode node1;

        /// <summary>
        /// 下连接
        /// </summary>
        PerceptronNode node2;

        /// <summary>
        /// 权重
        /// </summary>
        public double weight = 0;

        //权重+=上连接的结果*下连接的误差增量

    }


    /// <summary>
    /// 感知机单元节点
    /// </summary>
    public class PerceptronNode : Node, IAwake, ChildOf<INode>
    {
        /// <summary>
        /// 上连接
        /// </summary>
        public TreeList<PerceptronNode> List1;

        /// <summary>
        /// 下连接
        /// </summary>
        public TreeList<PerceptronNode> List2;

        /// <summary>
        /// 结果
        /// </summary>
        public double result = 0;
        /// <summary>
        /// 权重
        /// </summary>
        public double weight = 0;

        /// <summary>
        /// 误差增量
        /// </summary>
        public double delta = 0;


        public override string ToString()
        {
            return $"{this.GetType().Name}\tweight:[{weight}] \tdelta:[{delta}] \tresult:[{result}]";
        }

        public void ForwardPropagation()
        {
            if (List1 is null) //输入层
            {


            }
            else //隐藏层
            {
                double ThresholdResults = 0;
                for (int i = 0; i < List1.Count; i++)
                {
                    ThresholdResults += List1[i].result * List1[i].weight;
                }

                //通过 激活函数 拿到 0 到 1 的数值
                result = 1 / (1 + Math.Exp(-(ThresholdResults)));
            }


        }

        public void BackPropagation()
        {
            double error = 0;
            if (List2 is null) //输出层
            {
                error = weight - result;

            }
            else //隐藏层
            {
                for (int i = 0; i < List2.Count; i++)
                {
                    error += List2[i].delta * List2[i].weight;
                }

            }

            //误差增量 = 斜率 * 误差。 斜率就是向谷底接近的速度
            delta = (result * (1 - result)) * error;

        }

        /// <summary>
        /// 权重变更
        /// </summary>
        public void BackPropagationWeight()
        {
            if (List2 is null) return;
            //需要偏置值！！！  注意：反向传播被权重影响，需要单独算
            weight += delta * result;
        }
    }


    class PerceptronNodeAddRule : AddRule<PerceptronNode>
    {
        public override void OnEvent(PerceptronNode self)
        {
            self.weight = new Random().NextDouble() * 2.0 - 1.0;
        }

    }

    class PerceptronNodeRemoveRule : RemoveRule<PerceptronNode>
    {
        public override void OnEvent(PerceptronNode self)
        {
            self.result = 0;
            self.weight = 0;
            self.delta = 0;
            self.List1 = null;
            self.List2 = null;
        }

    }
}
