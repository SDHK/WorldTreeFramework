
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/30 20:29

* 描述： 全局监听器法则执行器

*/

namespace WorldTree
{
    /// <summary>
    /// 全局监听器法则执行器
    /// </summary>
    public class ListenerRuleActuator : GlobalRuleActuatorBase, IRuleActuator<IRule>, ComponentOf<GlobalRuleActuatorManager>
        , AsRule<IAwakeRule<RuleGroup>>
    {

    }

}
