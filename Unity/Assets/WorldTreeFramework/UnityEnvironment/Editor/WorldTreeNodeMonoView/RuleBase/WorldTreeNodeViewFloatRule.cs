namespace WorldTree
{
	public interface IWorldTreeNodeViewFloatRule : ISendRuleBase<string, float> { }

	public abstract class WorldTreeNodeViewFloatRule<N> : SendRuleBase<N, IWorldTreeNodeViewFloatRule, string, float>
		where N : class, INode, AsRule<IWorldTreeNodeViewFloatRule> { }
}
