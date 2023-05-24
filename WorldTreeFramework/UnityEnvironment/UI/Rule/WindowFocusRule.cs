/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/7 10:56

* 描述： UI窗口焦点系统

*/

namespace WorldTree
{

    /// <summary>
    /// UI窗口焦点系统接口
    /// </summary>
    public interface IWindowFocusRule : ISendRuleBase { }

    /// <summary>
    /// UI窗口焦点系统
    /// </summary>
    public abstract class WindowFocusRule<N> : SendRuleBase<N, IWindowFocusRule> where N : class, INode, AsRule<IWindowFocusRule> { }
}
