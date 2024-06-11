/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/13 19:30

* 描述： 树任务令牌捕获任务
* 
* 这是一个和TreeTaskCompleted一样的同步任务，
* 
* 它可以从异步流中捕获到令牌
* 

*/

namespace WorldTree.Internal
{
	/// <summary>
	/// 树任务令牌捕获任务
	/// </summary>
	public class TreeTaskTokenCatch : TreeTaskBase, ISyncTask
		, ChildOf<INode>
		, AsAwake

	{
		public TreeTaskTokenCatch GetAwaiter() => this;
		public override bool IsCompleted { get; set; }
		public TreeTaskToken GetResult() { return m_Context as TreeTaskToken; }
	}
}
