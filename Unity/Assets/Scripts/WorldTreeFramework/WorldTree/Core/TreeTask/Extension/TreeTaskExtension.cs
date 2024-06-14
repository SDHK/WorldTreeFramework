
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


		public static async TreeTask GetAwaiter(this INode self, Task task)
		{

			if (task.IsCompleted)
			{
				self.Log("Task任务已完成？？？？？？？？？？？？？？");
				await self.TreeTaskCompleted();
			}
			else
			{
				TreeTask treeTask = self.AddTemp(out TreeTask _);
				task.GetAwaiter().OnCompleted(treeTask.SetResult);
				await treeTask;
			}
		}

		public static async TreeTask<T> GetAwaiter<T>(this INode self, Task<T> task)
		{

			if (task.IsCompleted)
			{
				self.Log($"Task<{typeof(T)}>任务已完成？？？？？？？？？？？？？？");
				await self.TreeTaskCompleted();
				return task.GetAwaiter().GetResult();
			}
			else
			{
				TreeTask treeTask = self.AddTemp(out TreeTask _);
				task.GetAwaiter().OnCompleted(treeTask.SetCompleted);
				await treeTask;
				return task.GetAwaiter().GetResult();
			}
		}

	}
}
