/****************************************

* 作者：闪电黑客
* 日期：2024/9/11 17:32

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 热重载测试
	/// </summary>
	public class HotReloadTest : Node
		, ComponentOf<INode>
		, AsComponentBranch
		, AsAwake
	{
	}
}