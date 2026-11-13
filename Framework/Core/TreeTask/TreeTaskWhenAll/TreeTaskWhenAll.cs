/****************************************

* 作者：闪电黑客
* 日期：2026/11/10

* 描述：等一组任务全部结束。每条输入各自启动，全部结束再写 signal。

*/
using System;
using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 等一组任务全部结束。
	/// </summary>
	/// <remarks>
	/// 这些任务只能交给这里等，外面等的是返回的那条任务。
	/// </remarks>
	public static class TreeTaskWhenAll
	{
		/// <summary>
		/// 全部完成后返回。
		/// </summary>
		/// <remarks>
		/// 有失败时先把其余任务取完，再抛出第一条异常。取完是为了让任务节点回到池里。
		/// </remarks>
		public static async TreeTask WhenAll(this INode self, params TreeTask[] tasks)
		{
			await self.TreeTaskCompleted();
			Check(tasks);
			if (tasks.Length == 0) return;

			TreeTask signal = TreeTaskManager.GetTask(self);
			// 计数在闭包的字段上，不是装箱。每条 Start 改的是同一个 left。
			NodeRef<TreeTask> signalRef = signal;
			int left = tasks.Length;
			Exception first = null;

			// 一条输入结束。全部到齐才写 signal，失败时写第一条异常。
			void Arrive(Exception exception)
			{
				if (exception != null && first == null) first = exception;
				left--;
				if (left != 0) return;
				if (!CanSet(signalRef)) return;
				if (first != null) signalRef.Value.SetException(first);
				else signalRef.Value.SetResult();
			}

			// await 会把这条输入启动。失败也要报到 Arrive，否则计数少一次。
			async TreeTaskVoid StartOne(TreeTask task)
			{
				try
				{
					await task;
					Arrive(null);
				}
				catch (Exception exception)
				{
					Arrive(exception);
				}
			}

			// 每条马上启动。排着 await 的话，下一条要等前一条结束才开始跑。
			for (int i = 0; i < tasks.Length; i++)
				StartOne(tasks[i]).Coroutine();
			await signal;
		}

		/// <summary>
		/// 全部完成后按原来的顺序返回结果。
		/// </summary>
		/// <remarks>
		/// 有失败时先把其余任务取完，再抛出第一条异常。
		/// </remarks>
		public static async TreeTask<T[]> WhenAll<T>(this INode self, params TreeTask<T>[] tasks)
		{
			await self.TreeTaskCompleted();
			Check(tasks);
			if (tasks.Length == 0) return new T[0];

			TreeTask<T[]> signal = TreeTaskManager.GetTask<T[]>(self);
			NodeRef<TreeTask<T[]>> signalRef = signal;
			T[] results = new T[tasks.Length];
			int left = tasks.Length;
			Exception first = null;

			// 一条输入结束。结果按下标放下，全部到齐才写 signal。
			void Arrive(int index, T value, Exception exception)
			{
				if (exception != null)
				{
					if (first == null) first = exception;
				}
				else results[index] = value;
				left--;
				if (left != 0) return;
				if (!CanSet(signalRef)) return;
				if (first != null) signalRef.Value.SetException(first);
				else signalRef.Value.SetResult(results);
			}

			// 下标作为参数传进来。写 tasks[i] 会抓住循环变量，结束时全指到最后一条。
			async TreeTaskVoid StartOne(TreeTask<T> task, int index)
			{
				try
				{
					T value = await task;
					Arrive(index, value, null);
				}
				catch (Exception exception)
				{
					Arrive(index, default, exception);
				}
			}

			for (int i = 0; i < tasks.Length; i++)
				StartOne(tasks[i], i).Coroutine();
			return await signal;
		}

		/// <summary>
		/// 还没等之前先看一遍。
		/// </summary>
		/// <remarks>
		/// 空数组可以通过。空元素、已释放、重复的任务直接拒绝，避免第二次等待挂死。WhenAny 也用这一套。
		/// </remarks>
		internal static void Check<TTask>(TTask[] tasks)
			where TTask : TreeTaskBase
		{
			if (tasks == null) throw new ArgumentNullException(nameof(tasks));
			for (int i = 0; i < tasks.Length; i++)
			{
				TTask task = tasks[i];
				if (task == null) throw new ArgumentNullException(nameof(tasks));
				if (task.IsDisposed || task.InstanceId == 0)
					throw new InvalidOperationException("任务已经释放");
				for (int j = 0; j < i; j++)
				{
					if (ReferenceEquals(task, tasks[j]))
						throw new InvalidOperationException("同一个任务不能等待两次");
				}
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
