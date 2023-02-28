namespace WorldTree
{
    /// <summary>
    /// FixedUpdate系统接口
    /// </summary>
    public interface IFixedUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// FixedUpdate系统事件
    /// </summary>
    public abstract class FixedUpdateSystem<T> : SendSystemBase<IFixedUpdateSystem, T, float> where T : Entity { }
}
