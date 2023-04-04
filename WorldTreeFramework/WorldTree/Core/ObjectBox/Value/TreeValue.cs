/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 泛型树值类型

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
    {
        /// <summary>
        /// 法则执行器
        /// </summary>
        public IRuleActuator<IValueChangeRule<T>> RuleActuator;

        /// <summary>
        /// 值
        /// </summary>
        public virtual T Value { get; set; }
    }

    /// <summary>
    /// 泛型树值类型
    /// </summary>
    public class TreeValue<T> : TreeValueBase<T>, ChildOf<INode>
        where T : struct
    {
        private T value;
        public override T Value
        {
            get => value;

            set
            {
                if (!this.value.Equals(value))
                {
                    this.value = value;
                    RuleActuator?.Send(value);
                }
            }
        }
    }
}
