namespace WorldTree
{

    /// <summary>
    /// LateUpdate系统接口
    /// </summary>
    public interface ILateUpdateSystem : ISendSystem<float>{}
    /// <summary>
    /// LateUpdate系统基类
    /// </summary>
    public abstract class LateUpdateSystem<T> : SystemBase<T, ILateUpdateSystem>, ILateUpdateSystem
        where T : Entity
    {
        public void Invoke(Entity self, float deltaTime) => OnLateUpdate(self as T, deltaTime);
        public abstract void OnLateUpdate(T self, float deltaTime);
    }
}
