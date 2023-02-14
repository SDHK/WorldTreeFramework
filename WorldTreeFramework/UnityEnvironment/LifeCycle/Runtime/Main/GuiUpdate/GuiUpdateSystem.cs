namespace WorldTree
{

    /// <summary>
    /// OnGUI系统接口
    /// </summary>
    public interface IGuiUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// OnGUI系统基类
    /// </summary>
    public abstract class GuiUpdateSystem<T> : SystemBase<T, IGuiUpdateSystem>, IGuiUpdateSystem
       where T : Entity
    {
        public void Invoke(Entity self, float deltaTime) => OnGuiUpdate(self as T, deltaTime);
        public abstract void OnGuiUpdate(T self, float deltaTime);
    }
}
