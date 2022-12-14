namespace WorldTree
{

    /// <summary>
    /// OnGUI系统接口
    /// </summary>
    public interface IOnGUISystem : ISendSystem<float> { }

    /// <summary>
    /// OnGUI系统基类
    /// </summary>
    public abstract class OnGUISystem<T> : SystemBase<T, IOnGUISystem>, IOnGUISystem
       where T : Entity
    {
        public void Invoke(Entity self, float deltaTime) => OnGUI(self as T, deltaTime);
        public abstract void OnGUI(T self, float deltaTime);
    }
}
