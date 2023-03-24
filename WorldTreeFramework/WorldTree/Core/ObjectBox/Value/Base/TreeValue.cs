
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValue : Node, ChildOf<INode>
    {
        /// <summary>
        /// 法则执行器
        /// </summary>
        public RuleActuator ruleActuator;

        /// <summary>
        /// 全局法则类型
        /// </summary>
        public Type RuleType;
    }
}
