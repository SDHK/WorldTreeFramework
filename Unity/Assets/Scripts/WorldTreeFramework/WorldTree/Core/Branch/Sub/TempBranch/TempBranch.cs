/****************************************

* 作者：闪电黑客
* 日期：2024/5/24 18:37

* 描述：临时节点分支
*
* 和子节点分支一样，但是不受约束，可以挂载任意节点，但理念上应该只挂载临时节点

*/

namespace WorldTree
{
	/// <summary>
	/// 临时节点分支
	/// </summary>
	public class TempBranch : ChildBranch, IBranchUnConstraint
	{ }
}