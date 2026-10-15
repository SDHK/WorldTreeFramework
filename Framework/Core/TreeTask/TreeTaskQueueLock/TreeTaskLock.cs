/****************************************

* 作者：闪电黑客
* 日期：2026/10/15

* 描述：队列锁的持有凭证。Dispose 时放行同一个 key 的下一位。

*/
namespace WorldTree
{
	/// <summary>
	/// 异步队列锁的持有凭证
	/// </summary>
	public class TreeTaskLock : Node
		, TempOf<INode>
		, AsRule<Awake<long, TreeTaskQueueLockManager>>
	{
		/// <summary>
		/// 管理器。管理器还池后这里变空，不再去放行。
		/// </summary>
		public NodeRef<TreeTaskQueueLockManager> Manager;

		/// <summary>
		/// 锁的 key
		/// </summary>
		public long Key;
	}

	public static class TreeTaskLockRule
	{
		class AwakeRule : AwakeRule<TreeTaskLock, long, TreeTaskQueueLockManager>
		{
			protected override void Execute(TreeTaskLock self, long key, TreeTaskQueueLockManager manager)
			{
				self.Key = key;
				self.Manager = manager;
			}
		}

		class RemoveRule : RemoveRule<TreeTaskLock>
		{
			protected override void Execute(TreeTaskLock self)
			{
				//先摘掉自己，再放行。下一位若马上放开，不会把这一把再放一次
				TreeTaskQueueLockManager manager = self.Manager;
				long key = self.Key;
				self.Manager = null;
				self.Key = 0;
				if (manager != null)
					manager.RunNext(key);
			}
		}
	}
}
