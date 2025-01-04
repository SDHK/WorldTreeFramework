/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
namespace WorldTree.Internal
{
	/// <summary>
	/// 树任务令牌捕获任务
	/// </summary>
	public class TreeTaskTokenCatch : AwaiterBase<TreeTaskToken>, ISyncTask
		, ChildOf<INode>
		, AsAwake

	{
		public override TreeTaskToken GetResult()
		{ Result = TreeTaskToken; return base.GetResult(); }
	}
}