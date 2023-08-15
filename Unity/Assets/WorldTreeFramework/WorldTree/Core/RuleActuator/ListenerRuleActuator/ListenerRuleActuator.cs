
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
    public class ListenerRuleActuator : GlobalRuleActuatorBase, ICoreNode, IRuleActuator<IRule>, ComponentOf<GlobalRuleActuatorManager>
        , AsRule<IAwakeRule<RuleGroup>>
    {
        public override string ToString()
        {
            return $"ListenerRuleActuator : {ruleGroup?.RuleType.HashCore64ToType()}";
        }
    }

    class ListenerRuleActuatorAwakeRule : AwakeRule<ListenerRuleActuator, RuleGroup>
    {
        protected override void OnEvent(ListenerRuleActuator self, RuleGroup arg1)
        {
            self.ruleGroup = arg1;
        }
    }

}
