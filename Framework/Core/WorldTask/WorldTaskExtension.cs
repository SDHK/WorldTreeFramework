/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// WorldTask 扩展
	/// </summary>
	public static class WorldTaskExtension
	{
		/// <summary>
		/// 同步完成的一次 await
		/// </summary>
		public static WorldTaskCompleted WorldTaskCompleted(this IUnit self)
			=> self.World.PoolGetUnit<WorldTaskCompleted>();

		/// <summary>
		/// 读取当前异步链上的令牌。链上没有令牌时结果是空。
		/// </summary>
		public static WorldTaskTokenCatch WorldTaskTokenCatch(this IUnit self)
			=> self.World.PoolGetUnit<WorldTaskTokenCatch>();

		/// <summary>
		/// 后续代码改到目标世界线的线程上执行
		/// </summary>
		public static WorldTaskSwitchWorld SwitchWorld(this IUnit self, WorldContext worldContext)
		{
			WorldTaskSwitchWorld task = self.World.PoolGetUnit<WorldTaskSwitchWorld>();
			task.Target = worldContext;
			return task;
		}

		/// <summary>
		/// 真实时间延迟。到期由 CascadeTicker 执行 WorldTaskTimeout。
		/// </summary>
		public static async WorldTask AsyncDelay(this IUnit self, TimeSpan time)
		{
			if (time <= TimeSpan.Zero)
			{
				await self.WorldTaskCompleted();
				return;
			}

			WorldTaskToken token = await self.WorldTaskTokenCatch();
			if (token != null && token.State == WorldTokenState.Cancel)
			{
				await self.WorldTaskCompleted();
				return;
			}

			WorldTask task = self.World.PoolGetUnit<WorldTask>();
			long id = self.World.Line.WorldTimeManager.AddTimerDelay<WorldTaskTimeout>(time.Ticks, task);

			Action<WorldTokenState> onToken = null;
			if (token != null)
			{
				onToken = state =>
				{
					if (state != WorldTokenState.Cancel) return;
					if (id != -1) self.World.Line.WorldTimeManager.Timer.RemoveTicker(id);
					if (!task.IsCompleted) task.SetCompleted();
				};
				token.Add(onToken);
			}

			await task;
			token?.Remove(onToken);
		}

		/// <summary>
		/// 到时取消令牌，单位是秒。到期由 CascadeTicker 执行 WorldTaskTimeout。
		/// </summary>
		public static void TimeOut(this WorldTaskToken self, float timeOut)
		{
			if (self.State == WorldTokenState.Cancel) return;
			if (timeOut <= 0)
			{
				self.Cancel();
				return;
			}
			self.World.Line.WorldTimeManager.AddTimerDelay<WorldTaskTimeout>(TimeSpan.FromSeconds(timeOut).Ticks, self);
		}

		/// <summary>
		/// 等待 BCL 任务，完成和异常都回到当前世界线
		/// </summary>
		public static async WorldTask GetAwaiter(this IUnit self, Task task)
		{
			if (task.IsCompleted)
			{
				await self.WorldTaskCompleted();
				ThrowIfFaulted(task);
				return;
			}

			WorldTask worldTask = self.World.PoolGetUnit<WorldTask>();
			task.GetAwaiter().OnCompleted(() =>
			{
				self.World.Line.WorldContext.Post(() => CompleteFromTask(worldTask, task));
			});
			await worldTask;
		}

		/// <summary>
		/// 等待带返回值的 BCL 任务，完成和异常都回到当前世界线
		/// </summary>
		public static async WorldTask<T> GetAwaiter<T>(this IUnit self, Task<T> task)
		{
			if (task.IsCompleted)
			{
				await self.WorldTaskCompleted();
				ThrowIfFaulted(task);
				return task.Result;
			}

			WorldTask<T> worldTask = self.World.PoolGetUnit<WorldTask<T>>();
			task.GetAwaiter().OnCompleted(() =>
			{
				self.World.Line.WorldContext.Post(() =>
				{
					if (worldTask.IsCompleted) return;
					if (task.IsFaulted) worldTask.SetException(Unwrap(task.Exception));
					else if (task.IsCanceled) worldTask.SetException(new TaskCanceledException(task));
					else worldTask.SetResult(task.Result);
				});
			});
			return await worldTask;
		}

		/// <summary>
		/// 在世界线上完成对 BCL 任务的等待
		/// </summary>
		private static void CompleteFromTask(WorldTask worldTask, Task task)
		{
			if (worldTask.IsCompleted) return;
			if (task.IsFaulted) worldTask.SetException(Unwrap(task.Exception));
			else if (task.IsCanceled) worldTask.SetException(new TaskCanceledException(task));
			else worldTask.SetResult();
		}

		/// <summary>
		/// 已完成的 BCL 任务如果失败，就抛出原来的异常
		/// </summary>
		private static void ThrowIfFaulted(Task task)
		{
			if (task.IsFaulted) ExceptionDispatchInfo.Capture(Unwrap(task.Exception)).Throw();
			if (task.IsCanceled) ExceptionDispatchInfo.Capture(new TaskCanceledException(task)).Throw();
		}

		/// <summary>
		/// 拆开只包了一层的 AggregateException
		/// </summary>
		private static Exception Unwrap(Exception exception)
		{
			if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1)
				return aggregate.InnerExceptions[0];
			return exception;
		}
	}
}
