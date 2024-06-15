
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 17:17

* 描述： 节点回调的异步扩展

*/

using System.Runtime.CompilerServices;
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

		public static TreeTaskBox TreeTaskBox(this INode self,Task task)
		{
			self.AddTemp(out TreeTaskBox treeTaskBox);
			treeTaskBox.task = task;
			return treeTaskBox;
		}

		public static TreeTaskBox<T> TreeTaskBox<T>(this INode self, Task<T> task)
		{ 
			self.AddTemp(out TreeTaskBox<T> treeTaskBox);
			treeTaskBox.task = task;
			return treeTaskBox;
		}


		public static async TreeTask GetAwaiter(this INode self, Task task)
		{
			if (task.IsCompleted)
			{
				self.Log($"进同步Task!!!!!!!!!!!!!");
				await self.TreeTaskCompleted();
			}
			else
			{
				TreeTask treeTask = self.AddTemp(out TreeTask _);
				Task task1 = task.ContinueWith(t =>
				{
					treeTask.Log($"有回调Task!!!!!!!!!!!!![{treeTask.Id}] {treeTask}");
					treeTask.Core.worldContext.Post(treeTask.SetResult);
				});

				await treeTask;
			}
		}

		public static async TreeTask<T> GetAwaiter<T>(this INode self, Task<T> task)
		{

			if (task.IsCompleted)
			{
				self.Log($"进同步!!!!!!!!!!!!!Task<{typeof(T)}>");

				await self.TreeTaskCompleted();
				return task.Result;
			}
			else
			{
				TreeTask<T> treeTask = self.AddTemp(out TreeTask<T> _);
				task.GetAwaiter().OnCompleted(() =>
				{
					treeTask.Core.worldContext.Post((x) => treeTask.SetResult((T)x), task.Result);
					treeTask.Log($"有回调Task<{typeof(T)}>!!!!!!!!!!!!![{treeTask.Id}] {treeTask}");

				});
				Task task1 = task.ContinueWith(t =>
				{
					treeTask.Log($"有回调Task<{typeof(T)}>!!!!!!!!!!!!![{treeTask.Id}] {treeTask}");
				});
				return await treeTask;
			}
		}

		/// <summary>
		/// 令牌超时取消 （秒）
		/// </summary>
		public static async TreeTask TimeOut(this TreeTaskToken self, float TimeOut)
		{
			if (TimeOut <= 0 || self.State == TokenState.Cancel)
			{
				await self.TreeTaskCompleted();
			}

			await self.AsyncDelay(TimeOut);

			if (self.State == TokenState.Cancel) return;

			self.Cancel();
		}

		/// <summary>
		/// 超时任务
		/// </summary>

		public static async TreeTask TimeOut(this TreeTask self, float TimeOut)
		{
			self.Parent.AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(TimeOut).Coroutine();
			await self.AddToken(treeTaskToken);
		}

		/// <summary>
		/// 超时任务
		/// </summary>
		public static async TreeTask<T> TimeOut<T>(this TreeTask<T> self, float TimeOut)
		{
			self.Parent.AddTemp(out TreeTaskToken treeTaskToken);
			treeTaskToken.TimeOut(TimeOut).Coroutine();
			return await self.AddToken(treeTaskToken);
		}

	}
}
