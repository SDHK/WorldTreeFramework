/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{

	class CurveManagerRootAddRule : RootAddRule<CurveManager> { }

	/// <summary>
	/// 曲线管理器
	/// </summary>
	public class CurveManager : Node, ComponentOf<WorldTreeRoot>
		, AsComponentBranch
		, AsAwake
	{

	}
}
