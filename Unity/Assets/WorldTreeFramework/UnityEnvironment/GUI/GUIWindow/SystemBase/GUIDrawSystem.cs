/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/18 13:08

* 描述： GUI绘制系统

*/

namespace WorldTree
{
    /// <summary>
    /// GUI绘制
    /// </summary>
    public interface IGUIDrawSystem : ISendRuleBase { }

    /// <summary>
    /// GUI绘制系统事件
    /// </summary>
    public abstract class GUIDrawSystem<T> : SendRuleBase<T, IGUIDrawSystem> where T : class, INode, AsRule<IGUIDrawSystem> { }
}
