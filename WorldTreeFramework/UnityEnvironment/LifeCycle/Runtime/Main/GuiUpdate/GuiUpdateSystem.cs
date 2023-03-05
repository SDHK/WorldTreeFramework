namespace WorldTree
{

    /// <summary>
    /// OnGUI系统接口
    /// </summary>
    public interface IGuiUpdateSystem : ISendRule<float> { }

    /// <summary>
    /// OnGUI系统事件
    /// </summary>
    public abstract class GuiUpdateSystem<T> : SendRuleBase<IGuiUpdateSystem, T, float> where T : Node { }
}
