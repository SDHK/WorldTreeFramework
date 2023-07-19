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
        public override string ToString()
        {
            return $"GlobalRuleActuator : {ruleGroup?.RuleType}";
        }
    }


    public static class GlobalRuleActuatorRule
    {
        class GlobalRuleActuatorAddRule<R> : AddRule<GlobalRuleActuator<R>>
        where R : IRule
        {
            public override void OnEvent(GlobalRuleActuator<R> self)
            {
                self.LoadGlobalNode();
                self.ListenerSwitchesRule<R>();
            }
        }

        /// <summary>
        /// 填装全局节点,并设置监听目标法则
        /// </summary>
        public static void LoadGlobalNode<R>(this GlobalRuleActuator<R> self)
         where R : IRule
        {
            self.ruleGroup = self.Core.RuleManager.GetOrNewRuleGroup<R>();
            foreach (var item in self.ruleGroup)
            {
                if (self.Core.ReferencedPoolManager.TryGetPool(item.Key, out ReferencedPool pool))
                {
                    foreach (var node in pool)
                    {
                        self.TryAdd(node.Value);
                    }
                }
            }
        }
    }
}
