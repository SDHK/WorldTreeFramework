/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 对类回调的异步扩展
	/// </summary>
	public static class TreeTaskExtension
	{
		/// <summary>
		/// 同步完成
		/// </summary>
		public static TreeTaskCompleted TreeTaskCompleted(this IUnit self)
		{
			return TreeTaskManager.Get(self as INode ?? self.World).AddTemp(out TreeTaskCompleted _);
		}

		/// <summary>
		/// 同步捕获令牌
		/// </summary>
		public static TreeTaskTokenCatch TreeTaskTokenCatch(this INode self)
		{
			return TreeTaskManager.Get(self).AddTemp(out TreeTaskTokenCatch _);
		}

		/// <summary>
		/// 树任务连接
		/// </summary>
		public static TreeTaskLink TreeTaskLink(this INode self, Task task)
		{
			TreeTaskManager.Get(self).AddTemp(out TreeTaskLink treeTaskBox);
			treeTaskBox.Task = task;
			return treeTaskBox;
		}

		/// <summary>
		/// 树任务连接
		/// </summary>
		public static TreeTaskLink<T> TreeTaskLink<T>(this INode self, Task<T> task)
		{
			TreeTaskManager.Get(self).AddTemp(out TreeTaskLink<T> treeTaskBox);
			treeTaskBox.Task = task;
			return treeTaskBox;
		}

		/// <summary>
		/// 等待 BCL 任务。完成、失败、取消都回到当前世界线。
		/// </summary>
		public static async TreeTask GetAwaiter(this INode self, Task task)
		{
			if (task.IsCompleted)
			{
				await self.TreeTaskCompleted();
				ThrowIfFaulted(task);
				return;
			}

			TreeTask treeTask = TreeTaskManager.Get(self).AddTemp(out TreeTask _);
			task.GetAwaiter().OnCompleted(() =>
			{
				treeTask.World.Line.WorldContext.Post(() => CompleteFromTask(treeTask, task));
			});
			await treeTask;
		}

		/// <summary>
		/// 等待带返回值的 BCL 任务。完成、失败、取消都回到当前世界线。
		/// </summary>
		public static async TreeTask<T> GetAwaiter<T>(this INode self, Task<T> task)
		{
			if (task.IsCompleted)
			{
				await self.TreeTaskCompleted();
				ThrowIfFaulted(task);
				return task.Result;
			}

			TreeTask<T> treeTask = TreeTaskManager.Get(self).AddTemp(out TreeTask<T> _);
			task.GetAwaiter().OnCompleted(() =>
			{
				treeTask.World.Line.WorldContext.Post(() =>
				{
					if (treeTask.IsCompleted) return;
					if (task.IsFaulted) treeTask.SetException(Unwrap(task.Exception));
					else if (task.IsCanceled) treeTask.SetException(new TaskCanceledException(task));
					else treeTask.SetResult(task.Result);
				});
			});
			return await treeTask;
		}

		/// <summary>
		/// 在世界线上结束对 BCL 任务的等待
		/// </summary>
		private static void CompleteFromTask(TreeTask treeTask, Task task)
		{
			if (treeTask.IsCompleted) return;
			if (task.IsFaulted) treeTask.SetException(Unwrap(task.Exception));
			else if (task.IsCanceled) treeTask.SetException(new TaskCanceledException(task));
			else treeTask.SetResult();
		}

		/// <summary>
		/// 已经结束的 BCL 任务如果失败或取消，抛出原来的异常
		/// </summary>
		private static void ThrowIfFaulted(Task task)
		{
			if (task.IsFaulted) ExceptionDispatchInfo.Capture(Unwrap(task.Exception)).Throw();
			if (task.IsCanceled) ExceptionDispatchInfo.Capture(new TaskCanceledException(task)).Throw();
		}

		/// <summary>
		/// 只包了一层的 AggregateException 拆开
		/// </summary>
		private static Exception Unwrap(Exception exception)
		{
			if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1)
				return aggregate.InnerExceptions[0];
			return exception;
		}

		/// <summary>
		/// 令牌超时取消 （秒）
		/// </summary>
		public static async TreeTask TimeOut(this TreeTaskToken self, float timeOut)
		{
			if (timeOut <= 0 || self.State == TokenState.Cancel)
			{
				await self.TreeTaskCompleted();
			}

			TreeTask asyncTask = TreeTaskManager.Get(self).AddTemp(out TreeTask _);
			self.World.Line.WorldTimeManager.AddTimerDelay<TreeTaskSetResuIt>(TimeSpan.FromSeconds(timeOut).Ticks, asyncTask);
			await asyncTask;

			if (self.State == TokenState.Cancel) return;

			self.Cancel();
		}

		/// <summary>
		/// 超时任务 （秒）
		/// </summary>

		public static async TreeTask TimeOut(this TreeTask self, float timeOut)
		{
			TreeTaskManager.Get(self).AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(timeOut).Coroutine();
			await self.AddToken(treeTaskToken);
		}

		/// <summary>
		/// 超时任务 （秒）
		/// </summary>
		public static async TreeTask<T> TimeOut<T>(this TreeTask<T> self, float timeOut)
		{
			TreeTaskManager.Get(self).AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(timeOut).Coroutine();
			return await self.AddToken(treeTaskToken);
		}
	}
}