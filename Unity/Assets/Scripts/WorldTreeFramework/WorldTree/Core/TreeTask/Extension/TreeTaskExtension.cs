/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
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
		public static TreeTaskCompleted TreeTaskCompleted(this INode self)
		{
			return self.AddTemp(out TreeTaskCompleted _);
		}

		/// <summary>
		/// 同步捕获令牌
		/// </summary>
		public static TreeTaskTokenCatch TreeTaskTokenCatch(this INode self)
		{
			return self.AddTemp(out TreeTaskTokenCatch _);
		}

		/// <summary>
		/// 树任务连接
		/// </summary>
		public static TreeTaskLink TreeTaskLink(this INode self, Task task)
		{
			self.AddTemp(out TreeTaskLink treeTaskBox);
			treeTaskBox.Task = task;
			return treeTaskBox;
		}

		/// <summary>
		/// 树任务连接
		/// </summary>
		public static TreeTaskLink<T> TreeTaskLink<T>(this INode self, Task<T> task)
		{
			self.AddTemp(out TreeTaskLink<T> treeTaskBox);
			treeTaskBox.Task = task;
			return treeTaskBox;
		}

		/// <summary>
		/// 异步等待
		/// </summary>
		public static async TreeTask GetAwaiter(this INode self, Task task)
		{
			if (task.IsCompleted)
			{
				await self.TreeTaskCompleted();
			}
			else
			{
				TreeTask treeTask = self.AddTemp(out TreeTask _);
				task.GetAwaiter().OnCompleted(() =>
				{
					treeTask.Core.WorldContext.Post(treeTask.SetResult);
				});

				await treeTask;
			}
		}

		/// <summary>
		/// 异步等待
		/// </summary>
		public static async TreeTask<T> GetAwaiter<T>(this INode self, Task<T> task)
		{
			if (task.IsCompleted)
			{
				await self.TreeTaskCompleted();
				return task.Result;
			}
			else
			{
				TreeTask<T> treeTask = self.AddTemp(out TreeTask<T> _);
				task.GetAwaiter().OnCompleted(() =>
				{
					treeTask.Core.WorldContext.Post((x) => treeTask.SetResult((T)x.Object), new(task.Result));
				});

				return await treeTask;
			}
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

			await self.AsyncDelay(timeOut);

			if (self.State == TokenState.Cancel) return;

			self.Cancel();
		}

		/// <summary>
		/// 超时任务
		/// </summary>

		public static async TreeTask TimeOut(this TreeTask self, float timeOut)
		{
			self.Parent.AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(timeOut).Coroutine();
			await self.AddToken(treeTaskToken);
		}

		/// <summary>
		/// 超时任务
		/// </summary>
		public static async TreeTask<T> TimeOut<T>(this TreeTask<T> self, float timeOut)
		{
			self.Parent.AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(timeOut).Coroutine();
			return await self.AddToken(treeTaskToken);
		}
	}
}