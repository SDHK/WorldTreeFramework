namespace WorldTree
{

    /// <summary>
    /// LateUpdate系统接口
    /// </summary>
    public interface ILateUpdateSystem : ISendSystem<float> { }
    /// <summary>
    /// LateUpdate系统事件
    /// </summary>
    public abstract class LateUpdateSystem<T> : SendSystemBase<ILateUpdateSystem, T, float> where T : Node { }
}
