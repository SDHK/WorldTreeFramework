/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

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

	public static partial class TreeTaskQueueCompleterRule
	{
		class AwakeRule : AwakeRule<TreeTaskQueueCompleter, long, TreeTaskQueueLockManager>
		{
			protected override void Execute(TreeTaskQueueCompleter self, long key, TreeTaskQueueLockManager queueLock)
			{
				self.key = key;
				self.queueLock = queueLock;
			}
		}

		class RemoveRule : RemoveRule<TreeTaskQueueCompleter>
		{
			protected override void Execute(TreeTaskQueueCompleter self)
			{
				self.queueLock.RunNext(self.key);
				self.queueLock = default;
				self.key = default;
			}
		}
	}

}
