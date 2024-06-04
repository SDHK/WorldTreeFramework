/****************************************

* 作者： 闪电黑客
* 日期： 2023/10/24 22:00

* 描述： 组件分支
*
* 主要分支之一
* 设定根据类型为键挂载，
* 所以在同一节点下，同一类型的组件只能有一个
*

*/

namespace WorldTree
{
	/// <summary>
	/// 组件分支
	/// </summary>
	public class ComponentBranch : Branch<long>, IBranchTypeKey
	{ }

}