
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 10:34

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree 
{
    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValueBase : Node
    {
        /// <summary>
        /// 全局广播法则类型
        /// </summary>
        public Type GlobalRuleType;
    }


    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValueBase<T> : TreeValueBase
        where T : struct
    {
        /// <summary>
        /// 全局法则执行器
        /// </summary>
        public IRuleActuator<IValueChangeRule<T>> m_GlobalRuleActuator;
        /// <summary>
        /// 法则执行器
        /// </summary>
        public IRuleActuator<IValueChangeRule<T>> m_RuleActuator;

        /// <summary>
        /// 值
        /// </summary>
        public virtual T Value { get; set; }

    }
}
