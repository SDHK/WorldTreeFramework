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
	}
}
