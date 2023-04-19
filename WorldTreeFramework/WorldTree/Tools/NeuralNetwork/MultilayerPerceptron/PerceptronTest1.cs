using System;

namespace WorldTree
{

    /// <summary>
    /// 感知机单元提炼1
    /// </summary>
    public class PerceptronTest1 : PerceptronBase
    {
        /// <summary>
        /// 上连接
        /// </summary>
        public TreeList<PerceptronBase> List1;

        /// <summary>
        /// 下连接
        /// </summary>
        public TreeList<PerceptronBase> List2;

        public override void ForwardPropagation()
        {
            double ThresholdResults = 0;
            for (int i = 0; i < List1.Count; i++)
            {
                ThresholdResults += List1[i].result * List1[i].weight;
            }

            //通过 激活函数 拿到 0 到 1 的数值
            result = 1 / (1 + Math.Exp(-(ThresholdResults)));
        }

        public override void BackPropagation()
        {
            double error = 0;
            for (int i = 0; i < List2.Count; i++)
            {
                error += List2[i].delta * List2[i].weight;
            }
            //误差增量 = 斜率 * 误差。 斜率就是向谷底接近的速度
            delta = (1 * (1 - result)) * error;
        }

        /// <summary>
        /// 权重变更
        /// </summary>
        public void BackPropagationWeight()
        {
            //是否要偏置值？  注意：反向传播被权重影响，需要单独算
            weight += delta * result;
        }
    }




    /// <summary>
    /// 感知机接收单元
    /// </summary>
    public class PerceptronSurface1 : PerceptronBase
    {

    }



    //输入单元和隐藏层单元
    public abstract class PerceptronBase
    {

        /// <summary>
        /// 结果
        /// </summary>
        public double result;
        /// <summary>
        /// 权重
        /// </summary>
        public double weight;

        /// <summary>
        /// 误差增量
        /// </summary>
        public double delta;


        public double ForwardPropagationCalculation()
        {
            return 1 / (1 + Math.Exp(-(result * weight)));
        }

        /// <summary>
        /// 正向传播
        /// </summary>
        public virtual void ForwardPropagation() { }

        /// <summary>
        /// 反向传播
        /// </summary>
        public virtual void BackPropagation() { }
    }







}
