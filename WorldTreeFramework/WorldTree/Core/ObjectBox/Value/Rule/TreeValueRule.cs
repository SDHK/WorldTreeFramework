/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

using System;

namespace WorldTree
{

    class TreeValueValueChangeRule<T> : ValueChangeRule<TreeValue<T>, T>
    where T : struct
    {
        public override void OnEvent(TreeValue<T> self, T arg1)
        {
            self.Value = arg1;
        }
    }

    class TreeValueRemoveRule<T> : RemoveRule<TreeValue<T>>
    where T : struct
    {
        public override void OnEvent(TreeValue<T> self)
        {
            self.RuleActuator = default;
            self.Value = default;
            self.type = default;
        }
    }

    public static class TreeValueRule
    {
        /// <summary>
        /// 单向绑定
        /// </summary>
        public static void Bind<T>(this ITreeValue<T> self, ITreeValue<T> treeValue)
            where T : struct
        {
            if (self.RuleActuator is null)
            {
                if (self.TryGetRuleActuator(out IRuleActuator<IValueChangeRule<T>> ruleActuator))
                {
                    self.RuleActuator = ruleActuator;
                }
            }

            self.RuleActuator?.EnqueueReferenced(treeValue);
        }

        /// <summary>
        /// 双向绑定
        /// </summary>
        public static void BindTwoWay<T>(this ITreeValue<T> self, ITreeValue<T> treeValue)
            where T : struct
        {

            if (self.RuleActuator is null)
            {
                if (self.TryGetRuleActuator(out IRuleActuator<IValueChangeRule<T>> ruleActuator))
                {
                    self.RuleActuator = ruleActuator;
                }
            }
            if (treeValue.RuleActuator is null)
            {
                if (treeValue.TryGetRuleActuator(out IRuleActuator<IValueChangeRule<T>> ruleActuator))
                {
                    treeValue.RuleActuator = ruleActuator;
                }
            }

            self.RuleActuator?.EnqueueReferenced(treeValue);
            treeValue.RuleActuator?.EnqueueReferenced(self);


        }
    }

}
