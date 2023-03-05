namespace WorldTree
{
    /// <summary>
    /// Update系统接口
    /// </summary>
    public interface IUpdateSystem : ISendRule<float> { }

    /// <summary>
    /// Update系统基类
    /// </summary>
    public abstract class UpdateSystem<E> : SendRuleBase<IUpdateSystem, E, float> where E : Node { }
}
