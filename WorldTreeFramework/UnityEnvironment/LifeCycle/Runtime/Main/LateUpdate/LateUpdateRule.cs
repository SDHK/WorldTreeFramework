namespace WorldTree
{

    /// <summary>
    /// LateUpdate法则接口
    /// </summary>
    public interface ILateUpdateRule : ISendRule<float> { }
    /// <summary>
    /// LateUpdate法则
    /// </summary>
    public abstract class LateUpdateRule<T> : SendRuleBase<ILateUpdateRule, T, float> where T : class, INode { }
}
