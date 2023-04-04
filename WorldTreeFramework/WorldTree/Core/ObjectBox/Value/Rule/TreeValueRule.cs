/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{

    class TreeValueValueChangeRule<T> : ValueChangeRule<TreeValueBase<T>, T>
    where T : struct
    {
        public override void OnEvent(TreeValueBase<T> self, T arg1)
        {
            self.Value = arg1;
        }
    }

    class TreeValueRemoveRule<T> : RemoveRule<TreeValueBase<T>>
    where T : struct
    {
        public override void OnEvent(TreeValueBase<T> self)
        {
            self.RuleActuator = default;
            self.Value = default;
            self.GlobalRuleType = default;
        }
    }

    public static class TreeValueRule
    {
        /// <summary>
        /// 单向绑定
        /// </summary>
        public static void Bind<T>(this TreeValueBase<T> self, TreeValueBase<T> treeValue)
            where T : struct
        {
            if (self.RuleActuator is null) self.TryGetRuleActuator(out self.RuleActuator);
            self.RuleActuator?.EnqueueReferenced(treeValue);
        }

        /// <summary>
        /// 双向绑定
        /// </summary>
        public static void BindTwoWay<T>(this TreeValueBase<T> self, TreeValueBase<T> treeValue)
            where T : struct
        {
            if (self.RuleActuator is null) self.TryGetRuleActuator(out self.RuleActuator);
            if (treeValue.RuleActuator is null) treeValue.TryGetRuleActuator(out treeValue.RuleActuator);
            self.RuleActuator?.EnqueueReferenced(treeValue);
            treeValue.RuleActuator?.EnqueueReferenced(self);
        }
    }

}
