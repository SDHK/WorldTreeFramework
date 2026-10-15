/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;
using System.Threading;
using System.Threading.Tasks;
using WorldTree.Internal;

namespace WorldTree.Server
{
	/// <summary>
	/// TreeTask 异步任务测试规则
	/// </summary>
	public static partial class TreeTaskTestRule
	{
		[NodeRule(nameof(AddRule<TreeTaskTest>))]
		private static void OnAddRule(this TreeTaskTest self)
		{
			self.Log("======== TreeTask 测试开始 ========");
			//令牌挂在根任务上，会顺着关联任务链传播到所有 await 出去的任务
			self.AddTemp(out TreeTaskToken token);
			Run(self, token).Coroutine(token);
		}

		/// <summary>
		/// 依次执行所有用例
		/// </summary>
		private static async TreeTask Run(TreeTaskTest self, TreeTaskToken token)
		{
			self.Log($"---- [1] 同步任务与嵌套 await，线程 {Environment.CurrentManagedThreadId} ----");
			int sum = await NestedAdd(self, 3, 4);
			self.Log($"[1] 嵌套 await 结果 {sum}");

			self.Log("---- [2] AsyncYield 逐帧等待 ----");
			await YieldFrames(self, 3);

			self.Log("---- [3] AsyncDelay 真实时间等待 ----");
			await RealDelay(self, 1.5f);

			self.Log("---- [4] BCL Task 桥接 ----");
			await BridgeTask(self);

			self.Log("---- [5] TreeTaskLink 连接 BCL Task ----");
			await BridgeLink(self);

			self.Log("---- [6] 令牌超时取消等待 ----");
			await CancelWait(self, token);

			self.Log("---- [7] 异常沿 await 链传播 ----");
			await CatchException(self);

			self.Log("---- [8] 令牌沿任务链传递并取消 ----");
			await TokenPropagate(self);

			self.Log("---- [9] 子令牌衔接主令牌 ----");
			await TokenLink(self);

			self.Log("---- [10] while 循环 ----");
			await WhileLoop(self);

			self.Log("---- [11] 队列锁 ----");
			self.AddTemp(out TreeTaskToken lockToken);
			//队列锁全是同步操作，协程一驱动就会跑到尾，不用再 await（TreeTask 的 GetResult 会还池，双重 await 会挂住）
			QueueLock(self).Coroutine(lockToken);

			self.Log("======== TreeTask 测试结束 ========");
		}

		/// <summary>
		/// 内层方法：首个 await 是同步任务，返回两数之和
		/// </summary>
		private static async TreeTask<int> NestedAdd(TreeTaskTest self, int left, int right)
		{
			await self.TreeTaskCompleted();
			return left + right;
		}

		/// <summary>
		/// 逐帧等待，观察帧数推进
		/// </summary>
		private static async TreeTask YieldFrames(TreeTaskTest self, int frameCount)
		{
			for (int i = 0; i < frameCount; i++)
			{
				long frame = self.World.Line.WorldTimeManager.TotalFrames;
				self.Log($"[2] 第 {i + 1} 次等待，当前总帧 {frame}");
				await self.AsyncYield();
			}
		}

		/// <summary>
		/// 真实时间延迟，验证 AsyncDelay 会按时完成
		/// </summary>
		private static async TreeTask RealDelay(TreeTaskTest self, float seconds)
		{
			DateTime startTime = DateTime.UtcNow;
			await self.AsyncDelay(TimeSpan.FromSeconds(seconds));
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[3] 请求 {seconds} 秒，实际耗时 {elapsed:F0} 毫秒");
		}

		/// <summary>
		/// BCL Task 桥接：完成后回到世界线线程
		/// </summary>
		private static async TreeTask BridgeTask(TreeTaskTest self)
		{
			int lineThread = Environment.CurrentManagedThreadId;
			await self.GetAwaiter(Task.Run(() => Thread.Sleep(300)));
			self.Log($"[4] Task 完成，线程 {lineThread} -> {Environment.CurrentManagedThreadId}");

			int value = await self.GetAwaiter(Task.FromResult(42));
			self.Log($"[4] Task.FromResult 结果 {value}");

			try
			{
				await self.GetAwaiter(Task.FromException(new InvalidOperationException("TreeTask 桥接异常")));
				self.Log("[4] 失败任务没有抛出");
			}
			catch (InvalidOperationException e)
			{
				self.Log($"[4] 捕获桥接异常 {e.Message}");
			}

			try
			{
				await self.GetAwaiter(Task.FromCanceled(new CancellationToken(true)));
				self.Log("[4] 取消任务没有抛出");
			}
			catch (TaskCanceledException)
			{
				self.Log("[4] 捕获桥接取消 TaskCanceledException");
			}

			TaskCompletionSource<int> source = new TaskCompletionSource<int>();
			TreeTask<int> waiting = self.GetAwaiter(source.Task);
			source.SetException(new InvalidOperationException("TreeTask 延迟异常"));
			try
			{
				await waiting;
				self.Log("[4] 延迟失败没有抛出");
			}
			catch (InvalidOperationException e)
			{
				self.Log($"[4] 捕获延迟异常 {e.Message}");
			}
		}

		/// <summary>
		/// TreeTaskLink 连接 BCL Task
		/// </summary>
		private static async TreeTask BridgeLink(TreeTaskTest self)
		{
			DateTime startTime = DateTime.UtcNow;
			TreeTaskLink link = self.TreeTaskLink(Task.Run(() => Thread.Sleep(400)));
			await link;
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[5] TreeTaskLink 完成，耗时 {elapsed:F0} 毫秒");

			TreeTaskLink<int> linkValue = self.TreeTaskLink(Task.FromResult(123));
			int value = await linkValue;
			self.Log($"[5] TreeTaskLink<int> 结果 {value}");

			try
			{
				await self.TreeTaskLink(Task.FromException(new InvalidOperationException("TreeTaskLink 已失败")));
				self.Log("[5] 已失败链接没有抛出");
			}
			catch (InvalidOperationException e)
			{
				self.Log($"[5] 捕获已失败链接 {e.Message}");
			}

			TaskCompletionSource<int> source = new TaskCompletionSource<int>();
			TreeTaskLink<int> lateLink = self.TreeTaskLink(source.Task);
			ThreadPool.QueueUserWorkItem(state =>
			{
				Thread.Sleep(50);
				source.SetException(new InvalidOperationException("TreeTaskLink 延迟异常"));
			});
			try
			{
				await lateLink;
				self.Log("[5] 延迟链接没有抛出");
			}
			catch (InvalidOperationException e)
			{
				self.Log($"[5] 捕获延迟链接 {e.Message}");
			}
		}

		/// <summary>
		/// 令牌超时取消：等待被提前打断
		/// </summary>
		private static async TreeTask CancelWait(TreeTaskTest self, TreeTaskToken token)
		{
			//1 秒后取消令牌
			token.TimeOut(1f).Coroutine();

			DateTime startTime = DateTime.UtcNow;
			await self.AsyncDelay(TimeSpan.FromSeconds(5));
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[6] 请求等待 5 秒，实际 {elapsed:F0} 毫秒，令牌状态 {token.State}");
		}

		/// <summary>
		/// 内层方法：等待同步任务后抛异常
		/// </summary>
		private static async TreeTask<int> ThrowInside(TreeTaskTest self)
		{
			await self.TreeTaskCompleted();
			throw new InvalidOperationException("TreeTask 测试异常");
		}

		/// <summary>
		/// 异常沿 await 链传播到调用方
		/// </summary>
		private static async TreeTask CatchException(TreeTaskTest self)
		{
			try
			{
				int value = await ThrowInside(self);
				self.Log($"[7] 没有异常，结果 {value}");
			}
			catch (Exception e)
			{
				self.Log($"[7] 捕获到异常 {e.GetType().Name}: {e.Message}");
			}
		}

		/// <summary>
		/// 根令牌顺着 await 传到内层，超时后最内层等待被打断
		/// </summary>
		private static async TreeTask TokenPropagate(TreeTaskTest self)
		{
			self.AddTemp(out TreeTaskToken root);
			TreeTask chain = PropagateBody(self, root);
			chain.SetToken(root);
			root.TimeOut(1f).Coroutine();
			DateTime startTime = DateTime.UtcNow;
			await chain;
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[8] 整链耗时 {elapsed:F0} 毫秒，根令牌 {root.State}");
		}

		/// <summary>
		/// 停在同步点，等外部把令牌设上再往下走
		/// </summary>
		private static async TreeTask PropagateBody(TreeTaskTest self, TreeTaskToken root)
		{
			await self.TreeTaskCompleted();
			await PropagateMid(self, root);
		}

		/// <summary>
		/// 中间层确认拿到的是根令牌
		/// </summary>
		private static async TreeTask PropagateMid(TreeTaskTest self, TreeTaskToken root)
		{
			TreeTaskToken token = await self.TreeTaskTokenCatch();
			self.Log($"[8] 中层令牌 {token?.Id} 是根 {token == root}");
			await PropagateInner(self, root);
			self.Log($"[8] 中层结束 {token?.State}");
		}

		/// <summary>
		/// 最内层长等待，应由根令牌提前打断
		/// </summary>
		private static async TreeTask PropagateInner(TreeTaskTest self, TreeTaskToken root)
		{
			TreeTaskToken token = await self.TreeTaskTokenCatch();
			self.Log($"[8] 内层令牌 {token?.Id} 是根 {token == root}");
			await self.AsyncDelay(TimeSpan.FromSeconds(5));
			self.Log($"[8] 内层结束 {token?.State}");
		}

		/// <summary>
		/// 子令牌接到主令牌上，主令牌超时后两边一起取消
		/// </summary>
		private static async TreeTask TokenLink(TreeTaskTest self)
		{
			self.AddTemp(out TreeTaskToken parent);
			self.AddTemp(out TreeTaskToken child);
			parent.Add(child);
			TreeTask chain = LinkBody(self, child);
			chain.SetToken(child);
			parent.TimeOut(1f).Coroutine();
			DateTime startTime = DateTime.UtcNow;
			await chain;
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[9] 衔接耗时 {elapsed:F0} 毫秒，主 {parent.State} 子 {child.State}");
		}

		/// <summary>
		/// 停在同步点后，用子令牌做一次长等待
		/// </summary>
		private static async TreeTask LinkBody(TreeTaskTest self, TreeTaskToken child)
		{
			await self.TreeTaskCompleted();
			TreeTaskToken token = await self.TreeTaskTokenCatch();
			self.Log($"[9] 子链令牌 {token?.Id} 是子 {token == child}");
			await self.AsyncDelay(TimeSpan.FromSeconds(5));
			self.Log($"[9] 子链结束 {token?.State}");
		}

		/// <summary>
		/// 同步循环立刻跑完，等待循环靠令牌停，避免一直占着心跳
		/// </summary>
		private static async TreeTask WhileLoop(TreeTaskTest self)
		{
			self.AddTemp(out TreeTaskToken token);
			TreeTask chain = WhileBody(self);
			chain.SetToken(token);
			token.TimeOut(1f).Coroutine();
			DateTime startTime = DateTime.UtcNow;
			await chain;
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[10] 循环结束 {elapsed:F0} 毫秒，令牌 {token.State}");
		}

		/// <summary>
		/// 先空转几十次同步等待，再按令牌做有限次真实等待
		/// </summary>
		private static async TreeTask WhileBody(TreeTaskTest self)
		{
			await self.TreeTaskCompleted();
			TreeTaskToken token = await self.TreeTaskTokenCatch();
			int syncCount = 0;
			while (syncCount < 32)
			{
				await self.TreeTaskCompleted();
				syncCount++;
			}

			int waitCount = 0;
			while (token != null && token.State != TokenState.Cancel && waitCount < 5)
			{
				waitCount++;
				await self.AsyncDelay(TimeSpan.FromSeconds(0.3));
			}
			self.Log($"[10] 同步 {syncCount} 次，等待 {waitCount} 次，令牌 {token?.State}");
		}

		/// <summary>
		/// 同键排队，异键互不影响，取消的等待不占锁
		/// </summary>
		private static async TreeTask QueueLock(TreeTaskTest self)
		{
			const long key = 7;
			const long other = 8;
			int[] orders = new int[1];

			TreeTaskLock first = await self.AsyncLock(key);
			self.Log($"[11] 先拿到锁 {first != null}");

			//三个等待都协程驱动，不 await。它们要么同步完成，要么在 first.Dispose 放行时同步完成
			EnterQueue(self, key, "同键", orders).Coroutine();
			EnterQueue(self, other, "异键", orders).Coroutine();

			self.AddTemp(out TreeTaskToken token);
			EnterQueue(self, key, "取消", orders).Coroutine(token);
			token.Cancel();

			first?.Dispose();
			self.Log($"[11] 结束 顺序 {orders[0]}");

			//先让一个拿过锁的等待节点回到池顶，下一轮取消复用的就是它
			TreeTaskLock pad = await self.AsyncLock(key);
			EnterQueue(self, key, "垫", orders).Coroutine();
			pad?.Dispose();

			int mark = orders[0];
			TreeTaskLock held = await self.AsyncLock(key);
			self.AddTemp(out TreeTaskToken reuseToken);
			EnterQueue(self, key, "复用取消", orders).Coroutine(reuseToken);
			reuseToken.Cancel();
			EnterQueue(self, key, "复用等待", orders).Coroutine();
			bool alive = held != null && held.InstanceId != 0;
			bool stayed = orders[0] == mark;
			self.Log($"[11] 复用 锁还在 {alive} 无人进入 {stayed}");
			if (alive) held.Dispose();
			self.Log($"[11] 复用放行 {orders[0] == mark + 1}");
		}

		/// <summary>
		/// 排队进入一把锁，拿到后记一次顺序
		/// </summary>
		private static async TreeTask EnterQueue(TreeTaskTest self, long lockKey, string name, int[] orders)
		{
			TreeTaskLock hold = await self.AsyncLock(lockKey);
			if (hold == null)
			{
				self.Log($"[11] {name} 没拿到");
				return;
			}
			using (hold)
			{
				orders[0]++;
				self.Log($"[11] {name} 第 {orders[0]} 个");
			}
		}
	}
}
