/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{

    class TreeValueRemoveRule<T> : RemoveRule<TreeValueBase<T>>
    where T : IEquatable<T>
    {
        public override void OnEvent(TreeValueBase<T> self)
        {
            self.m_GlobalValueChange = default;
            self.m_ValueChange = default;
            self.Value = default;
        }
    }


    public static class TreeValueRule
    {

        /// <summary>
        /// 设置一个全局法则执行器
        /// </summary>
        public static void SetGlobalRuleActuator<T, R>(this TreeValueBase<T> self, R defaultRule = default)
            where T : IEquatable<T>
            where R : IValueChangeRule<T>
        {
            self.m_GlobalValueChange = (IRuleActuator<IValueChangeRule<T>>)self.GetGlobalNodeRuleActuator<R>();
        }

        /// <summary>
        /// 单向绑定 数值监听
        /// </summary>
        public static void AddListenerValueChange<T1, N>(this TreeValueBase<T1> self, N eventNode)
            where T1 : IEquatable<T1>
            where N : class, INode, AsRule<ISendRule<T1>>
        {
            if (self.m_ValueChange is null) self.TryGetRuleActuator(out self.m_ValueChange);
            self.m_ValueChange.EnqueueReferenced(eventNode);
        }

        /// <summary>
        /// 单向绑定(类型转换)
        /// </summary>
        public static void Bind<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if (self.m_ValueChange is null) self.TryGetRuleActuator(out self.m_ValueChange);
            self.m_ValueChange.EnqueueReferenced(treeValue);
        }

       

        /// <summary>
        /// 双向绑定(类型转换)
        /// </summary>
        public static void BindTwoWay<T1, T2>(this TreeValueBase<T1> self, TreeValueBase<T2> treeValue)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            if (self.m_ValueChange is null) self.TryGetRuleActuator(out self.m_ValueChange);
            if (treeValue.m_ValueChange is null) treeValue.TryGetRuleActuator(out treeValue.m_ValueChange);
            self.m_ValueChange.EnqueueReferenced(treeValue);
            treeValue.m_ValueChange.EnqueueReferenced(self);
        }
    }

}
