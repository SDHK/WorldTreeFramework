/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 14:53

* 描述：入口节点
*
* 在 世界树 启动后挂载在根节点
*
* 可用于初始化启动需要的功能组件

*/
namespace WorldTree
{
	/// <summary>
	/// 入口节点
	/// </summary>
	public class Entry : Node
		, ComponentOf<WorldTreeRoot>
		, AsAwake
		, AsComponentBranch
	{ }
}
