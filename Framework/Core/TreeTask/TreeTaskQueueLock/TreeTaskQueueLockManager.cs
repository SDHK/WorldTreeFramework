/****************************************

* 作者：闪电黑客
* 日期：2026/10/15

* 描述：按 key 排队的异步锁。等待对象是 TreeTask，挂在 TreeTaskManager 的临时分支上。

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 异步任务队列锁
	/// </summary>
	public class TreeTaskQueueLockManager : Node, ComponentOf<World>
		, AsRule<Awake>
	{
		/// <summary>
		/// 每个 key 一条等待队列。队列还在，就表示已经有人持有这把锁。
		/// </summary>
		public UnitDictionary<long, UnitQueue<TreeTaskQueueLockAsync>> queueDict;
	}

	/// <summary>
	/// 排在队列里的等待。取消时标记完成，但不从池里拿走，避免对象还在队列中就被复用。
	/// </summary>
	public class TreeTaskQueueLockAsync : TreeTask<TreeTaskLock>
		, AsRule<TreeTaskTokenEvent>
	{
		/// <summary>
		/// 还在队列里。为真时取结果不能还池。
		/// </summary>
		public bool InQueue;

		/// <summary>
		/// 调用方已经拿走锁。还池时不再替它释放。
		/// </summary>
		private bool handed;

		/// <summary>
		/// 排进队列前清掉上一轮残留。节点池不抹字段，Result 还会指着上一把锁。
		/// </summary>
		public void JoinQueue()
		{
			Result = default;
			handed = false;
			InQueue = true;
		}

		/// <summary>
		/// 延续注册时令牌已经写上。这里订阅取消。
		/// </summary>
		public override void UnsafeOnCompleted(Action continuation)
		{
			//先记下延续。令牌是构建器在这次调用前写上的，这里才能看到
			base.UnsafeOnCompleted(continuation);
			TreeTaskToken token = TreeTaskToken.Value;
			if (token == null || IsCompleted) return;
			//已经取消的不再排队，直接结束，结果是空
			if (token.State == TokenState.Cancel)
			{
				SetCompleted();
				return;
			}
			token.Add(this);
		}

		/// <summary>
		/// 取消的等待不把锁交出去。人还在队列里时不能还池。
		/// </summary>
		public override TreeTaskLock GetResult()
		{
			TreeTaskToken token = TreeTaskToken.Value;
			token?.tokenEvent?.Remove(this);
			//取消时结果是空。暂停后又被取消的，Result 里可能已经塞了凭证，要放掉
			bool reject = awaiterState == AwaiterState.Canceled;
			TreeTaskLock value = reject ? null : Result;
			if (reject && Result != null)
			{
				TreeTaskLock drop = Result;
				Result = default;
				drop.Dispose();
			}
			//调用方已经取过一次。后面还池不能再把凭证释放掉
			handed = true;
			//还在队列里就先留着。现在还池的话，池子会把这个对象发给下一次等待
			if (!InQueue)
				base.GetResult();
			return value;
		}

		public override void OnDispose()
		{
			TreeTaskToken token = TreeTaskToken.Value;
			token?.tokenEvent?.Remove(this);
			//没人取走的凭证跟着任务一起放掉，避免这把锁卡住
			TreeTaskLock hold = handed ? null : Result;
			//节点池不抹字段。不清的话，下次取消会把残留凭证当成自己的锁放掉
			Result = default;
			InQueue = false;
			handed = false;
			//base 会把节点还池。还池之后不能再写自己的字段，下一位可能已经复用了这个对象
			base.OnDispose();
			if (hold != null) hold.Dispose();
		}

		/// <summary>
		/// 排队期间被取消：任务结束，锁留给后面的人。
		/// </summary>
		public void CancelFromToken()
		{
			if (IsCompleted || InstanceId == 0) return;
			TreeTaskToken.Value?.tokenEvent?.Remove(this);
			//只结束这次等待，不 SetResult。人还留在队列里，等 RunNext 取出再还池
			SetCompleted();
		}
	}

	public static partial class TreeTaskQueueLockManagerRule
	{
		class AddRule : AddRule<TreeTaskQueueLockManager>
		{
			protected override void Execute(TreeTaskQueueLockManager self)
			{
				//字典是单位，不挂子节点
				self.World.PoolGetUnit(out self.queueDict);
			}
		}

		class RemoveRule : RemoveRule<TreeTaskQueueLockManager>
		{
			protected override void Execute(TreeTaskQueueLockManager self)
			{
				//先摘掉字典，避免等待者结束时再进来排队
				UnitDictionary<long, UnitQueue<TreeTaskQueueLockAsync>> queueDict = self.queueDict;
				self.queueDict = null;
				if (queueDict == null) return;
				foreach (UnitQueue<TreeTaskQueueLockAsync> queue in queueDict.Values)
				{
					while (queue.TryDequeue(out TreeTaskQueueLockAsync wait))
					{
						if (wait == null || wait.InstanceId == 0) continue;
						//续体里可能已经还池，甚至被池子再次发出去。只还池当时那个实例
						long instanceId = wait.InstanceId;
						wait.InQueue = false;
						if (!wait.IsCompleted)
							wait.SetCompleted();
						if (wait.InstanceId == instanceId)
							wait.Dispose();
					}
					queue.Dispose();
				}
				queueDict.Dispose();
			}
		}

		/// <summary>
		/// 异步队列锁。同一个 key 按排队顺序进入，Dispose 凭证后下一位才继续。
		/// 没拿到锁时结果是空，调用方要自己判断。
		/// </summary>
		/// <remarks>
		/// <code>
		/// using (TreeTaskLock hold = await self.AsyncLock(self.Id))
		/// {
		///     if (hold == null) return;
		/// }
		/// </code>
		/// </remarks>
		public static async TreeTask<TreeTaskLock> AsyncLock(this INode self, long key)
		{
			//链上已经取消的不再去占锁
			TreeTaskToken token = await self.TreeTaskTokenCatch();
			if (token != null && token.State == TokenState.Cancel) return null;

			//管理器按类型只有一个，没有就挂到世界上
			if (!self.World.TryGetComponent(out TreeTaskQueueLockManager manager))
				self.World.AddComponent(out manager);
			//空闲时这里是已完成的同步任务，排队时是还没完成的等待
			TreeTask<TreeTaskLock> wait = manager.Enter(key);
			if (wait == null) return null;
			return await wait;
		}

		/// <summary>
		/// 空闲就直接给凭证，否则排进这个 key 的队列。只给 AsyncLock 用。
		/// </summary>
		private static TreeTask<TreeTaskLock> Enter(this TreeTaskQueueLockManager self, long key)
		{
			//管理器正在移除，字典已经摘掉
			if (self.queueDict == null) return null;

			//没有这个 key：当前调用者就是持有者，队列先占着，人本身不进队列
			if (!self.queueDict.TryGetValue(key, out UnitQueue<TreeTaskQueueLockAsync> queue))
			{
				self.World.PoolGetUnit(out queue);
				self.queueDict.Add(key, queue);
				//凭证和任务都挂到 TreeTaskManager 上。凭证不能挂在任务下面，任务取结果时会把子节点一起拆掉
				TreeTaskManager.Get(self).AddTemp(out TreeTaskLock hold, key, self);
				//同步任务，调用方 await 时当场取走凭证并还池
				TreeTaskManager.Get(self).AddTemp(out TreeTask<TreeTaskLock> syncTask);
				syncTask.SetResult(hold);
				return syncTask;
			}

			//已经有人持有：排进去，等 RunNext 来 SetResult
			TreeTaskManager.Get(self).AddTemp(out TreeTaskQueueLockAsync wait);
			wait.JoinQueue();
			queue.Enqueue(wait);
			return wait;
		}

		/// <summary>
		/// 当前持有者放开后，跳过已经取消的等待，把锁交给下一位。队列空了就删掉这个 key。
		/// </summary>
		public static void RunNext(this TreeTaskQueueLockManager self, long key)
		{
			if (self.queueDict == null) return;
			if (!self.queueDict.TryGetValue(key, out UnitQueue<TreeTaskQueueLockAsync> queue)) return;

			while (queue.TryDequeue(out TreeTaskQueueLockAsync wait))
			{
				if (wait == null || wait.InstanceId == 0) continue;
				long instanceId = wait.InstanceId;
				//先离开队列再还池，避免池子复用后队列还指着它
				wait.InQueue = false;
				//取消过的等待已经结束，跳过，把锁留给后面还活着的人
				if (wait.IsCompleted)
				{
					if (wait.InstanceId == instanceId)
						wait.Dispose();
					continue;
				}

				TreeTaskManager.Get(self).AddTemp(out TreeTaskLock hold, key, self);
				//唤醒下一位。对方如果立刻放开，会重入这里
				wait.SetResult(hold);
				return;
			}

			//没有下一位了，这个 key 不再表示有人持有
			self.queueDict.Remove(key);
			queue.Dispose();
		}

		/// <summary>
		/// 排队期间令牌取消：结束这次等待，锁留给后面的人。
		/// </summary>
		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TreeTaskQueueLockAsync>
		{
			protected override void Execute(TreeTaskQueueLockAsync self, TokenState state)
			{
				if (state == TokenState.Cancel)
					self.CancelFromToken();
			}
		}
	}
}
