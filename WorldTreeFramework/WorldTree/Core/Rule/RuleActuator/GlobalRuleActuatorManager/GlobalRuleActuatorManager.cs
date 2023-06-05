
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 10:08

* 描述： 全局法则执行器集合

*/

using System;

namespace WorldTree
{

    public static partial class NodeRule
    {
        /// <summary>
        /// 获取全局节点法则执行器
        /// </summary>
        public static bool TryGetGlobalRuleActuator<R>(this INode self, out GlobalRuleActuator<R> globalRuleActuator)
        where R : IRule
        {
            if (self.Core.RuleManager.TryGetRuleGroup<R>(out RuleGroup _))
            {
                self.Root.AddComponent(out GlobalRuleActuatorManager _).AddComponent(out globalRuleActuator);
                return true;

            }
            else
            {
                globalRuleActuator = null;
                return false;
            }
        }
    }

    /// <summary>
    /// 全局法则执行器管理器
    /// </summary>
    public class GlobalRuleActuatorManager : Node, ComponentOf<WorldTreeRoot> { }
}
