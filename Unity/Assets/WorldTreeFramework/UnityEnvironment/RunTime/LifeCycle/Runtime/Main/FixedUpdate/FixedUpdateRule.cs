namespace WorldTree
{
    /// <summary>
    /// FixedUpdate法则接口
    /// </summary>
    public interface IFixedUpdateRule : ISendRuleBase<float> { }

    /// <summary>
    /// FixedUpdate法则
    /// </summary>
    public abstract class FixedUpdateRule<T> : SendRuleBase<T, IFixedUpdateRule, float> where T : class, INode, AsRule<IFixedUpdateRule> { }
}
