/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 10:59

* 描述： 绑定委托式泛型树值类型

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 绑定委托式泛型树值类型
    /// </summary>
    public class TreeValueDelegate<T> : TreeValueBase<T>
        , IAwake<object, Func<object, T>, Action<object, T>>

        , ChildOf<INode>

    where T : struct, IEquatable<T>
    {
        /// <summary>
        /// 绑定的对象
        /// </summary>
        public object m_BindObject;
        /// <summary>
        /// 从绑定对象上获取值
        /// </summary>
        public Func<object, T> m_Get;
        /// <summary>
        /// 将值设置到绑定对象上
        /// </summary>
        public Action<object, T> m_Set;
        public override T Value
        {
            get => m_Get(m_BindObject);

            set
            {
                if (!m_Get(m_BindObject).Equals(value))
                {
                    m_Set(m_BindObject, value);
                    m_RuleActuator?.Send(value);
                    m_GlobalRuleActuator?.Send(value);
                }
            }
        }
    }
}
