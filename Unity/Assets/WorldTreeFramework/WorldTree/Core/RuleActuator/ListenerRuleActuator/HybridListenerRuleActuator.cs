
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/4 18:26

* 描述： 混合型监听器法则执行器

*/

namespace WorldTree
{

    /// <summary>
    /// 混合型监听器法则执行器
    /// </summary>
    public class HybridListenerRuleActuator : CoreNode, IRuleActuatorTraversal, IRuleActuator<IRule>
        , ComponentOf<HybridListenerRuleActuatorGroup>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 静态监听器法则执行器
        /// </summary>
        public StaticListenerRuleActuator staticListenerRuleActuator;
        /// <summary>
        /// 动态监听器法则执行器
        /// </summary>
        public DynamicListenerRuleActuator dynamicListenerRuleActuator;

        public int TraversalCount => staticListenerRuleActuator?.TraversalCount ?? 0 + dynamicListenerRuleActuator?.TraversalCount ?? 0;

        public int RefreshTraversalCount()
        {
            return staticListenerRuleActuator?.RefreshTraversalCount() ?? 0 + dynamicListenerRuleActuator?.RefreshTraversalCount() ?? 0;
        }

        public bool TryGetNext(out INode node, out RuleList ruleList)
        {
            if (staticListenerRuleActuator != null && staticListenerRuleActuator.TraversalCount != 0)
            {
                return staticListenerRuleActuator.TryGetNext(out node, out ruleList);
            }
            if (dynamicListenerRuleActuator != null && dynamicListenerRuleActuator.TraversalCount != 0)
            {
                return dynamicListenerRuleActuator.TryGetNext(out node, out ruleList);
            }
            node = null;
            ruleList = null;
            return false;
        }
    }

    public static class HybridListenerRuleActuatorRule
    {
        class RemoveRule : RemoveRule<HybridListenerRuleActuator>
        {
            protected override void OnEvent(HybridListenerRuleActuator self)
            {
                self.staticListenerRuleActuator = null;
                self.dynamicListenerRuleActuator = null;
            }
        }
    }
}
