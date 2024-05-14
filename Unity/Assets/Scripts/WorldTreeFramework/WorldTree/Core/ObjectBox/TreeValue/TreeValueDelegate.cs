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
    public partial class TreeValueDelegate<T> : TreeValueBase<T>
        , AsAwake<object, Func<object, T>, Action<object, T>>
        , ChildOf<INode>
    where T : IEquatable<T>
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
                if (this.m_Get(m_BindObject) is null)
                {
                    this.m_Set(m_BindObject, value);
                }
                else if (!m_Get(m_BindObject).Equals(value))
                {
                    m_Set(m_BindObject, value);
                    m_ValueChange?.Send(value);
                    m_GlobalValueChange?.Send(value);
                }
            }
        }
    }


    public static class TreeValueDelegateRule
    {
        class AwakeRuleGenerics<T> : AwakeRule<TreeValueDelegate<T>, object, Func<object, T>, Action<object, T>>
            where T : struct, IEquatable<T>
        {
            protected override void Execute(TreeValueDelegate<T> self, object arg1, Func<object, T> arg2, Action<object, T> arg3)
            {
                self.m_BindObject = arg1;
                self.m_Get = arg2;
                self.m_Set = arg3;
            }
        }
    }



}
