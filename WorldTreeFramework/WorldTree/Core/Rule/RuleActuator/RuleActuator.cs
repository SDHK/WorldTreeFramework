/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则执行器
* 

*/


namespace WorldTree
{

    /// <summary>
    /// 法则执行器
    /// </summary>
    public partial class RuleActuator : RuleActuatorBase, IRuleActuator<IRule> { }

    /// <summary>
    /// 法则执行器
    /// </summary>
    public class RuleActuator<R> : RuleActuatorBase where R : IRule { }


    class RuleActuatorRemoveRule<R> : RemoveRule<RuleActuator<R>>
         where R : IRule
    {
        public override void OnEvent(RuleActuator<R> self)
        {
            self.ruleGroup = null;
        }
    }


    public static class RuleActuatorRule
    {

        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>
        public static void EnqueueReferenced<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuatorBase)self).TryAddReferenced(node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuatorBase)self).TryAdd(node);
        }


        /// <summary>
        /// 移除节点
        /// </summary>
        public static void Remove<R, N>(this IRuleActuator<R> self, N node)
            where R : IRule
            where N : class, INode, AsRule<R>
        {
            ((RuleActuatorBase)self).Remove(node);
        }



        /// <summary>
        /// 添加法则节点并建立引用关系
        /// </summary>
        public static void Add<N, R, RN>(this IRuleActuator<R> self, N node, out RN ruleNode)
            where N : class, INode
            where R : IRule
            where RN : class, INode, ComponentOf<N>, AsRule<R>
        {
            ((RuleActuatorBase)self).TryAddReferenced(node.AddComponent(out ruleNode));
        }

    }
}
