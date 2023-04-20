/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/10 15:41

* 描述： 神经网络：感知机单元

*/


namespace WorldTree
{
    /// <summary>
    /// 感知机单元
    /// </summary>
    public class Perceptron : Node, ChildOf<INode>, IAwake
    {
        /// <summary>
        /// 训练输入
        /// </summary>
        public TreeMatrix2<double> Inputs;

        /// <summary>
        /// 权重
        /// </summary>
        public TreeMatrix2<double> weight;

        /// <summary>
        /// 训练输出
        /// </summary>
        public TreeMatrix2<double> output;

       

        public TreeMatrix2<double> FP(TreeMatrix2<double> input)
        {
            var Dot = input.Dot(weight);
            var Exp = Dot.Exp_();
            Dot.Dispose();
            var Add = Exp.Additive(1);
            Exp.Dispose();
            var Division = 1d.Division(Add);
            Add.Dispose();
            return Division;
        }

        public TreeMatrix2<double> BP(TreeMatrix2<double> y, TreeMatrix2<double> output)
        {
            var Division = 1d.Subtraction_(output);
            //斜率
            var slope = output.Multiplication(Division);
            Division.Dispose();

            //误差
            var error = y.Subtraction(output);

            //增量
            var data = error.Multiplication(slope);

            error.Dispose();
            slope.Dispose();
            return data;
        }

        public void Test()
        {
            Inputs = this.AddChild(out TreeMatrix2<double> _, 4, 3);
            Inputs[0, 0] = 0; Inputs[0, 1] = 0; Inputs[0, 2] = 1;
            Inputs[1, 0] = 0; Inputs[1, 1] = 1; Inputs[1, 2] = 1;
            Inputs[2, 0] = 1; Inputs[2, 1] = 0; Inputs[2, 2] = 1;
            Inputs[3, 0] = 1; Inputs[3, 1] = 1; Inputs[3, 2] = 1;

            Inputs.Print();
            Inputs.Turn().Print();

            output = this.AddChild(out TreeMatrix2<double> _, 4, 1);
            output[0, 0] = 0;
            output[1, 0] = 1;
            output[2, 0] = 1;
            output[3, 0] = 0;

            weight = this.AddChild(out TreeMatrix2<double> _, 3, 1);
            weight[0, 0] = 0; 
            weight[1, 0] = 0; 
            weight[2, 0] = 0;
        }

        public void Train()
        {
            for (int i = 0; i < 10000; i++)
            {
                var output_ = FP(Inputs);
                var delta = BP(output, output_);
              
                output_.Dispose();
                var Turn = Inputs.Turn();
                var Dot1 = Turn.Dot(delta);
                delta.Dispose();
                Turn.Dispose();

                weight.Additive1(Dot1);
                Dot1.Dispose();
            }
        }

        public void DY()
        {
            this.AddChild(out TreeMatrix2<double> Inputs1, 1, 3);
            Inputs1[0, 0] = 0; Inputs1[0, 1] = 0; Inputs1[0, 2] = 1;

            var outputR = FP(Inputs1);

            weight.Print();

            outputR.Print();


            Inputs1.Dispose();
            outputR.Dispose();
        }


    }
}
