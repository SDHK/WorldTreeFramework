/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/21 20:42

* 描述： 泛型树值类型

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 泛型树值类型
    /// </summary>
    public class TreeValue<T> : TreeValueBase<T>
        , IAwake
        , IAwake<T>
        , ChildOf<INode>

    where T : struct, IEquatable<T>
    {
        public T m_Value;
        public override T Value
        {
            get => m_Value;

            set
            {
                if (!this.m_Value.Equals(value))
                {
                    this.m_Value = value;
                    m_RuleActuator?.Send(value);
                    m_GlobalRuleActuator?.Send(value);
                }
            }
        }
    }
}
