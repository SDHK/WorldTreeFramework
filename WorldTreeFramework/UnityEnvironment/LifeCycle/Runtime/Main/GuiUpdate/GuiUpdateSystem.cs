namespace WorldTree
{

    /// <summary>
    /// OnGUI系统接口
    /// </summary>
    public interface IGuiUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// OnGUI系统事件
    /// </summary>
    public abstract class GuiUpdateSystem<T> : SendSystemBase<IGuiUpdateSystem, T, float> where T : Node { }
}
