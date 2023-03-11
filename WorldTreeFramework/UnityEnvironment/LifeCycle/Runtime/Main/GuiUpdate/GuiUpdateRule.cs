namespace WorldTree
{

    /// <summary>
    /// OnGUI法则接口
    /// </summary>
    public interface IGuiUpdateRule : ISendRule<float> { }

    /// <summary>
    /// OnGUI法则
    /// </summary>
    public abstract class GuiUpdateRule<T> : SendRuleBase<IGuiUpdateRule, T, float> where T : class,INode { }
}
