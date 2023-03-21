/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 树节点Int类型

*/

namespace WorldTree
{
    /// <summary>
    /// 树节点int类型
    /// </summary>
    public class TreeInt : Node, ChildOfNode
    {
        /// <summary>
        /// 值
        /// </summary>
        public int value;

        /// <summary>
        /// 法则执行器
        /// </summary>
        public RuleActuator ruleActuator;
    }


}
