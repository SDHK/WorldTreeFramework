namespace WorldTree
{
    /// <summary>
    /// FixedUpdate系统接口
    /// </summary>
    public interface IFixedUpdateSystem : ISendRule<float> { }

    /// <summary>
    /// FixedUpdate系统事件
    /// </summary>
    public abstract class FixedUpdateSystem<T> : SendRuleBase<IFixedUpdateSystem, T, float> where T : Node { }
}
