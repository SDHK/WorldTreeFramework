/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/8 14:38

* 描述： 静态监听器法则执行器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 静态监听器法则执行器
    /// </summary>
    public class StaticListenerRuleActuator : RuleGroupActuatorBase, IListenerIgnorer, IRuleActuator<IRule>
        , ComponentOf<HybridListenerRuleActuator>
        , AsAwake<RuleGroup>
    {
        public override string ToString()
        {
            return $"StaticListenerRuleActuator : {ruleGroupDict?.RuleType.CodeToType()}";
        }
    }

    public static class StaticListenerRuleActuatorRule
    {
        class AwakeRule : AwakeRule<StaticListenerRuleActuator, RuleGroup>
        {
            protected override void Execute(StaticListenerRuleActuator self, RuleGroup arg1)
            {
                self.ruleGroupDict = arg1;
            }
        }

        /// <summary>
        /// 执行器填装监听器
        /// </summary>
        public static void RuleActuatorAddListener(this StaticListenerRuleActuator self)
        {
            //遍历法则集合获取监听器类型
            foreach (var listenerType in self.ruleGroupDict)
            {
                //从池里拿到已存在的监听器
                if (self.Core.ReferencedPoolManager.TryGetPool(listenerType.Key, out ReferencedPool listenerPool))
                {
                    //全部注入到执行器
                    foreach (var listener in listenerPool)
                    {
                        self.TryAdd(listener.Value);
                    }
                }
            }
        }
    }


}
