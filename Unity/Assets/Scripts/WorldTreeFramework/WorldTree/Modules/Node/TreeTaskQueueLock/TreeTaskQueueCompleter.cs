/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 14:06

* 描述： 异步任务队列锁的解锁器

*/

namespace WorldTree
{
	/// <summary>
	/// 异步任务队列锁的解锁器
	/// </summary>
	public class TreeTaskQueueCompleter : Node, ChildOf<INode>
		, AsAwake<long, TreeTaskQueueLockManager>
	{
		/// <summary>
		/// 管理器
		/// </summary>
		public TreeTaskQueueLockManager queueLock;
		/// <summary>
		/// 锁的key
		/// </summary>
		public long key;
	}




}
