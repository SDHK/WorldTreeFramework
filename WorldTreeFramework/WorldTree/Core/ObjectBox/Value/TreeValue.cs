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
    public interface ITreeValue : INode
    {
        /// <summary>
        /// 全局广播法则类型
        /// </summary>
        public Type type { get; set; }
    }


    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public interface ITreeValue<T> : ITreeValue
    {
        /// <summary>
        /// 法则执行器
        /// </summary>
        public IRuleActuator<IValueChangeRule<T>> RuleActuator { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }
    }



    /// <summary>
    /// 泛型树值类型
    /// </summary>
    public class TreeValue<T> : Node, ITreeValue<T>, ChildOf<INode>
        where T : struct
    {
        public Type type { get; set; }

        public IRuleActuator<IValueChangeRule<T>> RuleActuator { get; set; }

        private T value;

        public virtual T Value
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
