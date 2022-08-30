namespace WorldTree
{

    /// <summary>
    /// OnGUI系统接口
    /// </summary>
    public interface IOnGUISystem : ISystem
    {
        void Execute(Entity self, float deltaTime);
    }

    /// <summary>
    /// OnGUI系统基类
    /// </summary>
    public abstract class OnGUISystem<T> : SystemBase<T, IOnGUISystem>, IOnGUISystem
       where T : Entity
    {
        public void Execute(Entity self, float deltaTime) => OnGUI(self as T, deltaTime);
        public abstract void OnGUI(T self, float deltaTime);
    }
}
