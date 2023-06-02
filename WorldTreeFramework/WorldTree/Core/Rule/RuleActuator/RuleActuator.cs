﻿/****************************************

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
    public class RuleActuator<R> : RuleActuatorBase, IRuleActuator<R> where R : IRule
    {
        public override string ToString()
        {
            return $"RuleActuator<{typeof(R)}>";
        }
    }


    public static class RuleActuatorRule
    {
        /// <summary>
        /// 添加节点法则并建立引用关系
        /// </summary>
        public static void Add<R, N, NR>(this RuleActuator<R> self, N node, NR defaultRule)
            where R : IRule
            where N : class, INode, AsRule<NR>
            where NR : R
        {
            if (self.GetRuleGroup(typeof(NR)) == null) { World.Log($"空法则{typeof(NR)}"); }
            self.TryAddReferenced(node, self.GetRuleGroup(typeof(NR)));
        }
    }
}
