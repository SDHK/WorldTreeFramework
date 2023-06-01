/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/26 11:11

* 描述： 泛型全局单法则执行器

*/

namespace WorldTree
{

    /// <summary>
    /// 全局法则执行器
    /// </summary>
    public class GlobalRuleActuator<R> : GlobalRuleActuatorBase, IRuleActuator<R>, ComponentOf<GlobalRuleActuatorManager>
     where R : IRule
    {
    }


    public static class GlobalRuleActuatorRule
    {
        class GlobalRuleActuatorAddRule<R> : AddRule<GlobalRuleActuator<R>>
        where R : IRule
        {
            public override void OnEvent(GlobalRuleActuator<R> self)
            {
                self.LoadGlobalNode<R>();
                self.ListenerSwitchesRule<R>();
            }
        }
    }
}
