namespace WorldTree
{
    /// <summary>
    /// FixedUpdate法则接口
    /// </summary>
    public interface IFixedUpdateRule : ISendRule<float> { }

    /// <summary>
    /// FixedUpdate法则
    /// </summary>
    public abstract class FixedUpdateRule<T> : SendRuleBase<IFixedUpdateRule, T, float> where T : Node { }
}
