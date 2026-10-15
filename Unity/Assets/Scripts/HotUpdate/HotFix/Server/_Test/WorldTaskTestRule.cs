/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;
using System.Threading.Tasks;

namespace WorldTree.Server
{
	/// <summary>
	/// WorldTask 异步任务测试规则
	/// </summary>
	public static partial class WorldTaskTestRule
	{
		[NodeRule(nameof(AddRule<WorldTaskTest>))]
		private static void OnAddRule(this WorldTaskTest self)
		{
			self.Log("======== WorldTask 测试开始 ========");
			self.World.PoolGetUnit(out WorldTaskToken token);
			Run(self, token).Coroutine(token);
		}

		/// <summary>
		/// 依次执行所有用例
		/// </summary>
		private static async WorldTask Run(WorldTaskTest self, WorldTaskToken token)
		{
			self.Log("---- [1] 同步任务与嵌套 await ----");
			int sum = await NestedAdd(self, 3, 4);
			self.Log($"[1] 嵌套 await 结果 {sum}");

			self.Log("---- [2] AsyncDelay 真实时间 ----");
			await RealDelay(self, 1.5f);

			self.Log("---- [3] BCL Task 成功与失败 ----");
			await BridgeTask(self);

			self.Log("---- [4] 令牌取消等待 ----");
			await CancelWait(self, token);

			self.Log("---- [5] 异常沿 await 链传播 ----");
			await CatchException(self);

			self.Log("---- [6] 切回当前世界线 ----");
			await SwitchSameLine(self);

			self.Log("======== WorldTask 测试结束 ========");
		}

		/// <summary>
		/// 内层方法：同步完成后返回两数之和
		/// </summary>
		private static async WorldTask<int> NestedAdd(WorldTaskTest self, int left, int right)
		{
			await self.WorldTaskCompleted();
			return left + right;
		}

		/// <summary>
		/// 真实时间延迟
		/// </summary>
		private static async WorldTask RealDelay(WorldTaskTest self, float seconds)
		{
			DateTime startTime = DateTime.UtcNow;
			await self.AsyncDelay(TimeSpan.FromSeconds(seconds));
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[2] 请求 {seconds} 秒，实际耗时 {elapsed:F0} 毫秒");
		}

		/// <summary>
		/// BCL 任务桥接
		/// </summary>
		private static async WorldTask BridgeTask(WorldTaskTest self)
		{
			int value = await self.GetAwaiter(Task.FromResult(42));
			self.Log($"[3] Task.FromResult 结果 {value}");

			try
			{
				await self.GetAwaiter(Task.FromException(new InvalidOperationException("WorldTask 桥接异常")));
				self.Log("[3] 失败：异常没有抛出");
			}
			catch (InvalidOperationException e)
			{
				self.Log($"[3] 捕获桥接异常 {e.Message}");
			}
		}

		/// <summary>
		/// 短定时取消一个长等待
		/// </summary>
		private static async WorldTask CancelWait(WorldTaskTest self, WorldTaskToken token)
		{
			token.TimeOut(0.5f);
			DateTime startTime = DateTime.UtcNow;
			await self.AsyncDelay(TimeSpan.FromSeconds(5));
			double elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
			self.Log($"[4] 请求等待 5 秒，实际 {elapsed:F0} 毫秒，令牌状态 {token.State}");
		}

		/// <summary>
		/// 内层抛异常
		/// </summary>
		private static async WorldTask<int> ThrowInside(WorldTaskTest self)
		{
			await self.WorldTaskCompleted();
			throw new InvalidOperationException("WorldTask 测试异常");
		}

		/// <summary>
		/// 调用方接住内层异常
		/// </summary>
		private static async WorldTask CatchException(WorldTaskTest self)
		{
			try
			{
				int value = await ThrowInside(self);
				self.Log($"[5] 没有异常，结果 {value}");
			}
			catch (Exception e)
			{
				self.Log($"[5] 捕获到异常 {e.GetType().Name}: {e.Message}");
			}
		}

		/// <summary>
		/// 切到当前世界线，确认会继续执行
		/// </summary>
		private static async WorldTask SwitchSameLine(WorldTaskTest self)
		{
			int threadId = Environment.CurrentManagedThreadId;
			await self.SwitchWorld(self.World.Line.WorldContext);
			self.Log($"[6] 切换完成，线程 {threadId} -> {Environment.CurrentManagedThreadId}");
		}
	}
}
