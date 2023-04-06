/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/6 10:59

* 描述： 委托式泛型树值类型

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 委托式泛型树值类型
    /// </summary>
    public class TreeValueDelegate<T> : TreeValueBase<T>
        , IAwake<Func<T>, Action<T>>

        , ChildOf<INode>

    where T : struct, IEquatable<T>
    {
        public Func<T> Get;
        public Action<T> Set;
        public override T Value
        {
            get => Get();

            set
            {
                if (Get().Equals(value))
                {
                    this.Set(value);
                    m_RuleActuator?.Send(value);
                    m_GlobalRuleActuator?.Send(value);
                }
            }
        }
    }
}
