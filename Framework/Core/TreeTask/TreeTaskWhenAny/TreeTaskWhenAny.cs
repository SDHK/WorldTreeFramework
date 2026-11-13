/****************************************

* 作者：闪电黑客
* 日期：2026/11/10

* 描述：多个任务谁先完成，返回的任务就完成。门是这些任务唯一的等待者。

*/
using System;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 谁先完成，返回的任务就完成。
	/// </summary>
	/// <remarks>
	/// 最快的那条如果失败，异常从这里抛出。其余任务的结果在各自的 Start 里取走。
	/// </remarks>
	public static class TreeTaskWhenAny
	{
		/// <summary>
		/// 最快的一条完成时返回。
		/// </summary>
		/// <remarks>
		/// 调用时还不知道是谁，所以这是一条新任务。
		/// </remarks>
		public static async TreeTask WhenAny(this INode self, params TreeTask[] tasks)
		{
			await self.TreeTaskCompleted();
			TreeTaskWhenAll.Check(tasks);
			if (tasks.Length == 0)
				throw new InvalidOperationException("没有可以等待的任务");

			TreeTask signal = TreeTaskManager.GetTask(self);
			// 已经完成的先启动。await 会当场取走结果，数组里第一条就是赢家。
			for (int i = 0; i < tasks.Length; i++)
			{
				if (!tasks[i].IsCompleted) continue;
				Start(tasks[i], signal).Coroutine();
			}
			for (int i = 0; i < tasks.Length; i++)
			{
				TreeTask task = tasks[i];
				if (task.IsCompleted || task.IsDisposed || task.InstanceId == 0) continue;
				Start(task, signal).Coroutine();
			}
			await signal;
		}

		/// <summary>
		/// 最快的一条完成时返回它的结果。
		/// </summary>
		/// <remarks>
		/// 调用时还不知道是谁，所以这是一条新任务。
		/// </remarks>
		public static async TreeTask<T> WhenAny<T>(this INode self, params TreeTask<T>[] tasks)
		{
			await self.TreeTaskCompleted();
			TreeTaskWhenAll.Check(tasks);
			if (tasks.Length == 0)
				throw new InvalidOperationException("没有可以等待的任务");

			TreeTask<T> signal = TreeTaskManager.GetTask<T>(self);
			for (int i = 0; i < tasks.Length; i++)
			{
				if (!tasks[i].IsCompleted) continue;
				Start(tasks[i], signal).Coroutine();
			}
			for (int i = 0; i < tasks.Length; i++)
			{
				TreeTask<T> task = tasks[i];
				if (task.IsCompleted || task.IsDisposed || task.InstanceId == 0) continue;
				Start(task, signal).Coroutine();
			}
			return await signal;
		}

		/// <summary>
		/// 等这一条输入。
		/// </summary>
		/// <remarks>
		/// await 会把它启动，结束后再决定要不要写 signal。
		/// </remarks>
		private static async TreeTaskVoid Start(TreeTask task, TreeTask signal)
		{
			// 包一层。还池或被别人拿走后，引用变空，不会写到新节点上。
			NodeRef<TreeTask> signalRef = signal;
			try
			{
				await task;
				if (!CanSet(signalRef)) return;
				signalRef.Value.SetResult();
			}
			catch (Exception exception)
			{
				// await 失败时到不了上面。不写的话 signal 会一直等。
				if (!CanSet(signalRef)) return;
				signalRef.Value.SetException(exception);
			}
		}

		/// <summary>
		/// 等这一条输入。
		/// </summary>
		/// <remarks>
		/// await 会把它启动，结束后再把结果交给 signal。
		/// </remarks>
		private static async TreeTaskVoid Start<T>(TreeTask<T> task, TreeTask<T> signal)
		{
			NodeRef<TreeTask<T>> signalRef = signal;
			try
			{
				T value = await task;
				if (!CanSet(signalRef)) return;
				signalRef.Value.SetResult(value);
			}
			catch (Exception exception)
			{
				if (!CanSet(signalRef)) return;
				signalRef.Value.SetException(exception);
			}
		}

		/// <summary>
		/// 判断还能不能写。
		/// </summary>
		/// <remarks>
		/// 还是原来那个节点，并且没人写过，才可以设置。
		/// </remarks>
		private static bool CanSet<N>(NodeRef<N> signal)
			where N : TreeTaskBase
		{
			N value = signal.Value;
			return value != null && !value.IsDisposed && !value.IsCompleted;
		}
	}
}
