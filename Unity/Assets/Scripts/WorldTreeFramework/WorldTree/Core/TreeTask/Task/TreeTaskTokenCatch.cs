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

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
		{ Result = m_TreeTaskToken; return base.GetResult(); }
	}
}