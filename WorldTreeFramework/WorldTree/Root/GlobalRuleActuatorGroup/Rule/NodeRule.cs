namespace WorldTree
{
    public static partial class NodeRule
    {
        /// <summary>
        /// 获取全局节点法则执行器
        /// </summary>
        public static IRuleActuator<R> GetGlobalNodeRuleActuator<R>(this INode self)
        where R : IRule
        {
            return self.Root.AddComponent(out GlobalRuleActuatorGroup _).GetGlobalRuleActuator<R>();
        }
    }
}
