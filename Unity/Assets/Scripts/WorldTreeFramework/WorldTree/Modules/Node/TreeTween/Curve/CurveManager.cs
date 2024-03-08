/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/15 20:02

* 描述： 曲线管理器

*/

namespace WorldTree
{

    class CurveManagerRootAddRule : RootAddRule<CurveManager> { }

    /// <summary>
    /// 曲线管理器
    /// </summary>
    public class CurveManager : Node, ComponentOf<WorldTreeRoot>
         , AsRule<IAwakeRule>
    {

    }
}
