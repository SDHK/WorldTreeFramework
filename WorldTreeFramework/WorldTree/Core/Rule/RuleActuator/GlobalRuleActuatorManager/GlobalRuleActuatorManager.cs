
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
        public static GlobalRuleActuator<R> GetGlobalRuleActuator<R>(this INode self)
        where R : IRule
        {
            return self.Root.AddComponent(out GlobalRuleActuatorManager _).AddComponent(out GlobalRuleActuator<R> _);
        }
    }

    /// <summary>
    /// 全局法则执行器管理器
    /// </summary>
    public class GlobalRuleActuatorManager : Node, ComponentOf<WorldTreeRoot> { }
}
