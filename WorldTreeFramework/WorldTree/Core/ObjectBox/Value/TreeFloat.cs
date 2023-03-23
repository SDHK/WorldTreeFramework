/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:55

* 描述： 树节点float类型

*/

namespace WorldTree
{

    /// <summary>
    /// 树节点float类型
    /// </summary>
    public class TreeFloat : Node, ChildOf<INode>
    {
        /// <summary>
        /// 值
        /// </summary>
        public float value;

        /// <summary>
        /// 法则执行器
        /// </summary>
        public RuleActuator ruleActuator;
    }
}
