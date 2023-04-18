
using System;

namespace WorldTree
{
    public class Perceptron1 : Node, ChildOf<INode>, IAwake
    {
        /// <summary>
        /// 训练输入
        /// </summary>
        public TreeMatrix2<double> Inputs;

        /// <summary>
        /// 训练输出
        /// </summary>
        public TreeMatrix2<double> output;

        /// <summary>
        /// 权重0
        /// </summary>
        public TreeMatrix2<double> weight0;
        /// <summary>
        /// 权重1
        /// </summary>
        public TreeMatrix2<double> weight1;

        public void FP(TreeMatrix2<double> input, out TreeMatrix2<double> L1, out TreeMatrix2<double> L2)
        {
            var Dot = input.Dot(weight0);//矩阵相乘
            var Exp = Dot.Exp_();//指数幂  遍历 =Math.Exp(x）
            Dot.Dispose();
            var Add = Exp.Additive(1);//遍历+1
            Exp.Dispose();
            L1 = 1d.Division(Add);//遍历被1除
            Add.Dispose();

            //==========================================

            var Dot1 = L1.Dot(weight1);
            var Exp1 = Dot1.Exp_();
            Dot1.Dispose();
            var Add1 = Exp1.Additive(1);
            Exp1.Dispose();
            L2 = 1d.Division(Add1);
            Add1.Dispose();

        }

        public void BP(TreeMatrix2<double> l1, TreeMatrix2<double> l2, TreeMatrix2<double> y, out TreeMatrix2<double> L0delta, out TreeMatrix2<double> L1delta)
        {

            //误差
            var error = y.Subtraction(l2); //遍历减法

            var Subtraction = 1d.Subtraction_(l2); //遍历 被1减
            //斜率
            var slope = l2.Multiplication(Subtraction);// 单纯的遍历 相乘
            Subtraction.Dispose();

            //增量
            L1delta = error.Multiplication(slope); // 单纯的遍历 相乘

            error.Dispose();
            slope.Dispose();

            //==========================================

            //误差
            var Turn = weight1.Turn();  //xy遍历扭转 也就是 x，y = y，x
            var l0error = L1delta.Dot(Turn); //矩阵 相乘
            Turn.Dispose();

            Subtraction = 1d.Subtraction_(l1); //遍历 被1减
            //斜率
            var l0slope = l1.Multiplication(Subtraction); // 单纯的遍历 相乘
            Subtraction.Dispose();

            //增量
            L0delta = l0slope.Multiplication(l0error); // 单纯的遍历 相乘

            l0error.Dispose();
            l0slope.Dispose();
        }

        public void Test()
        {
            Inputs = this.AddChild(out TreeMatrix2<double> _, 8, 3);
            Inputs[0, 0] = 0; Inputs[0, 1] = 0; Inputs[0, 2] = 1;
            Inputs[1, 0] = 0; Inputs[1, 1] = 1; Inputs[1, 2] = 1;
            Inputs[2, 0] = 1; Inputs[2, 1] = 0; Inputs[2, 2] = 1;
            Inputs[3, 0] = 1; Inputs[3, 1] = 1; Inputs[3, 2] = 1;
            Inputs[4, 0] = 0; Inputs[4, 1] = 0; Inputs[4, 2] = 0;
            Inputs[5, 0] = 0; Inputs[5, 1] = 1; Inputs[5, 2] = 0;
            Inputs[6, 0] = 1; Inputs[6, 1] = 0; Inputs[6, 2] = 0;
            Inputs[7, 0] = 1; Inputs[7, 1] = 1; Inputs[7, 2] = 0;

            Inputs.Print();
            Inputs.Turn().Print();

            output = this.AddChild(out TreeMatrix2<double> _, 8, 1);
            output[0, 0] = 0;
            output[1, 0] = 1;
            output[2, 0] = 1;
            output[3, 0] = 0;
            output[4, 0] = 0;
            output[5, 0] = 1;
            output[6, 0] = 1;
            output[7, 0] = 0;


            Random rand = new Random();

            weight0 = this.AddChild(out TreeMatrix2<double> _, 3, 4);
            for (int x = 0; x < weight0.xLength; x++)
            {
                for (int y = 0; y < weight0.yLength; y++)
                {
                    weight0[x, y] = rand.NextDouble() * 2.0 - 1.0;
                }
            }


            weight1 = this.AddChild(out TreeMatrix2<double> _, 4, 1);
            for (int x = 0; x < weight0.xLength; x++)
            {
                for (int y = 0; y < weight0.yLength; y++)
                {
                    weight0[x, y] = rand.NextDouble() * 2.0 - 1.0;
                }
            }


        }

        public void Train()
        {
            for (int i = 0; i < 1000; i++)
            {
                FP(Inputs, out var l1, out var l2);//正向传播，拿到结果

                BP(l1, l2, output, out var L0delta, out var L1delta);//训练用的输出算出和结果，反向传播算出权重差值

                l2.Dispose();

                //L0delta.Print();
                //L1delta.Print();


                //根据权重差值改变权重

                var Turn1 = l1.Turn();
                l1.Dispose();

                var dot1 = Turn1.Dot(L1delta);
                L1delta.Dispose();
                Turn1.Dispose();

                weight1.Additive1(dot1);
                dot1.Dispose();

                //==========================================


                var Turn0 = Inputs.Turn();

                var dot0 = Turn0.Dot(L0delta);
                L0delta.Dispose();
                Turn0.Dispose();

                weight0.Additive1(dot0);
                dot0.Dispose();

            }
        }

        public void DY()
        {
            this.AddChild(out TreeMatrix2<double> Inputs1, 1, 3);
            Inputs1[0, 0] = 0; Inputs1[0, 1] = 1; Inputs1[0, 2] = 1;

            FP(Inputs1, out var l1, out var l2);//使用时就是只塞输入，的正向传播，l2就是结果。

            l1.Print();
            l2.Print();


            Inputs1.Dispose();
            l1.Dispose();
            l2.Dispose();


        }
    }
}
