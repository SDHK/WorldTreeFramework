/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 17:28

* 描述： 

*/

namespace WorldTree
{
    public static partial class NodeRule
    {
        /// <summary>
        /// 获取一个填装法则的法则执行器
        /// </summary>
        public static bool TryGetRuleActuator<R>(this INode self, out IRuleActuator<R> ruleActuator)
            where R : IRule
        {
            if (self.Core.RuleManager.TryGetRuleGroup<R>(out var ruleGroup))
            {
                self.AddChild(out RuleActuator RuleActuator);
                RuleActuator.ruleGroup = (RuleGroup)ruleGroup;
                ruleActuator = (IRuleActuator<R>)RuleActuator;
                return true;
            }
            ruleActuator = null;
            return false;
        }
    }
}
