/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 泛型法则执行器
* 

*/


namespace WorldTree
{
    /// <summary>
    /// 泛型法则执行器
    /// </summary>
    public class RuleActuator<R> : RuleActuatorBase, ChildOf<INode>, IRuleActuator<R>

        , AsRule<IAwakeRule>
        where R : IRule

    {
        public override string ToString()
        {
            return $"RuleActuator<{typeof(R)}>:[{nodeDictionary?.Count}]";
        }
    }


    public static class RuleActuatorRule
    {
        class RuleActuatorReferencedChildRemoveRule<R> : ReferencedChildRemoveRule<RuleActuator<R>>
            where R : IRule
        {
            protected override void OnEvent(RuleActuator<R> self, INode node)
            {
                self.Remove(node);
            }
        }

        /// <summary>
        /// 添加节点法则并建立引用关系
        /// </summary>
        public static void Add<R, N, NR>(this RuleActuator<R> self, N node, NR defaultRule)
            where R : IRule
            where N : class, INode, AsRule<NR>
            where NR : R
        {
            if (self.Core.RuleManager.TryGetRuleList<NR>(node.Type, out RuleList ruleList))
            {
                self.TryAddReferenced(node, ruleList);
            }
            else
            {
                World.Log($"空法则{typeof(NR)}");
            }
        }
    }
}
