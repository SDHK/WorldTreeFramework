
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
        , AsAwake
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
    }
}
