/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 11:00

* 描述： UI窗口失焦系统

*/

namespace WorldTree
{

    /// <summary>
    /// UI窗口失焦系统接口
    /// </summary>
    public interface IWindowLostFocusRule : ISendRule { }


    /// <summary>
    /// UI窗口失焦系统
    /// </summary>
    public abstract class WindowLostFocusRule<N> : SendRuleBase<IWindowLostFocusRule, N> where N : class,INode { }
}
