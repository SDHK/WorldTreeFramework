
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
    public class PerceptronNode : Node, ChildOf<INode>
        , AsChildBranch
        , AsAwake
    {
        /// <summary>
        /// 上连接
        /// </summary>
        public TreeList<PerceptronLine> Link1List;

        /// <summary>
        /// 下连接
        /// </summary>
        public TreeList<PerceptronLine> Link2List;

        /// <summary>
        /// 误差增量
        /// </summary>
        public double Delta = 0;

        /// <summary>
        /// 偏置项
        /// </summary>
        public double Bias = 0;

        /// <summary>
        /// 结果
        /// </summary>
        public double Result = 0;

        public override string ToString()
        {
            return $"{this.GetType().Name}\tDelta:[{Delta}]\tBias:[{Bias}]\tResult:[{Result}]";
        }
    }
}
