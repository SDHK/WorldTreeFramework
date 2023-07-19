/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{




    public static partial class TreeValueRule
    {

        /// <summary>
        /// 设置一个全局法则执行器
        /// </summary>
        public static void SetGlobalRuleActuator<T, R>(this TreeValueBase<T> self, R defaultRule = default)
            where T : IEquatable<T>
            where R : ISendRuleBase<T>
        {

            self.m_GlobalValueChange = (IRuleActuator<ISendRuleBase<T>>)self.GetOrNewGlobalRuleActuator<R>(out _);
        }

        /// <summary>
        /// 单向绑定 数值监听
        /// </summary>
        public static void AddListenerValueChange<T1, N>(this TreeValueBase<T1> self, N eventNode)
            where T1 : IEquatable<T1>
            where N : class, INode, AsRule<ISendRuleBase<T1>>
        {
            if (self.m_ValueChange is null) self.AddChild(out self.m_ValueChange);
            self.m_ValueChange.Add(eventNode, default(IValueChangeRule<T1>));
        }

        /// <summary>
        /// 单向绑定(类型转换)
        /// </summary>
        public static void Bind<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if (self.m_ValueChange is null) self.AddChild(out self.m_ValueChange);
            self.m_ValueChange.Add(treeValue, default(IValueChangeRule<T1>));
        }



        /// <summary>
        /// 双向绑定(类型转换)
        /// </summary>
        public static void BindTwoWay<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if (self.m_ValueChange is null) self.AddChild(out self.m_ValueChange);
            if (treeValue.m_ValueChange is null) treeValue.AddChild(out treeValue.m_ValueChange);
            self.m_ValueChange.Add(treeValue, default(IValueChangeRule<T1>));
            treeValue.m_ValueChange.Add(self, default(IValueChangeRule<T2>));
        }
    }

}
