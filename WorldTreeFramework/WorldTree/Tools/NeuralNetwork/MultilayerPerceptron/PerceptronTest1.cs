namespace WorldTree
{


    /// <summary>
    /// 连接线
    /// </summary>
    public class PerceptronLine
    {
        public double Input;
        public double weight;
    }


    //输入单元和隐藏层单元
    public class PerceptronBase { }


    /// <summary>
    /// 感知机单元提炼1
    /// </summary>
    public class PerceptronTest1
    {


        /// <summary>
        /// 感知器连线
        /// </summary>
        public TreeList<PerceptronLine> Inputs;


        /// <summary>
        /// 输出
        /// </summary>
        public TreeValue<double> output;


    }


   


}
