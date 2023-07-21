/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 11:17

* 描述： UI窗口焦点更新系统

*/

namespace WorldTree
{
    /// <summary>
    /// UI窗口焦点更新系统接口
    /// </summary>
    public interface IWindowFocusUpdateRule : ISendRuleBase<float> { }

    /// <summary>
    /// UI窗口焦点更新系统
    /// </summary>
    public abstract class WindowFocusUpdateRule<N> : SendRuleBase<N, IWindowFocusUpdateRule, float> where N : class, INode, AsRule<IWindowFocusUpdateRule> { }
}
